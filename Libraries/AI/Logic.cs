using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using aima.core.learning.knowledge;
using aima.core.logic.fol;
using aima.core.logic.fol.domain;
using aima.core.logic.fol.inference;
using aima.core.logic.fol.inference.proof;
using aima.core.logic.fol.kb;
using aima.core.logic.fol.parsing;
using aima.core.logic.propositional.inference;
using aima.core.logic.propositional.kb;
using aima.core.logic.propositional.parsing;
using aima.core.logic.propositional.parsing.ast;
using aima.core.logic.propositional.visitors;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Services;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using java.util;

namespace Aricie.PortalKeeper.AI.Logic
{
    public class PLKbInfo
    {
       

        private KnowledgeBase _knowledgeBase;


        [Browsable(false)]
        [JsonIgnore()]
        [XmlIgnore()]
        public KnowledgeBase KnowledgeBase {
            get
            {
                if (_knowledgeBase == null)
                {
                    _knowledgeBase = new KnowledgeBase();
                    foreach (string s in _Sentences)
                    {
                        KnowledgeBase.tell(s);
                    }
                }
                return _knowledgeBase;
            }
        }



        private List<string> _Sentences;
        public List<string> Sentences
        {
            get
            {
                if (_knowledgeBase != null)
                {
                    return _knowledgeBase.getSentences().toArray().Select(objSentence => objSentence.ToString()).ToList();
                }
                return _Sentences;
            }
            set { _Sentences = value; }
        }

       
    }

    public enum PLInferenceProcedure
    {
        TTEntails,
        Resolve,
        PLFCEntails,
        DPLLEntails,
        DPLLSatisfiable,
        WalkSatKb,
        WalkSatPredicate
    }


    public class PLKbInferProcedureInfo
    {

        private PLParser parser;

        public PLKbInferProcedureInfo()
        {
            parser = new PLParser();
            WalkSatProbability = 0.5;
            WalkSatMaxFlips = 100;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PLInferenceProcedure Procedure { get; set; }

        [ConditionalVisible("Procedure", false, true, PLInferenceProcedure.WalkSatKb, PLInferenceProcedure.WalkSatPredicate)]
        public double WalkSatProbability { get; set; }

        public bool ShouldSerializeWalkSatProbability()
        {
            return Procedure == PLInferenceProcedure.WalkSatKb || Procedure == PLInferenceProcedure.WalkSatPredicate;
        }

        [ConditionalVisible("Procedure", false, true, PLInferenceProcedure.WalkSatKb, PLInferenceProcedure.WalkSatPredicate)]
        public int WalkSatMaxFlips { get; set; }

        public bool ShouldSerializeWalkSatMaxFlips()
        {
            return Procedure == PLInferenceProcedure.WalkSatKb || Procedure == PLInferenceProcedure.WalkSatPredicate;
        }

        public bool Ask(KnowledgeBase kb, string predicate)
        {
            
            
            switch (Procedure)
            {
                case PLInferenceProcedure.TTEntails:
                    return kb.askWithTTEntails(predicate);
                case PLInferenceProcedure.PLFCEntails:
                    return new PLFCEntails().plfcEntails(kb, new PropositionSymbol(predicate));
                case PLInferenceProcedure.Resolve:
                    return new PLResolution().plResolution(kb, (Sentence)parser.parse(predicate));
                case PLInferenceProcedure.DPLLEntails:
                    return new DPLLSatisfiable().isEntailed(kb, (Sentence)parser.parse(predicate));
                case PLInferenceProcedure.DPLLSatisfiable:
                    return new DPLLSatisfiable().dpllSatisfiable((Sentence)parser.parse(predicate));
                case PLInferenceProcedure.WalkSatPredicate:
                    return (new WalkSAT().walkSAT(ConvertToConjunctionOfClauses.convert((Sentence)parser.parse(predicate)).getClauses(), WalkSatProbability, WalkSatMaxFlips) != null);
                case PLInferenceProcedure.WalkSatKb:
                    return (new WalkSAT().walkSAT(kb.asCNF(), WalkSatProbability, WalkSatMaxFlips) != null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


    }


    public class PLKbInferInfo
    {

        public PLKbInferInfo()
        {
            KnowledgeBase = new AnonymousGeneralVariableInfo<KnowledgeBase>();
            Predicate = new SimpleOrExpression<string>();
            Procedure = new AnonymousGeneralVariableInfo<PLKbInferProcedureInfo>()
            {
                VariableMode = VariableMode.Instance,
            };
        }

        [ConditionalVisible("Procedure", true, true, PLInferenceProcedure.DPLLSatisfiable, PLInferenceProcedure.WalkSatPredicate)]
        public AnonymousGeneralVariableInfo<KnowledgeBase> KnowledgeBase { get; set; }

        [ConditionalVisible("Procedure", true, true,  PLInferenceProcedure.WalkSatKb)]
        public SimpleOrExpression<string> Predicate { get; set; }

        public bool ShouldSerializePredicate()
        {
            return !(Predicate.Mode == SimpleOrExpressionMode.Simple && Predicate.Simple.IsNullOrEmpty());
        }

        public AnonymousGeneralVariableInfo<PLKbInferProcedureInfo> Procedure { get; set; }

        public bool Ask(object owner, IContextLookup globalVars)
        {
            var kb = KnowledgeBase.EvaluateTyped(owner, globalVars);
            var predicate = Predicate.GetValue(owner, globalVars);
            var objInferProc = Procedure.EvaluateTyped(owner, globalVars);
            return objInferProc.Ask(kb, predicate);
        }
    }



    public class FolDomainInfo
    {

        public FolDomainInfo()
        {
            _Constants = new List<string>();
            _Predicates = new List<string>();
            _Functions = new List<string>();
        }

        private FOLDomain _domain;


        [Browsable(false)]
        [XmlIgnore()]
        public FOLDomain Domain
        {
            get
            {
                if (_domain == null)
                {
                    var objDomain = new FOLDomain();
                    foreach (string s in _Constants)
                    {
                        objDomain.addConstant(s);
                    }
                    foreach (string strPredicate in _Predicates)
                    {
                        objDomain.addPredicate(strPredicate);
                    }
                    foreach (string s in _Functions)
                    {
                        objDomain.addFunction(s);
                    }
                    _domain = objDomain;
                }
                return _domain;
            }
        }


        private List<string> _Constants;

        public List<string> Constants
        {
            get
            {
                if (_domain != null)
                {
                    return _domain.getConstants().toArray().Select(o => o.ToString()).ToList();
                }
                return _Constants;
            }
            set
            {
                _Constants = value;
            }
        }

        private List<string> _Predicates;


        public List<string> Predicates
        {
            get
            {
                if (_domain != null)
                {
                    return _domain.getPredicates().toArray().Select(objPredicate => objPredicate.ToString()).ToList();
                }
                return _Predicates;
                
            }
            set
            {
                _Predicates = value;
            }
        }

        private List<string> _Functions;

        public List<string> Functions
        {
            get
            {
                if (_domain != null)
                {
                    return _domain.getFunctions().toArray().Select(o => o.ToString()).ToList();
                }
                return _Functions;
            }
            set { _Functions = value; }
        }

      
    }

    public enum FolInferenceProcedure
    {
        OTTERLikeTheoremProver,
        BCAsk,
        FCAsk,
        ModelElimination,
        TFMResolution
    }

    public class FolKbInfo
    {
        private FOLKnowledgeBase _knowledgeBase;

        public FolKbInfo()
        {
            Domain = new FolDomainInfo();
            _Sentences = new List<string>();
        }

        public FolDomainInfo Domain { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FolInferenceProcedure Procedure { get; set; }


        [Browsable(false)]
        [XmlIgnore()]
        public FOLKnowledgeBase KnowledgeBase
        {
            get
            {
                if (_knowledgeBase == null)
                {
                    InferenceProcedure ip;
                    switch (Procedure)
                    {
                        case FolInferenceProcedure.OTTERLikeTheoremProver:
                            ip = new FOLOTTERLikeTheoremProver();
                            break;
                        case FolInferenceProcedure.BCAsk:
                            ip = new FOLBCAsk();
                            break;
                        case FolInferenceProcedure.FCAsk:
                            ip = new FOLFCAsk();
                            break;
                        case FolInferenceProcedure.ModelElimination:
                            ip = new FOLModelElimination();
                            break;
                        case FolInferenceProcedure.TFMResolution:
                            ip = new FOLTFMResolution();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                 
                    StandardizeApartIndexicalFactory.flush();
                    _knowledgeBase = new FOLKnowledgeBase(Domain.Domain, ip);
                    foreach (string s in _Sentences)
                    {
                        KnowledgeBase.tell(s);
                    }
                }
                return _knowledgeBase;
            }
        }

        private List<string> _Sentences;
        public List<string> Sentences
        {
            get
            {
                if (_knowledgeBase != null)
                {
                    return _knowledgeBase.getOriginalSentences().toArray().Select(objSentence => objSentence.ToString()).ToList();
                }
                return _Sentences;
            }
            set { _Sentences = value; }
        }

    }


    public class FolKbInferInfo
    {

        public FolKbInferInfo()
        {
            KnowledgeBase = new AnonymousGeneralVariableInfo<FOLKnowledgeBase>();
            Predicate = new SimpleOrExpression<string>();
        }

        public AnonymousGeneralVariableInfo<FOLKnowledgeBase> KnowledgeBase { get; set; }

        public SimpleOrExpression<string> Predicate { get; set; }

       
        public InferenceResult Ask(object owner, IContextLookup globalVars)
        {
            var kb = KnowledgeBase.EvaluateTyped(owner, globalVars);
            var predicate = Predicate.GetValue(owner, globalVars);
            return kb.ask(predicate);
        }


        public List<string> GetProof(InferenceResult result)
        {
            var toReturn = new List<string>();
            Iterator terator = result.getProofs().iterator();
            while (terator.hasNext())
            {
                toReturn.AddRange(ProofPrinter.printProof((Proof)terator.next()).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }
            return toReturn;
        }

    }


}