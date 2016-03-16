using System.Collections.Generic;
using FileHelpers;
using GoTraxx;

namespace Aricie.PortalKeeper.AI.Games.Go
{
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

        [FieldArrayLength(361, 361)] public int[] Labels;

        [FieldArrayLength(361, 361)] public int[] Inputs;


        //public List<int> Labels { get; set; }

        //public List<int> Inputs { get; set; }


        public static GoGameInput FromSGF(string fileContent)
        {
            
            
            GameRecord lGameRecord = new GameRecord();
            SGFCollection lSGFCollection = new SGFCollection();
            lSGFCollection.LoadSGFFromMemory(fileContent);
            lSGFCollection.RetrieveGame(lGameRecord, 0);

            

            GoBoard lGoBoard = new GoBoard(19);
            
            GameRecordBoardAdapter.Apply(lGameRecord, lGoBoard, true);
            var middleGameTurnIdx =  lGoBoard.MoveNbr / 2;
            var middleGameBoard = new GoBoard(19);
            GameRecordBoardAdapter.Apply(lGameRecord, middleGameBoard, true, middleGameTurnIdx);

            var inputList = new List<int>();
            var labelList = new List<int>();
            foreach (GoCell lCell in lGoBoard.Cells)
            {
                
                var cellStatus = GetCellStatus(lGoBoard, lCell);
                var midGameCell = middleGameBoard.Cells[lCell.Index];
                var midGamePosition = GetCellPosition(midGameCell);
                inputList.Add(midGamePosition);
                labelList.Add(cellStatus);
            }

            var toReturn = new GoGameInput();
            toReturn.MovesNb = lGameRecord.Moves.Count;
            toReturn.Labels = labelList.ToArray();
            toReturn.Inputs = inputList.ToArray();
            return toReturn;
        }

        public static int GetCellPosition(GoCell lCell)
        {
            int toReturn = 0;
            if (lCell.Color.IsBlack)
            {
                toReturn = 1;
            }
            else if (lCell.Color.IsWhite)
            {
                toReturn = -1;
            }
            return toReturn;
        }

        public static int GetCellStatus(GoBoard lGoBoard, GoCell lCell)
        {
            int toReturn = 0;
            SafetyStatus lSafetyStatus = lGoBoard.GetSafetyStatus(lCell.Index);
            if (!lSafetyStatus.IsUndecided)
            {
                if (lSafetyStatus.IsBlack) toReturn = 1;
                else if (lSafetyStatus.IsWhite) toReturn = -1;
                if (lSafetyStatus.IsDead) toReturn = - toReturn;
            }
            else
            {
                if (lCell.Color == Color.Black)
                {
                    if (lGoBoard.GetBlockLibertyCount(lCell.Index)>1)
                    {
                        toReturn = 1;
                    }
                }
                else if (lCell.Color == Color.White)
                {
                    if (lGoBoard.GetBlockLibertyCount(lCell.Index) > 1)
                    {
                        toReturn = -1;
                    }
                }
                else
                {
                    var emptyBlock = (GoEmptyBlock) lCell.Block;
                    int blacks=0;
                    int whites =0;
                    foreach (var adjacentBlock in emptyBlock.AdjacentBlocks.StoneBlocks)
                    {
                        if (!lGoBoard.GetSafetyStatus(adjacentBlock.MemberList[0]).IsDead)
                        {
                            if (adjacentBlock.BlockColor == Color.Black)
                            {
                                blacks += adjacentBlock.StoneCount;
                            }
                            else
                            {
                                whites += adjacentBlock.StoneCount;
                            }
                        }
                    }
                    if (whites < 4 * blacks)
                    {
                        toReturn = 1;
                    }
                    else if (blacks < 4 * whites)
                    {
                        toReturn = -1;
                    }

                }
            }
            
            return toReturn;
        }


    }
}