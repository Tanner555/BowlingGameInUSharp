using System;
using System.Collections.Generic;
using System.Text;

namespace HelloUSharp
{
    public enum BowlAction { Tidy, Reset, EndTurn, EndGame, Undefined };

    public static class BowlActionMaster
    {
        public static BowlAction NextAction(List<int> rolls)
        {
            BowlAction nextAction = BowlAction.Undefined;

            for (int i = 0; i < rolls.Count; i++)
            { // Step through rolls

                if (i == 20)
                {
                    nextAction = BowlAction.EndGame;
                }
                else if (i >= 18 && rolls[i] == 10)
                { // Handle last-frame special cases
                    nextAction = BowlAction.Reset;
                }
                else if (i == 19)
                {
                    if (rolls[18] == 10 && rolls[19] == 0)
                    {
                        nextAction = BowlAction.Tidy;
                    }
                    else if (rolls[18] + rolls[19] == 10)
                    {
                        nextAction = BowlAction.Reset;
                    }
                    else if (rolls[18] + rolls[19] >= 10)
                    {  // Roll 21 awarded
                        nextAction = BowlAction.Tidy;
                    }
                    else
                    {
                        nextAction = BowlAction.EndGame;
                    }
                }
                else if (i % 2 == 0)
                { // First bowl of frame
                    if (rolls[i] == 10)
                    {
                        rolls.Insert(i, 0); // Insert virtual 0 after strike
                        nextAction = BowlAction.EndTurn;
                    }
                    else
                    {
                        nextAction = BowlAction.Tidy;
                    }
                }
                else
                { // Second bowl of frame
                    nextAction = BowlAction.EndTurn;
                }
            }

            return nextAction;
        }
    }
}
