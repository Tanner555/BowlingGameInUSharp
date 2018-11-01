using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using UnrealEngine.Slate;
using UnrealEngine;
using UnrealEngine.GameplayTasks;
using UnrealEngine.SlateCore;
using UnrealEngine.NavigationSystem;

namespace HelloUSharp
{
    [UClass, Blueprintable, BlueprintType]
    class BowlGameMode : AGameMode
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int Value12345 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public FVector CameraFollowOffset { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public TSubclassOf<AActor> BowlingBallSubClassReference { get; set; }

        protected APlayerCameraManager myCameraManager = null;
        protected BowlingBall myBall = null;

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //PrintString("Hello From " + GetName(), FLinearColor.Green);
            myCameraManager = UGameplayStatics.GetPlayerCameraManager(this, 0);
            if(myCameraManager != null)
            {
                //PrintString("Camera Manager is " + myCameraManager.GetName(), FLinearColor.AliceBlue, printToLog:true);
            }
            else
            {
                PrintString("Couldn't Find Camera Manager", FLinearColor.AliceBlue);
            }
        }

        [UFunction, BlueprintCallable]
        public void SetBallFromBallFindCollection(List<BowlingBall> balls)
        {
            if(balls != null && balls.Count > 0 && balls[0] != null)
            {
                //PrintString("Setting my ball to: " + balls[0].GetName(), FLinearColor.Green);
                myBall = balls[0];
            }
        }

        /// <summary>
        /// TSubClassOf Class Is Always Null, If you try to set a uproperty, it's still null.
        /// Using This Won't Work: if (BowlingBallSubClassReference != null)
        /// but this will: if (BowlingBallSubClassReference.Value != null)
        /// </summary>
        private void BrokenGetBallAttempt()
        {
            if (BowlingBallSubClassReference != null)
            {
                PrintString("Class: " + BowlingBallSubClassReference.Value.GetName(), FLinearColor.Green, printToLog: true);
                List<AActor> _balls = new List<AActor>();

                World.GetAllActorsOfClass(BowlingBallSubClassReference, out _balls);

                if (_balls != null && _balls.Count > 0)
                {
                    PrintString("Found some balls", FLinearColor.Green, printToLog: true);
                }
                else
                {
                    PrintString("Couldn't find any balls", FLinearColor.Green, printToLog: true);
                }
            }
        }
    }
}
