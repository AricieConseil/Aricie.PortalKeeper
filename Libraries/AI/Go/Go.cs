using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileHelpers;
using GoTraxx;
using Microsoft.MSR.CNTK.Extensibility.Managed;

namespace Aricie.PortalKeeper.AI.Games.Go
{
    public class GnuGoStatus
    {
       public const SafetyFlag BlackTerritory = SafetyFlag.Black | SafetyFlag.Territory;
        public const SafetyFlag WhiteTerritory = SafetyFlag.White | SafetyFlag.Territory;

        public GnuGoStatus(GoBoard board, string gtpStatus)
        {
            var statuses = gtpStatus.Trim('=').Trim(' ').Split('=');
            CellsByStatus = new Dictionary<SafetyFlag, HashSet<int>>();
            var whiteStatusList = statuses[0];
            var whiteIndices = GetCellIndices(board, whiteStatusList);
            CellsByStatus[WhiteTerritory] = new HashSet<int>(whiteIndices);
            var blackStatusList = statuses[1];
            var blackIndices = GetCellIndices(board, blackStatusList);
            CellsByStatus[BlackTerritory] = new HashSet<int>(blackIndices);
            var aliveList = statuses[2];
            var aliveIndices = GetCellIndices(board, aliveList);
            CellsByStatus[SafetyFlag.Alive] = new HashSet<int>(aliveIndices);
            var deadList = statuses[3];
            var deadIndices = GetCellIndices(board, deadList);
            CellsByStatus[SafetyFlag.Dead] = new HashSet<int>(deadIndices);
        }

        public static List<int> GetCellIndices(GoBoard board, string statusList)
        {
            var toReturn = new List<int>();
            var matches = cellRegex.Matches(statusList);
            foreach (Match cellMatch in matches)
            {
                var idx = board.Coord.At(cellMatch.Value);
                toReturn.Add(idx);
            }
            return toReturn;
        }

        private static Regex cellRegex = new Regex("([A-Z]\\d\\d?)", RegexOptions.Compiled);


        public Dictionary<SafetyFlag, HashSet<int>> CellsByStatus { get; set; }

    }


    [DelimitedRecord(" ")]
    public class GoGameInput
    {

        public GoGameInput()
        {
            //LabelsArray = new int[] {};
            //Labels = new List<int>();
            //Inputs = new List<int>();
        }


        [FieldHidden] public int MovesNb;

        [FieldArrayLength(361, 361)] public float[] Labels;

        [FieldArrayLength(361, 361)] public float[] Inputs;


        public static GoBoard BoardFromSGF(string sgfContent)
        {
            GameRecord lGameRecord = new GameRecord();
            SGFCollection lSGFCollection = new SGFCollection();
            lSGFCollection.LoadSGFFromMemory(sgfContent);
            lSGFCollection.RetrieveGame(lGameRecord, 0);



            GoBoard lGoBoard = new GoBoard(19);

            GameRecordBoardAdapter.Apply(lGameRecord, lGoBoard, true);
            return lGoBoard;
        }

        public static GoBoard Undo(GoBoard original, int nbMoves)
        {
            var toReturn = original.Clone(true);
            for (int i = 0; i < nbMoves; i++)
            {
                toReturn.Undo();
            }
            return toReturn;
        }


        public static GoGameInput FromSGF(GoBoard endGame, GoBoard midGame, string gtpStatus)
        {

            var inputList = GetPositionFeature(midGame);
            var labelList = GetStatusFeature(endGame, gtpStatus);

            var toReturn = new GoGameInput();
            
            toReturn.Labels = labelList.ToArray();
            toReturn.Inputs = inputList.ToArray();
            return toReturn;

        }



        public static string GTPInfluence(string dataDirectory, GoBoard board)
        {

            return GTPInfluenceByFunction(dataDirectory, board, GetInfluence);
        }

        //public static string GTPStatusInfluence(GoBoard board)
        //{


        //    return GTPInfluenceByFunction("", board, GetStatusInfluence);
        //}


        public static string GTPInfluenceByFunction(string dataDirectory, GoBoard board, InfluenceFunction infFunc)
        {

            StringBuilder s = new StringBuilder(512);

            var influence = infFunc.Invoke(dataDirectory, board);

            s.Append("INFLUENCE ");
            for (int x = 0; x < board.BoardSize; x++)
                for (int y = 0; y < board.BoardSize; y++)
                    s.AppendFormat("{0} {1} ", board.At(x, y), influence[board.At(x, y)].ToString(CultureInfo.InvariantCulture));

            return s.ToString();
        }





        private static float InfluenceFactor = 50f / (2f * 361f);

        public delegate List<float> InfluenceFunction(string dataDirectory, GoBoard board);

        
        public static float GetInfluenceValue(string dataDirectory, GoBoard board, Color playerToMove)
        {

            var toReturn = GetInfluence(dataDirectory, board).Sum();
            if (playerToMove.IsWhite)
            {
                toReturn = -toReturn;
            }
            return (toReturn + 351) * InfluenceFactor;
        }




        public static List<float> GetInfluence(string dataDirectory, GoBoard board)
        {
            var feature = GetPositionFeature(board);

            var evaluation = EvaluateGoModel(dataDirectory, feature);
            evaluation = evaluation.Select(x => 2 * (x - 0.5F)).ToList();
            return evaluation;
        }

        //public static List<float> GetStatusInfluence(GoBoard board)
        //{
        //    var feature = GetStatusFeature(board);

        //    //var evaluation = EvaluateGoModel(feature);
        //    var evaluation = feature.Select(x => 2 * (x - 0.5F)).ToList();
        //    return evaluation;
        //}




        public static List<float> GetPositionFeature(GoBoard board)
        {
            var inputList = new List<float>();
            foreach (GoCell lCell in board.Cells)
            {
                var cellColor = GetCellPosition(lCell);
                inputList.Add(cellColor);
            }
            return inputList;

        }

        public static List<float> GetStatusFeature(GoBoard board, string gtpStatus)
        {
            var inputList = new List<float>();
            var objGnugoStatus = new GnuGoStatus(board, gtpStatus);
            foreach (GoCell lCell in board.Cells)
            {
                var cellColor = GetCellStatus(board, lCell, objGnugoStatus);
                inputList.Add(cellColor);
            }
          
            return inputList;

        }

       

      

        public static float GetCellPosition(GoCell lCell)
        {
            float toReturn = 0.5F;
            if (lCell.Color.IsBlack)
            {
                toReturn = 1F;
            }
            else if (lCell.Color.IsWhite)
            {
                toReturn = 0F;
            }
            return toReturn;
        }

        public static float GetCellStatus(GoBoard lGoBoard, GoCell lCell, GnuGoStatus objGnugoStatus)
        {
            float toReturn = 0.5F;
            SafetyStatus lSafetyStatus = lGoBoard.GetSafetyStatus(lCell.Index);
            if (!lSafetyStatus.IsUndecided)
            {
                if (lSafetyStatus.IsBlack) toReturn = 1;
                else if (lSafetyStatus.IsWhite) toReturn = 0;
                if (lSafetyStatus.IsDead) toReturn = 1-toReturn;
            }
            else
            {
                if (lCell.Color == Color.Black)
                {
                    //if (lGoBoard.GetBlockLibertyCount(lCell.Index) > 1)
                    if (objGnugoStatus.CellsByStatus[SafetyFlag.Dead].Contains(lCell.Index)) 
                    {
                        toReturn = 0;
                    }
                    else 
                    {
                        toReturn = 1;
                    }
                }
                else if (lCell.Color == Color.White)
                {
                    //if (lGoBoard.GetBlockLibertyCount(lCell.Index) > 1)
                    if (objGnugoStatus.CellsByStatus[SafetyFlag.Dead].Contains(lCell.Index))
                    {
                        toReturn = 1;
                    }
                    else
                    {
                        toReturn = 0;
                    }
                }
                else
                {
                    if (objGnugoStatus.CellsByStatus[GnuGoStatus.BlackTerritory].Contains(lCell.Index))
                    {
                        toReturn = 1;
                    }
                    else if (objGnugoStatus.CellsByStatus[GnuGoStatus.WhiteTerritory].Contains(lCell.Index))
                    {
                        toReturn = 0;
                    }
                    else
                    {
                        foreach (int cellIdx in ((GoEmptyBlock) (lCell.Block)).MemberList)
                        {
                            if (objGnugoStatus.CellsByStatus[GnuGoStatus.BlackTerritory].Contains(cellIdx))
                            {
                                toReturn = 1;
                                break;
                            }
                            else if (objGnugoStatus.CellsByStatus[GnuGoStatus.WhiteTerritory].Contains(cellIdx))
                            {
                                toReturn = 0;
                                break;
                            }
                        }
                    }
                }
            }

            return toReturn;
        }


        private static IEvaluateModelManagedF model;

        private static List<float> EvaluateGoModel(string dataDirectory, List<float> feature)
        {
            try
            {
                // The examples assume the executable is running from the data folder
                // We switch the current directory to the data folder (assuming the executable is in the <CNTK>/x64/Debug|Release folder
                //Path.Combine(initialDirectory, @"..\..\Examples\Image\MNIST\Data\");

                //Dictionary<string, List<float>> outputs;

                //using (var model = new IEvaluateModelManagedF())
                //{


                if (model == null)
                {
                    Environment.CurrentDirectory = dataDirectory; // @"D:\Aricie\Go\cntk\Data";
                    model = new IEvaluateModelManagedF();
                    // Initialize model evaluator
                    string config = GetFileContents(Path.Combine(Environment.CurrentDirectory, @"..\Config\02_Convolution.cntk"));
                    model.Init(config);

                    // Load model
                    string modelFilePath = Path.Combine(Environment.CurrentDirectory, @"..\Output\Models\02_Convolution");


                    //model.LoadModel(modelFilePath);
                   model.CreateNetwork(string.Format("deviceId=0\nmodelPath=\"{0}\"", modelFilePath));

                    // Generate random input values in the appropriate structure and size
                    //var inputs = GetDictionary("features", 28 * 28, 255);


                    // We can preallocate the output structure and pass it in (multiple output layers)
                    //outputs = GetDictionary("ol.z", 10, 1);

                }
                var inputs = new Dictionary<string, List<float>>();
                inputs.Add("features", feature);
                return model.Evaluate(inputs, "ol.act", 361);
                //}

                //OutputResults(outputs);
            }
            //catch (CNTKException ex)
            //{
            //    Console.WriteLine("Error: {0}\nNative CallStack: {1}\n Inner Exception: {2}", ex.Message, ex.NativeCallStack, ex.InnerException != null ? ex.InnerException.Message : "No Inner Exception");
            //}
            catch (Exception ex)
            {
                Console.WriteLine(@"Error: {0} CallStack: {1} Inner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException?.Message ?? "No Inner Exception");
            }
            return null;
        }


        /// <summary>
        /// Reads the configuration file and returns the contents as a string
        /// </summary>
        /// <returns>The content of the configuration file</returns>
        static string GetFileContents(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            return string.Join("\n", lines);
        }








       


    }
}