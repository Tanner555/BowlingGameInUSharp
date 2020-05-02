using System;
using System.Collections.Generic;
using System.Text;
using UnrealEngine.Runtime;

namespace USharpBowlProject
{
    [UEnum]
    public enum EBowlActionCSharp : System.Byte
    {
        Tidy = 0,
        Reset = 1,
        EndTurn = 2,
        EndGame = 3,
        Undefined = 4
    };

    [UEnum]
    public enum EBowlFrameCSharp : System.Byte
    {
        Frame01 = 0,
        Frame02 = 1,
        Frame03 = 2,
        Frame04 = 3,
        Frame05 = 4,
        Frame06 = 5,
        Frame07 = 6,
        Frame08 = 7,
        Frame09 = 8,
        Frame10 = 9
    }

    [UClassIgnore]
    public static class BowlActionMaster
    {
        public static EBowlActionCSharp NextAction(List<int> rolls)
        {
            EBowlActionCSharp nextAction = EBowlActionCSharp.Undefined;

            for (int i = 0; i < rolls.Count; i++)
            { // Step through rolls

                if (i == 20)
                {
                    nextAction = EBowlActionCSharp.EndGame;
                }
                else if (i >= 18 && rolls[i] == 10)
                { // Handle last-frame special cases
                    nextAction = EBowlActionCSharp.Reset;
                }
                else if (i == 19)
                {
                    if (rolls[18] == 10 && rolls[19] == 0)
                    {
                        nextAction = EBowlActionCSharp.Tidy;
                    }
                    else if (rolls[18] + rolls[19] == 10)
                    {
                        nextAction = EBowlActionCSharp.Reset;
                    }
                    else if (rolls[18] + rolls[19] >= 10)
                    {  // Roll 21 awarded
                        nextAction = EBowlActionCSharp.Tidy;
                    }
                    else
                    {
                        nextAction = EBowlActionCSharp.EndGame;
                    }
                }
                else if (i % 2 == 0)
                { // First bowl of frame
                    if (rolls[i] == 10)
                    {
                        //rolls.Insert(i, 0); // Insert virtual 0 after strike
                        nextAction = EBowlActionCSharp.EndTurn;
                    }
                    else
                    {
                        nextAction = EBowlActionCSharp.Tidy;
                    }
                }
                else
                { // Second bowl of frame
                    nextAction = EBowlActionCSharp.EndTurn;
                }
            }

            return nextAction;
        }
    }
}
