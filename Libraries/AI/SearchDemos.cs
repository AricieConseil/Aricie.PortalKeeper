using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aima.core.agent.impl;
using aima.core.environment.eightpuzzle;
using aima.core.environment.nqueens;
using aima.core.search.framework;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls;
using Aricie.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Aricie.PortalKeeper.AI.Search.Demo
{
    public enum SearchDemos
    {
        EightPuzzle,
        NQueen,
    }

    public enum EightPuzzleHeuristics
    {
        None,
        Manhattan,
        MisplacedTile,
    }

    public enum NQueensActions
    {
        Incremental,
        CompleteState
    }

    public enum NQueensHeuristics
    {
        None,
        QueensToBePlaced
    }



    public class SearchDemoInfo
    {

        public SearchDemoInfo()
        {
            NQueenBoardSize = 8;
            EightPuzzleInitialState = new List<int>();
            EightPuzzleShuffleMoves = 3;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public SearchDemos Selection { get; set; }

        [ConditionalVisible("Selection", false, true, SearchDemos.NQueen)]
        public int NQueenBoardSize { get; set; }

        public bool ShouldSerializeNQueenBoardSize()
        {
            return Selection == SearchDemos.NQueen;
        }

        [ConditionalVisible("Selection", false, true, SearchDemos.NQueen)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NQueensActions NQueensActions { get; set; }

        public bool ShouldSerializeNQueenActions()
        {
            return Selection == SearchDemos.NQueen;
        }

        [ConditionalVisible("Selection", false, true, SearchDemos.EightPuzzle)]
        [CollectionEditor(DisplayStyle = CollectionDisplayStyle.Accordion, EnableExport = false, ItemsReadOnly = true,
            NoAdd = true, NoDeletion = true, Ordered = true)]
        public List<int> EightPuzzleInitialState { get; set; }

        public bool ShouldSerializeEightPuzleInitialState()
        {
            return Selection == SearchDemos.EightPuzzle;
        }


        [ConditionalVisible("Selection", false, true, SearchDemos.EightPuzzle)]
        public int EightPuzzleShuffleMoves { get; set; }

        [ConditionalVisible("Selection", false, true, SearchDemos.EightPuzzle)]
        [ActionButton(IconName.Refresh, IconOptions.Normal)]
        public void InitEigthPuzzleInitialState(AriciePropertyEditorControl ape)
        {
            EightPuzzleInitialState = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.ToList();
            ape.ItemChanged = true;
            //string message = Localization.GetString("ParametersCreated.Message", ape.LocalResourceFile);
            //ape.DisplayMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess);
        }

        [ConditionalVisible("Selection", false, true, SearchDemos.EightPuzzle)]
        [ActionButton(IconName.Magic, IconOptions.Normal)]
        public void ShuffleEigthPuzzleInitialState(AriciePropertyEditorControl ape)
        {

            var eightPuzzleBoard = new EightPuzzleBoard(EightPuzzleInitialState.ToArray());

            var actionsFunction = EightPuzzleFunctionFactory.getActionsFunction();
            var resultFunction = EightPuzzleFunctionFactory.getResultFunction();
            var previousBoards = new HashSet<EightPuzzleBoard>();

            for (int i = 0; i < EightPuzzleShuffleMoves; i++)
            {
                Object[] successors = actionsFunction.actions(eightPuzzleBoard).toArray();

                EightPuzzleBoard nextState;
                do
                {
                    var choosenSuccessorIdx = CryptoHelper.Random.Next(successors.Length);
                    var objAction = (aima.core.agent.Action)successors[choosenSuccessorIdx];
                     nextState = (EightPuzzleBoard)resultFunction.result(eightPuzzleBoard, objAction);
                } while (previousBoards.Contains(nextState));

                previousBoards.Add(eightPuzzleBoard);
                eightPuzzleBoard = (EightPuzzleBoard) nextState;
            }
            EightPuzzleInitialState = new List<int>(eightPuzzleBoard.getState());
            ape.ItemChanged = true;

        }


        [ConditionalVisible("Selection", false, true, SearchDemos.EightPuzzle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public EightPuzzleHeuristics EightPuzzleHeuristics { get; set; }

        public bool ShouldSerializeEightPuzzleHeuristics()
        {
            return Selection == SearchDemos.EightPuzzle;
        }

        [ConditionalVisible("Selection", false, true, SearchDemos.NQueen)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NQueensHeuristics NQueensHeuristics { get; set; }

        public bool ShouldSerializeNQueensHeuristics()
        {
            return Selection == SearchDemos.NQueen;
        }

        public Object GetInitialState()
        {
            switch (Selection)
            {
                case SearchDemos.EightPuzzle:
                    return new EightPuzzleBoard(EightPuzzleInitialState.ToArray());
                case SearchDemos.NQueen:
                    return new NQueensBoard(NQueenBoardSize);
                default:
                    break;
            }
            return null;
        }

        public ActionsFunction GetActionsFunction()
        {
            switch (Selection)
            {
                case SearchDemos.EightPuzzle:
                    return EightPuzzleFunctionFactory.getActionsFunction();
                case SearchDemos.NQueen:
                    switch (NQueensActions)
                    {
                        case NQueensActions.CompleteState:
                            return NQueensFunctionFactory.getIActionsFunction();
                        case NQueensActions.Incremental:
                            return NQueensFunctionFactory.getIActionsFunction();
                    }
                    break;
                default:
                    return null;
            }
            return null;
        }

        public ResultFunction GetResultFunction()
        {
            switch (Selection)
            {
                case SearchDemos.EightPuzzle:
                    return EightPuzzleFunctionFactory.getResultFunction();
                case SearchDemos.NQueen:
                    return NQueensFunctionFactory.getResultFunction();
                default:
                    break;
            }
            return null;
        }

        public GoalTest GetGoalTestFunction()
        {
            switch (Selection)
            {
                case SearchDemos.EightPuzzle:
                    return new EightPuzzleGoalTest();
                case SearchDemos.NQueen:
                    return new NQueensGoalTest();
                default:
                    break;
            }
            return null;
        }

        public HeuristicFunction GetHeuristicFunction()
        {
            switch (Selection)
            {
                case SearchDemos.EightPuzzle:
                    switch (EightPuzzleHeuristics)
                    {
                        case EightPuzzleHeuristics.Manhattan:
                            return new ManhattanHeuristicFunction();
                        case EightPuzzleHeuristics.MisplacedTile:
                            return new MisplacedTilleHeuristicFunction();
                        default:
                            return null;
                    }
                case SearchDemos.NQueen:
                    switch (NQueensHeuristics)
                    {
                        case NQueensHeuristics.QueensToBePlaced:
                            return new AttackingPairsHeuristic();
                        default:
                            return null;
                    }
                default:
                    break;
            }
            return null;
        }


    }

}