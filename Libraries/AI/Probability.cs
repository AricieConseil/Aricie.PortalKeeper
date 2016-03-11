using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using aima.core.probability;
using aima.core.probability.bayes;
using aima.core.probability.bayes.approx;
using aima.core.probability.bayes.exact;
using aima.core.probability.bayes.impl;
using aima.core.probability.bayes.model;
using aima.core.probability.domain;
using aima.core.probability.hmm;
using aima.core.probability.hmm.exact;
using aima.core.probability.hmm.impl;
using aima.core.probability.proposition;
using aima.core.probability.temporal.generic;
using aima.core.probability.util;
using aima.core.util;
using AIFH_Vol3_Core.Core.DBNN;
using Aricie.Collections;
using Aricie.ComponentModel;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Entities;
using Aricie.DNN.Services;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.Services;
using Encog.Cloud.Indicator;
using java.lang;
using java.util;
using MathNet.Numerics.LinearAlgebra.Complex;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Matrix = aima.core.util.math.Matrix;


namespace Aricie.PortalKeeper.AI.Probability
{

    public enum RandomVariableDomainType
    {
        Boolean,
        FiniteInteger,
        ArbitraryToken,
    }

    public class RandomVariableInfo: NamedIdentifierEntity
    {
        public RandomVariableInfo()
        {
            IntValues = new List<int>();
            ArbitraryValues = new AnonymousGeneralVariableInfo<IEnumerable>();
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RandomVariableDomainType DomainType { get; set; }

        [ConditionalVisible("DomainType", false, true, RandomVariableDomainType.FiniteInteger)]
        public  List<int> IntValues { get; set; }

        public bool ShouldSerializeIntValues()
        {
            return DomainType == RandomVariableDomainType.FiniteInteger;
        }

        [ConditionalVisible("DomainType", false, true, RandomVariableDomainType.ArbitraryToken)]
        public AnonymousGeneralVariableInfo<IEnumerable> ArbitraryValues { get; set; }

        public bool ShouldSerializeArbitraryValues()
        {
            return DomainType == RandomVariableDomainType.FiniteInteger;
        }

      public RandomVariable GetRandomVariable(object owner, IContextLookup globalVars)
        {
            Domain objDomain;
            switch (DomainType)
            {
                case RandomVariableDomainType.Boolean:
                    objDomain = new BooleanDomain();
                    break;
                case RandomVariableDomainType.FiniteInteger:

                    objDomain =
                        new FiniteIntegerDomain(IntValues.ConvertAll<Integer>(intObj => new Integer(intObj)).ToArray());   
                    break;
                case RandomVariableDomainType.ArbitraryToken:
                    var enumerable = ArbitraryValues.EvaluateTyped(owner, globalVars);
                    var asList = new System.Collections.ArrayList();
                    foreach (object o in enumerable)
                    {
                        asList.Add(o);
                    }
                    objDomain =
                        new ArbitraryTokenDomain(asList.ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new RandVar(Name, objDomain);
        }
    }

    [DefaultProperty("RandomVariable")]
    public class BayesianNodeInfo
    {

        public BayesianNodeInfo()
        {
            RandomVariable = "";
            Parents = new List<string>();
            ConditionalProbabilities = new List<double>();

        }

        public string RandomVariable { get; set; }

        public List<string> Parents { get; set; }

        public bool ShouldSerializeParents()
        {
            return Parents.Count > 0 ;
        }

        public List<double> ConditionalProbabilities { get; set; }


        public FullCPTNode GetNode( IEnumerable<Node> previousNodes, IDictionary<string, RandomVariable> randomVariables)
        {
            var parentNodes = new List<Node>();
            foreach (string parentName in Parents)
            {
                foreach (Node previousNode in previousNodes)
                {
                    if (previousNode.getRandomVariable().getName() == parentName)
                    {
                        parentNodes.Add(previousNode);
                    }
                }
            }
            RandomVariable randomVar;
            if (! randomVariables.TryGetValue(RandomVariable, out randomVar))
            {
                throw new ApplicationException("Missing variable " + RandomVariable);
            }
            return  new FullCPTNode(randomVar, ConditionalProbabilities.ToArray(), parentNodes.ToArray());
        }

        public ProbabilityTable GetProbabilityTable(IDictionary<string, RandomVariable> randomVariables)
        {
            var listRandomVars = new List<RandomVariable>();
            RandomVariable randomVar;
            if (!randomVariables.TryGetValue(RandomVariable, out randomVar))
            {
                throw new ApplicationException("Missing variable " + RandomVariable);
            }
            listRandomVars.Add(randomVar);
            foreach (string parentName in Parents)
            {
                RandomVariable parentVar;
                if (!randomVariables.TryGetValue(parentName, out parentVar))
                {
                    throw new ApplicationException("Missing variable " + parentName);
                }
                listRandomVars.Add(parentVar);
            }
            return  new ProbabilityTable(ConditionalProbabilities.ToArray(), listRandomVars.ToArray());
        }

    }

    public class BayesianNetworkInfo
    {

        public BayesianNetworkInfo()
        {
            RandomVariables = new List<RandomVariableInfo>();
            Nodes = new List<BayesianNodeInfo>();
        }


        public List<RandomVariableInfo> RandomVariables { get; set; }


        public List<BayesianNodeInfo> Nodes { get; set; }

        public IDictionary<string, RandomVariable> GetRandomVariables(object owner, IContextLookup globalVars)
        {
            return GetRandomVariables(owner, globalVars, null);
        }

        public IDictionary<string, RandomVariable> GetRandomVariables(object owner, IContextLookup globalVars, IDictionary<string, RandomVariable> existingVars)
        {
            IDictionary<string, RandomVariable> randomVars = existingVars;
            if (randomVars == null)
            {
                randomVars = new Dictionary<string, RandomVariable>(RandomVariables.Count);
            }
            foreach (RandomVariableInfo randomVariableInfo in RandomVariables)
            {
                if (!randomVars.ContainsKey(randomVariableInfo.Name))
                {
                    randomVars[randomVariableInfo.Name] = randomVariableInfo.GetRandomVariable(owner, globalVars);
                }
            }
            return randomVars;
        }

        public BayesianNetwork GetNetwork(IDictionary<string, RandomVariable> randomVariables,
            IEnumerable<BayesianNodeInfo> objNodes)
        {
            var parentNodes = new List<Node>();
            var currentNodes = new List<Node>();
            foreach (BayesianNodeInfo node in objNodes)
            {
                var newNode = node.GetNode(currentNodes, randomVariables);
                currentNodes.Add(newNode);
                if (node.Parents.Count == 0)
                {
                    parentNodes.Add(newNode);
                }

            }
            return new BayesNet(parentNodes.ToArray());

        }


        public virtual BayesianNetwork  GetNetwork(IDictionary<string, RandomVariable> randomVariables)
        {
            return GetNetwork(randomVariables, Nodes);
        }

    }

    public enum BayesInferenceProcedure
    {
        EnumerationAsk,
        EliminationAsk,
        ApproxAdapter,
    }

    public enum BayesApproxAdapterMode
    {
        RejectionSampling,
        LikelihoodWeighting,
        GibbsAsk,
    }


    public class BayesInferenceInfo
    {
        public BayesInferenceInfo()
        {
            Procedure = BayesInferenceProcedure.EnumerationAsk;
            ApproxAdapterMode = BayesApproxAdapterMode.RejectionSampling;
            SampleNb = 1000;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof (StringEnumConverter))]
        public BayesInferenceProcedure Procedure { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof (StringEnumConverter))]
        [ConditionalVisible("Procedure", false, true, BayesInferenceProcedure.ApproxAdapter)]
        public BayesApproxAdapterMode ApproxAdapterMode { get; set; }

        public bool ShouldSerializeApproxAdapterMode()
        {
            return Procedure == BayesInferenceProcedure.ApproxAdapter;
        }


        [ConditionalVisible("Procedure", false, true, BayesInferenceProcedure.ApproxAdapter)]
        public int SampleNb { get; set; }

        public bool ShouldSerializeSampleNb()
        {
            return Procedure == BayesInferenceProcedure.ApproxAdapter;
        }

        public BayesInference GetInference()
        {
            switch (Procedure)
            {
                case BayesInferenceProcedure.EnumerationAsk:
                    return new EnumerationAsk();
                case BayesInferenceProcedure.EliminationAsk:
                    return new EliminationAsk();
                case BayesInferenceProcedure.ApproxAdapter:
                    switch (ApproxAdapterMode)
                    {
                        case BayesApproxAdapterMode.RejectionSampling:
                            return new BayesInferenceApproxAdapter((BayesSampleInference) new RejectionSampling(), SampleNb);
                        case BayesApproxAdapterMode.LikelihoodWeighting:
                            return new BayesInferenceApproxAdapter((BayesSampleInference) new LikelihoodWeighting(), SampleNb);
                        case BayesApproxAdapterMode.GibbsAsk:
                            return new BayesInferenceApproxAdapter((BayesSampleInference) new GibbsAsk(), SampleNb);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum PropositionType
    {
        RandomVariable,
        Assignment,
        Dysjunctive,
        Conjunctive
    }

    [DefaultProperty("FriendlyName")]
    public class PropositionInfo
    {

        public PropositionInfo()
        {
            RandomVariable = "";
            Value = new AnonymousGeneralVariableInfo<object>();
        }

        

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PropositionType PropositionType { get; set; }

        [ConditionalVisible("PropositionType", false, true, PropositionType.RandomVariable, PropositionType.Assignment)]
        public string RandomVariable { get; set; }

        public bool ShouldSerializeRandomVariable()
        {
            return PropositionType == PropositionType.Assignment || PropositionType == PropositionType.RandomVariable;
        }

        [ConditionalVisible("PropositionType", false, true, PropositionType.Assignment)]
        public AnonymousGeneralVariableInfo<object>  Value { get; set; }

        public bool ShouldSerializeValue()
        {
            return PropositionType == PropositionType.Assignment;
        }


        [ConditionalVisible("PropositionType", false, true, PropositionType.Conjunctive, PropositionType.Dysjunctive)]
        public List<PropositionInfo> Children { get; set; }

        public bool ShouldSerializeChildren()
        {
            return PropositionType != PropositionType.Assignment && PropositionType != PropositionType.RandomVariable;
        }

        public Proposition GetProposition(object owner, IContextLookup globalVars, IDictionary<string, RandomVariable> randomVariables)
        {
            switch (PropositionType)
            {
                case PropositionType.RandomVariable:
                case PropositionType.Assignment:
                    RandomVariable randomVar;
                    if (!randomVariables.TryGetValue(RandomVariable, out randomVar))
                    {
                        throw new ApplicationException("Missing variable " + RandomVariable);
                    }
                    switch (PropositionType)
                    {
                        case PropositionType.RandomVariable:
                            return (Proposition) randomVar;
                        case PropositionType.Assignment:
                            var objValue = Value.EvaluateTyped(owner, globalVars);
                            return new AssignmentProposition(randomVar, objValue);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PropositionType.Dysjunctive:
                case PropositionType.Conjunctive:
                    if (Children.Count > 0)
                    {
                        Proposition newPropositionInfo = null;
                        foreach (PropositionInfo assignmentPropositionInfo in Children)
                        {
                            var objAssignment = assignmentPropositionInfo.GetProposition(owner, globalVars, randomVariables);
                            if (newPropositionInfo == null)
                            {
                                newPropositionInfo = objAssignment;
                            }
                            else
                            {
                                if (PropositionType == PropositionType.Conjunctive)
                                {
                                    newPropositionInfo = new ConjunctiveProposition(newPropositionInfo, objAssignment);
                                }
                                else
                                {
                                    newPropositionInfo = new DisjunctiveProposition(newPropositionInfo, objAssignment);
                                }
                            }
                        }
                        return newPropositionInfo;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [XmlIgnore()]
        [Browsable(false)]
        public string FriendlyName =>  (PropositionType == PropositionType.Assignment )? $"{RandomVariable} = {Value.FriendlyName}": $"{PropositionType.ToString()}";
    }

    public enum BayesianQueryType
    {
        Prior,
        Posterior,
        Joint
    }

    public class BayesianAskInfo
    {

        public BayesianAskInfo()
        {
            Prior = new List<PropositionInfo>();
            Posteriors = new List<PropositionInfo>();
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public BayesianQueryType QueryType { get; set; }

        public List<PropositionInfo> Prior { get; set; }

        [ConditionalVisible("QueryType", false, true, BayesianQueryType.Posterior)]
        public List<PropositionInfo> Posteriors { get; set; }

        public bool ShouldSerializePosteriors()
        {
            return Posteriors.Count > 0;
        }


        public CategoricalDistribution AskBayesianModel(object owner, IContextLookup globalVars, IDictionary<string, RandomVariable> randomVariables, FiniteProbabilityModel model)
        {
            
            var priorProps = Prior.Select(objProp => objProp.GetProposition(owner, globalVars, randomVariables));
            switch (QueryType)
            {
                case BayesianQueryType.Prior:
                    return model.priorDistribution(priorProps.ToArray());
                case BayesianQueryType.Posterior:
                    var posteriorProps = Posteriors.Select(objProp => objProp.GetProposition(owner, globalVars, randomVariables));
                    return model.posteriorDistribution(priorProps.ToArray()[0], posteriorProps.ToArray());
                case BayesianQueryType.Joint:
                    return model.jointDistribution((priorProps.ToArray()));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class BayesianQueryInfo
    {
        public BayesianQueryInfo()
        {
            BayesianNetwork = new BayesianNetworkInfo();
            InferenceType = new BayesInferenceInfo();
            Ask = new BayesianAskInfo();

        }


        [ExtendedCategory("Query")]
        public BayesInferenceInfo InferenceType { get; set; }

        [ExtendedCategory("Query")]
        public BayesianAskInfo Ask { get; set; }

        [ExtendedCategory("Model")]
        public BayesianNetworkInfo BayesianNetwork { get; set; }

        public CategoricalDistribution AskBayesianModel(object owner, IContextLookup globalVars)
        {
            var randomVariables = BayesianNetwork.GetRandomVariables(owner, globalVars);
            var network = BayesianNetwork.GetNetwork(randomVariables);
            var inference = InferenceType.GetInference();
            var model = new FiniteBayesModel(network, inference);
            return Ask.AskBayesianModel(owner, globalVars, randomVariables, model);
        }
    }

    public class TemporalModelInfo
    {

        public TemporalModelInfo()
        {
            TransitionModel = new BayesianNetworkInfo();
            SensorModel = new BayesianNetworkInfo();
            ReverseTemporalVariableMap = new SerializableDictionary<string, string>();
        }

        public BayesianNetworkInfo TransitionModel { get; set; }

        public BayesianNetworkInfo SensorModel { get; set; }

        public SerializableDictionary<string,string> ReverseTemporalVariableMap { get; set; }

        public Map GetReverseTemporalMap(IDictionary<string, RandomVariable> randomVariables)
        {
            HashMap hashMap = new HashMap();
            foreach (KeyValuePair<string, string> pair in ReverseTemporalVariableMap)
            {
                RandomVariable keyVar, valueVar;
                if (!randomVariables.TryGetValue(pair.Key, out keyVar) || !randomVariables.TryGetValue(pair.Value, out valueVar))
                {
                    throw new ApplicationException("Missing variable " + pair.Key + " or " + pair.Value);
                }
                hashMap.put(keyVar, valueVar);
            }
            return (Map)hashMap;
        }
        
    }


    public abstract class TemporalAskInfo
    {

        public TemporalAskInfo()
        {
            Evidences = new List<List<PropositionInfo>>();
        }

        [ExtendedCategory("Query")]
        public List<List<PropositionInfo>> Evidences { get; set; }


        public abstract List<CategoricalDistribution> Ask(object owner, IContextLookup globalVars);

    }

    public class ForwardBackwardInfo: TemporalAskInfo
    {

        public ForwardBackwardInfo()
        {
            TemporalModel = new TemporalModelInfo();
            Prior = new BayesianNodeInfo();
            
        }

        [ExtendedCategory("Query")]
        public BayesianNodeInfo Prior { get; set; }

       

        [ExtendedCategory("Model")]
        public TemporalModelInfo TemporalModel { get; set; }

        public List<CategoricalDistribution> DoForwardBackward(object owner, IContextLookup globalVars)
        {
            var randomVariables = TemporalModel.TransitionModel.GetRandomVariables(owner, globalVars);
            var transitionalModel = new FiniteBayesModel( TemporalModel.TransitionModel.GetNetwork(randomVariables));
            
            randomVariables = TemporalModel.SensorModel.GetRandomVariables(owner, globalVars, randomVariables);
            var sensoryModel = new FiniteBayesModel(TemporalModel.SensorModel.GetNetwork(randomVariables));

            var temporalMap = TemporalModel.GetReverseTemporalMap(randomVariables);

            var forwardBackwardAlgorithm = new ForwardBackward(transitionalModel, temporalMap, sensoryModel);

            var objEvidences = new java.util.ArrayList(Evidences.Count);
            foreach (List<PropositionInfo> propositions in Evidences)
            {
                var stepEvidences = new java.util.ArrayList(propositions.Count);
                foreach (PropositionInfo proposition in propositions)
                {
                    stepEvidences.add(proposition.GetProposition(owner, globalVars, randomVariables));
                }
                objEvidences.add(stepEvidences);
            }

            CategoricalDistribution objPrior = Prior.GetProbabilityTable(randomVariables);
            return forwardBackwardAlgorithm.forwardBackward(objEvidences, objPrior).toArray().Select(o=>(CategoricalDistribution)o).ToList();
        }

        public override List<CategoricalDistribution> Ask(object owner, IContextLookup globalVars)
        {
           return DoForwardBackward( owner,  globalVars);
        }
    }


    [DefaultProperty("Rows")]
    public class MatrixInfo
    {

        public MatrixInfo()
        {
            Rows = new List<List<double>>();
        }

        public List<List<double>> Rows { get; set; }

        public Matrix GetMatrix()
        {
            var array = Rows.Select(row => row.ToArray()).ToArray();
            return  new Matrix(array);
        }
        
    }

    [DefaultProperty("FriendlyName")]
    public class SensorMapRowInfo
    {

        [XmlIgnore()]
        [Browsable(false)]
        public string FriendlyName =>  $"{Value.FriendlyName} {ReflectionHelper.GetFriendlyName(SensorModel)}" ;

        public SensorMapRowInfo()
        {
            Value= new AnonymousGeneralVariableInfo<object>();
            SensorModel = new MatrixInfo();
        }

        public AnonymousGeneralVariableInfo<object> Value { get; set; }

        public MatrixInfo SensorModel { get; set; }

    }
   
    public class HiddenMarkovModelInfo
    {

        public HiddenMarkovModelInfo()
        {
            StateVariable = new RandomVariableInfo() {Name = "State"};
            ObservationVariable = new RandomVariableInfo() { Name = "Evidence" };
            TransitionModel = new MatrixInfo();
            SensorModel = new List<SensorMapRowInfo>();
            Prior = new MatrixInfo();
        }

        public RandomVariableInfo StateVariable { get; set; }

        public RandomVariableInfo ObservationVariable { get; set; }

        public MatrixInfo TransitionModel { get; set; }

        public List<SensorMapRowInfo> SensorModel { get; set; }

        public MatrixInfo Prior { get; set; }


        public HiddenMarkovModel GetModel(object owner, IContextLookup globalVars)
        {
            var randomVar = StateVariable.GetRandomVariable(owner, globalVars);
            var transition = TransitionModel.GetMatrix();
            var sensor = new HashMap();
            foreach (SensorMapRowInfo row in SensorModel)
            {
                var value = row.Value.Evaluate(owner, globalVars);
                var distribution = row.SensorModel.GetMatrix();
                sensor.put(value, distribution);
            }
            var objPrior = Prior.GetMatrix();
            return  new HMM(randomVar,transition,sensor,objPrior);
        }

    }

    public class FixedLagSmoothingInfo : TemporalAskInfo
    {

        public FixedLagSmoothingInfo()
        {
            HiddenMarkovModel = new HiddenMarkovModelInfo();
            LagLength = 1;
        }


        [ExtendedCategory("Query")]
        public int LagLength { get; set; }

        [ExtendedCategory("Model")]
        public HiddenMarkovModelInfo HiddenMarkovModel { get; set; }

        public List<CategoricalDistribution> AskFixedLagSmoothing(object owner, IContextLookup globalVars)
        {
            var model = HiddenMarkovModel.GetModel(owner, globalVars);
            
            var stateVar = model.getStateVariable();
            var evidenceVar = HiddenMarkovModel.ObservationVariable.GetRandomVariable(owner, globalVars);
            var randomVarDico = new Dictionary<string, RandomVariable> { { stateVar.getName(), stateVar }, { evidenceVar.getName(), evidenceVar } };

            var objAlgorithm = new FixedLagSmoothing(model, LagLength);
            
            var objEvidences = new java.util.ArrayList(Evidences.Count);
            var toReturn = new List<CategoricalDistribution>();
            foreach (List<PropositionInfo> propositions in Evidences)
            {
                var stepEvidences = new java.util.ArrayList(propositions.Count);
                foreach (PropositionInfo proposition in propositions)
                {
                    stepEvidences.add(proposition.GetProposition(owner, globalVars, randomVarDico));
                }
                toReturn.Add( objAlgorithm.fixedLagSmoothing(stepEvidences)); 
            }
            return toReturn;
        }

        public override List<CategoricalDistribution> Ask(object owner, IContextLookup globalVars)
        {
            return AskFixedLagSmoothing(owner, globalVars);
        }

    }


    public class DynamicBayesianNetworkInfo:BayesianNetworkInfo
    {

        public DynamicBayesianNetworkInfo()
        {
            PriorNodes = new List<string>();
            SensoryNodes = new List<string>();
            TemporalVariableMap = new SerializableDictionary<string, string>();
        }

        [ExtendedCategory("Temporal")]
        public List<string> PriorNodes { get; set; }

        [ExtendedCategory("Temporal")]
        public List<string> SensoryNodes { get; set; }

        [ExtendedCategory("Temporal")]
        public SerializableDictionary<string, string> TemporalVariableMap { get; set; }
        

        //public BayesianNetworkInfo TransitionModel { get; set; }

        //public BayesianNetworkInfo SensorModel { get; set; }

        public override BayesianNetwork GetNetwork(IDictionary<string, RandomVariable> randomVariables)
        {
            var completeNet = base.GetNetwork(randomVariables);
           
            var rootNodes = new List<Node>();
            var priorNodes = new List<BayesianNodeInfo>();
            var sensorVars = new HashSet();
            var allVariables = completeNet.getVariablesInTopologicalOrder().toArray();
            foreach (RandomVariable variable in allVariables)
            {
                var varName = variable.getName();
                var objNode = completeNet.getNode(variable);
                if (objNode.isRoot())
                {
                    rootNodes.Add(objNode);
                }
                if (PriorNodes.Contains(varName))
                {
                    priorNodes.Add(this.Nodes.Find(objNodeInfo => objNodeInfo.RandomVariable == varName));
                }
                if (SensoryNodes.Contains(varName))
                {
                    sensorVars.Add(variable);   
                }
            }

            var priorNet = GetNetwork(randomVariables, priorNodes);
            var Xtm1ToXt = GetTemporalMap(randomVariables);
            return new DynamicBayesNet(priorNet,Xtm1ToXt,sensorVars, rootNodes.ToArray());
        }

        public Map GetTemporalMap(IDictionary<string, RandomVariable> randomVariables)
        {
            HashMap hashMap = new HashMap();
            foreach (KeyValuePair<string, string> pair in TemporalVariableMap)
            {
                RandomVariable keyVar, valueVar;
                if (!randomVariables.TryGetValue(pair.Key, out keyVar) || !randomVariables.TryGetValue(pair.Value, out valueVar))
                {
                    throw new ApplicationException("Missing variable " + pair.Key + " or " + pair.Value);
                }
                hashMap.put(keyVar, valueVar);
            }
            return (Map)hashMap;
        }

    }

    public enum RandomizerType
    {
        Random,
        Mock   
    }

    public class ParticleFilteringInfo : TemporalAskInfo
    {

        public ParticleFilteringInfo()
        {
            DynamicBayesianNetwork = new DynamicBayesianNetworkInfo();
            RandomizerValues = new List<double>();
            SampleNb = 10;
        }

        [ExtendedCategory("Query")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RandomizerType RandomizerType { get; set; }

        [ExtendedCategory("Query")]
        [ConditionalVisible("RandomizerType", false, true, RandomizerType.Mock)]
        public List<double> RandomizerValues { get; set; }

        [ExtendedCategory("Query")]
        public int SampleNb { get; set; }


        [ExtendedCategory("Model")]
        public DynamicBayesianNetworkInfo DynamicBayesianNetwork { get; set; }

        public List<AssignmentProposition[][]> AskParticleFiltering(object owner, IContextLookup globalVars)
        {
            var randomVars = DynamicBayesianNetwork.GetRandomVariables(owner, globalVars);
            var model = DynamicBayesianNetwork.GetNetwork(randomVars);

            Randomizer objRandomizer;
            switch (RandomizerType)
            {
                case RandomizerType.Random:
                    objRandomizer = new JavaRandomizer();
                    break;
                case RandomizerType.Mock:
                    objRandomizer = new MockRandomizer(RandomizerValues.ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var objAlgorithm = new ParticleFiltering(SampleNb,(DynamicBayesianNetwork) model, objRandomizer);

            var objEvidences = new java.util.ArrayList(Evidences.Count);
            var toReturn = new List<AssignmentProposition[][]>();
            foreach (List<PropositionInfo> propositions in Evidences)
            {
                var stepEvidences = new List<AssignmentProposition>(propositions.Count);
                foreach (PropositionInfo proposition in propositions)
                {
                    stepEvidences.Add((AssignmentProposition) proposition.GetProposition(owner, globalVars, randomVars));
                }
                toReturn.Add(objAlgorithm.particleFiltering(stepEvidences.Cast<AssignmentProposition>().ToArray()));
            }
            return toReturn;
        }

        public override List<CategoricalDistribution> Ask(object owner, IContextLookup globalVars)
        {
            throw new NotImplementedException();
        }
    }
}    