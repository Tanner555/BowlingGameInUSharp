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
        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int Value12345 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public TSubclassOf<AActor> BowlingBallSubClassReference { get; set; }
        #endregion

        #region Fields
        protected APlayerCameraManager myCameraManager = null;
        protected BowlingBall myBall = null;

        private FVector2D dragStart, dragEnd;
        private float startTime, endTime;
        #endregion

        #region Overrides
        protected override void ReceiveBeginPlay_Implementation()
        {
            //myCameraManager = UGameplayStatics.GetPlayerCameraManager(this, 0);
            //if (myCameraManager != null)
            //{
            //    PrintString("Camera Manager is " + myCameraManager.GetName(), FLinearColor.AliceBlue, printToLog: true);
            //}
            //else
            //{
            //    PrintString("Couldn't Find Camera Manager", FLinearColor.AliceBlue);
            //}
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //FHitResult _hit;
            ////base.ReceiveTick_Implementation(DeltaSeconds);
            //if (myCameraManager != null)
            //{
            //    //myCameraManager.
            //    myCameraManager.SetActorLocation(
            //        myCameraManager.GetActorLocation() + new FVector(2, 0, 0),
            //        false, out _hit, false
            //    );

            //}
        }
        #endregion

        #region Setters
        [UFunction, BlueprintCallable]
        public void SetBallFromBallFindCollection(List<BowlingBall> balls)
        {
            if (balls != null && balls.Count > 0 && balls[0] != null)
            {
                //PrintString("Setting my ball to: " + balls[0].GetName(), FLinearColor.Green);
                myBall = balls[0];
            }
        }
        #endregion

        [UFunction, BlueprintCallable]
        public void OnStartDrag(FVector2D mousePos)
        {
            dragStart = mousePos;
            startTime = World.GetGameTimeInSeconds();
        }

        [UFunction, BlueprintCallable]
        public void OnStopDrag(FVector2D mousePos)
        {
            dragEnd = mousePos;
            endTime = World.GetGameTimeInSeconds();

            float dragDuration = endTime - startTime;

            //Horizontal
            float launchSpeedY = (dragEnd.X - dragStart.X) / dragDuration;
            //Forward
            float launchSpeedX = (dragStart.Y - dragEnd.Y) / dragDuration;

            FVector _launchVelocity = new FVector(launchSpeedX, launchSpeedY, 0);
            StartLaunchingTheBall(_launchVelocity);
        }

        [UFunction, BlueprintCallable]
        public void StartLaunchingTheBall(FVector launchVelocity)
        {
            var _bowlPlayer = (MyBowlPlayer)World.GetPlayerPawn(0);
            if (myBall != null && _bowlPlayer != null)
            {
                PrintString("Launching Ball", FLinearColor.AliceBlue, printToLog:true);
                myBall.LaunchBall(launchVelocity);
                _bowlPlayer.StartFollowingBall(myBall);
            }
        }

        #region UnusedCode
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
        #endregion
    }
}
