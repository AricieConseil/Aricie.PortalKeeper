using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using aima.core.search.csp;
using Aricie.Collections;
using Aricie.ComponentModel;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Modules.PortalKeeper;
using Aricie.DNN.Services;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls;
using Aricie.PortalKeeper.AI.Games;
using Aricie.PortalKeeper.AI.Search;
using Aricie.Services;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;
using java.lang;
using java.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ArrayList = System.Collections.ArrayList;
using Boolean = System.Boolean;
using Exception = System.Exception;
using Object = System.Object;
using String = System.String;


namespace Aricie.PortalKeeper.AI.CSP
{
    public enum CSPStrategy
    {
        BacktrackingStrategy,
        ImprovedBacktrackingStrategy,
        MinConflictsStrategy,
    }

    public enum CSPInference
    {
        None,
        ForwardChecking,
        AC3,
    }

    public enum CSPSelection
    {
        DefaultOrder,
        MRV,
        MRVDeg,
    }

    public class CSPStrategyInfo
    {
        public CSPStrategyInfo()
        {
            MaxSteps = 50;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public CSPStrategy StrategyType { get; set; }

        [ConditionalVisible("StrategyType", false, true, CSPStrategy.ImprovedBacktrackingStrategy)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CSPSelection Selection { get; set; }

        [ConditionalVisible("StrategyType", false, true, CSPStrategy.ImprovedBacktrackingStrategy)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CSPInference Inference { get; set; }


        [ConditionalVisible("StrategyType", false, true, CSPStrategy.ImprovedBacktrackingStrategy)]
        public bool EnableLCV { get; set; }

        [ConditionalVisible("StrategyType", false, true, CSPStrategy.MinConflictsStrategy)]
        public int MaxSteps { get; set; }

        public SolutionStrategy GetStrategy()
        {
            SolutionStrategy toReturn;
            switch (StrategyType)
            {
                case CSPStrategy.BacktrackingStrategy:
                    toReturn = new BacktrackingStrategy();
                    break;
                case CSPStrategy.ImprovedBacktrackingStrategy:
                    var improved = new ImprovedBacktrackingStrategy();
                    toReturn = improved;
                    improved.enableLCV(EnableLCV);
                    switch (Selection)
                    {
                        case CSPSelection.DefaultOrder:
                            break;
                        case CSPSelection.MRV:
                            improved.setVariableSelection( ImprovedBacktrackingStrategy.Selection.MRV);
                            break;
                        case CSPSelection.MRVDeg:
                            improved.setVariableSelection(ImprovedBacktrackingStrategy.Selection.MRV_DEG);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    switch (Inference)
                    {
                        case CSPInference.None:
                            break;
                        case CSPInference.ForwardChecking:
                            improved.setInference(ImprovedBacktrackingStrategy.Inference.FORWARD_CHECKING);
                            break;
                        case CSPInference.AC3:
                            improved.setInference(ImprovedBacktrackingStrategy.Inference.AC3);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case CSPStrategy.MinConflictsStrategy:
                    toReturn = new MinConflictsStrategy(MaxSteps);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return toReturn;
        }
    }

    
   

    [SkipModelValidation()]
    public class CSPInfo
    {
        public CSPInfo()
        {
            Strategy = new AnonymousGeneralVariableInfo<SolutionStrategy>();
            CSP = new AnonymousGeneralVariableInfo<aima.core.search.csp.CSP>();
            StateListeners = new Variables<CSPStateListener>();
        }

       
        [ExtendedCategory("Strategy")]
        public AnonymousGeneralVariableInfo<aima.core.search.csp.SolutionStrategy> Strategy { get; set; }

        [ExtendedCategory("Problem")]
        public AnonymousGeneralVariableInfo<aima.core.search.csp.CSP> CSP { get; set; }


        [ExtendedCategory("Listeners")]
        public Variables<CSPStateListener> StateListeners { get; set; }
        
        public Assignment SolveCSP(object owner, IContextLookup globalVars)
        {
           
            var objStrategy = Strategy.EvaluateTyped(owner, globalVars);
            var objListeners = StateListeners.EvaluateGeneric(owner, globalVars);
            foreach (KeyValuePair<string, CSPStateListener> keyValuePairListener in objListeners)
            {
                objStrategy.addCSPStateListener(keyValuePairListener.Value);
                globalVars.Items[keyValuePairListener.Key] = keyValuePairListener.Value;
            }
            var objCSP = CSP.EvaluateTyped(owner, globalVars);
            return objStrategy.solve(objCSP);
        }


        
    }


    [ActionButton(IconName.ClockO, IconOptions.Normal)]
    public class StepCounter :  CSPStateListener
    {
        [IsReadOnly(true)]
        public int AssignmentCount { get; set; }

        [IsReadOnly(true)]
        public int DomainCount { get; set; }

        public StepCounter()
        {
            this.AssignmentCount = 0;
            this.DomainCount = 0;
        }

        public virtual void stateChanged(Assignment assignment, aima.core.search.csp.CSP csp)
        {
            ++this.AssignmentCount;
        }

        public virtual void stateChanged(aima.core.search.csp.CSP csp)
        {
            ++this.DomainCount;
        }

        public virtual void reset()
        {
            this.AssignmentCount = 0;
            this.DomainCount = 0;
        }
        
        public virtual string getResults()
        {
            StringBuffer stringBuffer = new StringBuffer();
            stringBuffer.append(new StringBuilder().append("assignment changes: ").append(this.AssignmentCount).toString());
            if (this.DomainCount != 0)
                stringBuffer.append(new StringBuilder().append(", domain changes: ").append(this.DomainCount).toString());
            return stringBuffer.toString();
        }


        [ActionButton(IconName.Refresh, IconOptions.Normal)]
        public void Reset(AriciePropertyEditorControl ape)
        {

            reset();
            ape.DisplayMessage("Counter was reset", ModuleMessage.ModuleMessageType.GreenSuccess);
            ape.ItemChanged = true;
        }

        public override string ToString()
        {
            return getResults();
        }
    }

    [DefaultProperty("ConstraintExpression")]
    public class DynamicConstraint : Constraint
    {

        public DynamicConstraint()
        {
            ConstraintExpression = new FleeExpressionInfo<bool>();
            Scope = new List<string>();
        }

        public FleeExpressionInfo<Boolean> ConstraintExpression { get; set; }

        public List<String> Scope { get; set; }

        public List getScope()
        {
            var toReturn = (List)new java.util.ArrayList();
            foreach (string s in Scope)
            {
                toReturn.add(new Variable(s));
            }

            return toReturn;
        }

        public bool isSatisfiedWith(Assignment a)
        {
            var assignedVars = a.getVariables();
            var dico = new Dictionary<String, Object>();
            foreach (Variable assignedVar in assignedVars.toArray())
            {
                dico[assignedVar.getName()] = a.getAssignment(assignedVar);
            }
            var context = new PortalKeeperContext<SimpleEngineEvent>();
            context.InitParams(dico);
            return ConstraintExpression.Evaluate(context, context);

        }
    }

    public class DynamicCSPInfo : IExpressionVarsProvider
    {
        public DynamicCSPInfo()
        {
            Variables = new AnonymousGeneralVariableInfo<IEnumerable>();
            Constraints = new AnonymousGeneralVariableInfo<IEnumerable<Constraint>>();
            Domains = new Variables<IEnumerable>();
            VariableDomains = new SimpleOrExpression<SerializableDictionary<string, string>>();
        }

        [ExtendedCategory("Domains")]
        public Variables<IEnumerable> Domains { get; set; }

        [ExtendedCategory("Variables")]
        public AnonymousGeneralVariableInfo<IEnumerable> Variables { get; set; }

        [Browsable(false)]
        public bool HasMultipleDomains
        {
            get { return Domains.Instances.Count > 1; }
        }

        [ConditionalVisible("HasMultipleDomains")]
        [ExtendedCategory("Variables")]
        public SimpleOrExpression<SerializableDictionary<string, string>> VariableDomains { get; set; }

        [ExtendedCategory("Constraints")]
        public AnonymousGeneralVariableInfo<IEnumerable<Constraint>> Constraints { get; set; }


        private void Init(object owner, IContextLookup globalVars, DynamicCSP objCSP)
        {
            var objVariables = Variables.EvaluateTyped(owner, globalVars);
            var dicoVariables = (from object objVariable in objVariables select ReflectionHelper.GetFriendlyName(objVariable)).ToDictionary(varName => varName, varName => new Variable(varName));
            foreach (var objVariable in dicoVariables.Values)
            {
                objCSP.AddNewVariable(objVariable);
            }
            var objDomains = Domains.EvaluateGeneric(owner, globalVars);
            var dicoDomains = new Dictionary<string, Domain>();
            foreach (KeyValuePair<string, IEnumerable> keyValuePair in objDomains)
            {
                var domainArray = new ArrayList();
                foreach (object domainItem in keyValuePair.Value)
                {
                    domainArray.Add(domainItem);
                }
                Domain domain = new Domain(domainArray.ToArray());
                dicoDomains.Add(keyValuePair.Key, domain);
            }


            if (dicoDomains.Count == 0)
            {
                throw new ApplicationException("No Domain defined for CSP problem");
            }
            if (dicoDomains.Count == 1)
            {
                var domain = dicoDomains.Values.First();
                Iterator terator = objCSP.getVariables().iterator();
                while (terator.hasNext())
                    objCSP.setDomain((Variable)terator.next(), domain);
            }
            else
            {
                var varDomains = VariableDomains.GetValue(owner, globalVars);


                foreach (KeyValuePair<string, Variable> varPair in dicoVariables)
                {
                    string targetDomain;
                    Domain objDomain;
                    if (varDomains.TryGetValue(varPair.Key, out targetDomain)
                        && dicoDomains.TryGetValue(targetDomain, out objDomain))
                    {
                        objCSP.setDomain(varPair.Value, objDomain);
                    }
                    else
                    {
                        throw new ApplicationException("Incomplete Variables Domain affectation");
                    }
                }
            }

            var objConstraints = Constraints.EvaluateTyped(owner, globalVars);
            foreach (Constraint objConstraint in objConstraints)
            {
                objCSP.addConstraint(objConstraint);
            }

        }

        public aima.core.search.csp.CSP GetCsp(object owner, IContextLookup globalVars)
        {
            var toReturn = new DynamicCSP();
            Init(owner, globalVars, toReturn);
            return toReturn;
        }

        public void AddVariables(IExpressionVarsProvider currentProvider, ref IDictionary<string, Type> existingVars)
        {
            try
            {
                var dumbContext = new PortalKeeperContext<SimpleEngineEvent>();
                var objVariables = Variables.EvaluateTyped(dumbContext, dumbContext);
                foreach (object objVariable in objVariables)
                {
                    var varName = ReflectionHelper.GetFriendlyName(objVariable);
                    existingVars[varName] = typeof(object);
                }

            }
            catch (Exception){ }
           
            //For Each objVar As VariableInfo In Me.Variables.Instances
            //    existingVars(objVar.Name) = ReflectionHelper.CreateType(objVar.VariableType)
            //Next
        }

    }

    public class DynamicCSP : aima.core.search.csp.CSP
    {

        public void AddNewVariable(Variable objVariable)
        {
            this.addVariable(objVariable);
        }

    }

}

namespace Aricie.PortalKeeper.AI.Learning.Neural
{
    public enum NeuralLayerType
    {
        BasicLayer,
    }
}