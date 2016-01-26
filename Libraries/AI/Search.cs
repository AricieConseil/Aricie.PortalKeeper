using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aima.core.search.framework;
using aima.core.search.informed;
using aima.core.search.uninformed;
using Aricie.DNN.Services;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.Entities;
using Aricie.DNN.ComponentModel;
using System.Collections;
//using aima.core.search.eightpuzzle;
using aima.core.search.local;
//using aima.core.search.nqueens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Aricie.DNN.UI.WebControls;
using Aricie.Security.Cryptography;
using java.util;

namespace Aricie.PortalKeeper.AI.Search
{

    public enum QueueSearchType
    {
        GraphSearch,
        TreeSearch
    }

    public enum SearchAlgorithmType
    {
        KnownInformed,
        KnownUninformed,
        CustomAlgorithm
    }

    public enum KnownInformedSearch
    {
        AStar,
        GreedyBestFirst,
        HillClimbing,
        SimulatedAnnealing
    }

    public enum KnownUninformedSearch
    {
        BreadthFirst,
        DepthFirst,
        DepthLimited,
        IterativeDeepening,
        UniformCost,
        Bidirectional
    }

    public class SearchInfo
    {
        private AnonymousGeneralVariableInfo<aima.core.search.framework.Search> _customSearchAlgorithm;

        public SearchInfo()
        {
            AlgorithmType = SearchAlgorithmType.KnownInformed;
            QueueSearchType = QueueSearchType.GraphSearch;
            _customSearchAlgorithm = new AnonymousGeneralVariableInfo<aima.core.search.framework.Search>();
            DepthLimit = 5;
        }

        [JsonProperty(DefaultValueHandling =  DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchAlgorithmType AlgorithmType { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        [ConditionalVisible("AlgorithmType", false, true, SearchAlgorithmType.KnownInformed)]
        public KnownInformedSearch KnownInformedAlgorithm { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        [ConditionalVisible("AlgorithmType", false, true, SearchAlgorithmType.KnownUninformed)]
        public KnownUninformedSearch KnownUninformedAlgorithm { get; set; }

        [ConditionalVisible("AlgorithmType", false, true, SearchAlgorithmType.KnownUninformed)]
        [ConditionalVisible("KnownUninformedAlgorithm", false, true, KnownUninformedSearch.DepthLimited)]
        public int DepthLimit { get; set; }

        [ConditionalVisible("AlgorithmType", false, true, SearchAlgorithmType.CustomAlgorithm)]
        public AnonymousGeneralVariableInfo<aima.core.search.framework.Search> CustomSearchAlgorithm
        {
            get
            {
                if (AlgorithmType !=  SearchAlgorithmType.CustomAlgorithm)
                {
                    return null;
                }
                return _customSearchAlgorithm;
            }
            set { _customSearchAlgorithm = value; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public QueueSearchType QueueSearchType { get; set; }

        public aima.core.search.framework.Search GetSearch(Object owner, IContextLookup globalVars, HeuristicFunction objHeuristicFunction)
        {
            aima.core.search.framework.Search toReturn;
            QueueSearch objQueueSearch;
            switch (QueueSearchType)
            {
                case QueueSearchType.TreeSearch:
                    objQueueSearch = new TreeSearch();
                    break;
                default:
                    objQueueSearch = new GraphSearch();
                    break;
            }
            switch (AlgorithmType)
            {
                case SearchAlgorithmType.KnownInformed:
                    switch (KnownInformedAlgorithm)
                    {
                        case KnownInformedSearch.GreedyBestFirst:
                            toReturn = new GreedyBestFirstSearch(objQueueSearch, objHeuristicFunction);
                            break;
                        case KnownInformedSearch.HillClimbing:
                            toReturn = new HillClimbingSearch(objHeuristicFunction);
                            break;
                        case KnownInformedSearch.SimulatedAnnealing:
                            toReturn = new SimulatedAnnealingSearch(objHeuristicFunction);
                            break;
                        default:
                            toReturn = new AStarSearch(objQueueSearch, objHeuristicFunction);
                            break;
                    }
                    break;
                case SearchAlgorithmType.KnownUninformed:
                    switch (KnownUninformedAlgorithm)
                    {

                        case KnownUninformedSearch.DepthFirst:
                            toReturn = new DepthFirstSearch(objQueueSearch);
                            break;
                        case KnownUninformedSearch.DepthLimited:
                            toReturn = new DepthLimitedSearch(DepthLimit);
                            break;
                        case KnownUninformedSearch.IterativeDeepening:
                            toReturn = new IterativeDeepeningSearch();
                            break;
                        case KnownUninformedSearch.UniformCost:
                            toReturn = new UniformCostSearch(objQueueSearch);
                            break;
                        case KnownUninformedSearch.Bidirectional:
                            toReturn = new BidirectionalSearch();
                            break;
                        default:
                            toReturn = new BreadthFirstSearch(objQueueSearch);
                            break;
                    }
                    break;
                default:
                    toReturn = CustomSearchAlgorithm.EvaluateTyped(owner, globalVars);
                    break;
            }
            return toReturn;
        }
    }


    public class ProblemInfo
    {

        public ProblemInfo()
        {

            ActionsFunction = new AnonymousGeneralVariableInfo<ActionsFunction>();
            ResultFunction = new AnonymousGeneralVariableInfo<ResultFunction>();
            GoalTest = new AnonymousGeneralVariableInfo<GoalTest>();
            StepCostFunction = new EnabledFeature<AnonymousGeneralVariableInfo<StepCostFunction>>();
            

        }

        public AnonymousGeneralVariableInfo<ActionsFunction> ActionsFunction { get; set; }

        public AnonymousGeneralVariableInfo<ResultFunction> ResultFunction { get; set; }

        public AnonymousGeneralVariableInfo<GoalTest> GoalTest { get; set; }

        public EnabledFeature<AnonymousGeneralVariableInfo<StepCostFunction>> StepCostFunction { get; set; }

        

        public Problem GetProblem(Object owner, IContextLookup globalVars, Object initialState)
        {
            Problem toReturn;
            var objActionsFunction = ActionsFunction.EvaluateTyped(owner, globalVars);
            var objResultFunction = ResultFunction.EvaluateTyped(owner, globalVars);
            var objGoalTest = GoalTest.EvaluateTyped(owner, globalVars);
            if (StepCostFunction.Enabled)
            {
                var objStepCostFunction = StepCostFunction.Entity.EvaluateTyped(owner, globalVars);
                toReturn = new Problem(initialState, objActionsFunction, objResultFunction, objGoalTest, objStepCostFunction);
            }
            else
            {
                toReturn = new Problem(initialState, objActionsFunction, objResultFunction, objGoalTest);
            }
            return toReturn;
        }
    }


    public class SearchAgentResult
    {
        public SearchAgent SearchAgent { get; set; }

        public Problem Problem { get; set; }
        
    }

    public class SearchAgentInfo
    {

        public SearchAgentInfo()
        {
            InitialState = new AnonymousGeneralVariableInfo();
            Problem = new SimpleOrExpression<ProblemInfo>();
            HeuristicFunction = new EnabledFeature<AnonymousGeneralVariableInfo<HeuristicFunction>>();
            Search = new SimpleOrExpression<SearchInfo>();
        }

        [ExtendedCategory("InitialState")]
        public AnonymousGeneralVariableInfo InitialState { get; set; }

        [ExtendedCategory("Problem")]
        public SimpleOrExpression<ProblemInfo> Problem { get; set; }

        [ExtendedCategory("Search")]
        public SimpleOrExpression<SearchInfo> Search { get; set; }

        [ExtendedCategory("Search")]
        public EnabledFeature<AnonymousGeneralVariableInfo<HeuristicFunction>> HeuristicFunction { get; set; }

        public SearchAgentResult PerformSearch(object owner, IContextLookup globalVars)
        {
            var objInitialState = InitialState.Evaluate(owner, globalVars);
            var objProblemInfo = Problem.GetValue(owner, globalVars);
            var objSearchInfo = Search.GetValue(owner, globalVars);
            Problem objProblem = objProblemInfo.GetProblem(owner, globalVars, objInitialState);
            var objHeuristicFunction = HeuristicFunction.Entity.EvaluateTyped(owner, globalVars);
            if (objHeuristicFunction==null)
            {
                objHeuristicFunction = DefaultHeuristics;
            }
            var objSearch = objSearchInfo.GetSearch(owner, globalVars, objHeuristicFunction);
            return new SearchAgentResult() { Problem = objProblem, SearchAgent = new SearchAgent(objProblem, objSearch)};
        }

        private static readonly NonHeuristics DefaultHeuristics = new NonHeuristics();

        private class NonHeuristics : HeuristicFunction
        {
            double HeuristicFunction.h(object obj)
            {
                return 1;
            }
        }

        public List<string> PrintActions(SearchAgent agent)
        {
            var toReturn = new List<String>();
            var actions =  agent.getActions().toArray();
            foreach (aima.core.agent.Action action in actions)
            {
                toReturn.Add(action.ToString());
            }
            return toReturn;
        }

        public Dictionary<string, string> PrintInstrumentation(SearchAgent agent)
        {
            var toReturn = new Dictionary<String, String>();
            var instrumentation = agent.getInstrumentation();
            Iterator terator = instrumentation.keySet().iterator();
            while (terator.hasNext())
            {
                string key = (string)terator.next();
                string property = instrumentation.getProperty(key);
                toReturn[key] = property;
            }
            
            return toReturn;
        }

        public List<object> GetStateSequence(SearchAgentResult result)
        {
            var toReturn = new List<object>();
            var obResultFunction = result.Problem.getResultFunction();
            
            var currentState = result.Problem.getInitialState();
            toReturn.Add(currentState);
            
            while (!result.SearchAgent.isDone())
            {
                var action = result.SearchAgent.execute(null);
                currentState = obResultFunction.result(currentState, action);
                toReturn.Add(currentState);
            }
            return toReturn;
        }

    }
}

