using System;
using System.Collections.Generic;
using System.Text;
using UnrealEngine.Runtime;

namespace HelloUSharp
{
    [UEnum]
    public enum EBowlAction : System.Byte
    {
        Tidy = 0,
        Reset = 1,
        EndTurn = 2,
        EndGame = 3,
        Undefined = 4
    };

    [UClassIgnore]
    public static class BowlActionMaster
    {
        public static EBowlAction NextAction(List<int> rolls)
        {
            EBowlAction nextAction = EBowlAction.Undefined;

            for (int i = 0; i < rolls.Count; i++)
            { // Step through rolls

                if (i == 20)
                {
                    nextAction = EBowlAction.EndGame;
                }
                else if (i >= 18 && rolls[i] == 10)
                { // Handle last-frame special cases
                    nextAction = EBowlAction.Reset;
                }
                else if (i == 19)
                {
                    if (rolls[18] == 10 && rolls[19] == 0)
                    {
                        nextAction = EBowlAction.Tidy;
                    }
                    else if (rolls[18] + rolls[19] == 10)
                    {
                        nextAction = EBowlAction.Reset;
                    }
                    else if (rolls[18] + rolls[19] >= 10)
                    {  // Roll 21 awarded
                        nextAction = EBowlAction.Tidy;
                    }
                    else
                    {
                        nextAction = EBowlAction.EndGame;
                    }
                }
                else if (i % 2 == 0)
                { // First bowl of frame
                    if (rolls[i] == 10)
                    {
                        //rolls.Insert(i, 0); // Insert virtual 0 after strike
                        nextAction = EBowlAction.EndTurn;
                    }
                    else
                    {
                        nextAction = EBowlAction.Tidy;
                    }
                }
                else
                { // Second bowl of frame
                    nextAction = EBowlAction.EndTurn;
                }
            }

            return nextAction;
        }
    }
}
