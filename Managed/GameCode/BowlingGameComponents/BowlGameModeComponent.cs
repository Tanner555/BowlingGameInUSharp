﻿using System;
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
    public class BowlGameModeComponent : UActorComponent
    {
        #region IgnoreProperties
        [UPropertyIngore]
        public AActor MyOwner
        {
            get
            {
                if (_owner == null)
                    _owner = GetOwner();

                return _owner;
            }
        }
        private AActor _owner = null;

        [UPropertyIngore]
        public FName BallTag => new FName("Ball");
        [UPropertyIngore]
        public FName PinTag => new FName("Pin");

        [UPropertyIngore]
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        #endregion

        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public float MinimalForwardLaunchVelocity { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public TSubclassOf<AActor> BowlingBallSubClassReference { get; set; }
        #endregion

        #region Fields
        protected APlayerCameraManager myCameraManager = null;
        protected BowlingBallComponent myBall = null;
        protected MyBowlPlayerComponent myBowler = null;

        private FVector2D dragStart, dragEnd;
        private float startTime, endTime;

        protected static BowlGameModeComponent ThisInstance = null;
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            MinimalForwardLaunchVelocity = 1500;
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            MyOwner.World.GetPlayerController(0).ShowMouseCursor = true;

            List<AActor> ballActors;
            MyOwner.World.GetAllActorsWithTag(BallTag, out ballActors);
            SetBallFromBallFindCollection(ballActors);

            myBowler = MyOwner.World.GetPlayerPawn(0).GetComponentByClass<MyBowlPlayerComponent>();
            //SetBallFromBallFindCollection(MyOwner.World.GetAllActorsOfClassList<BowlingBallComponent>());
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {

        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            //Set ThisInstance To Null, Otherwise Value Doesn't Get Destroyed and Will Crash Engine.
            ThisInstance = null;
        }
        #endregion

        #region Getter
        public static BowlGameModeComponent GetInstance(UObject worldContextObject)
        {
            if(ThisInstance == null)
            {
                ThisInstance = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<BowlGameModeComponent>();
            }
            return ThisInstance;
        }
        #endregion

        #region Setters
        [UFunction, BlueprintCallable]
        public void SetBallFromBallFindCollection(List<AActor> balls)
        {
            if (balls != null && balls.Count > 0 && balls[0] != null)
            {
                var _ballComp = balls[0].GetComponentByClass<BowlingBallComponent>();
                if (_ballComp != null)
                {
                    myBall = _ballComp;
                }
            }
        }
        #endregion

        #region DraggingAndBallLaunch
        [UFunction, BlueprintCallable]
        public void OnStartDrag(FVector2D mousePos)
        {
            dragStart = mousePos;
            startTime = MyOwner.World.GetGameTimeInSeconds();
        }

        [UFunction, BlueprintCallable]
        public void OnStopDrag(FVector2D mousePos)
        {
            dragEnd = mousePos;
            endTime = MyOwner.World.GetGameTimeInSeconds();

            float dragDuration = endTime - startTime;

            //Horizontal
            float launchSpeedY = (dragEnd.X - dragStart.X) / dragDuration;
            //Forward
            float launchSpeedX = (dragStart.Y - dragEnd.Y) / dragDuration;

            FVector _launchVelocity = new FVector(launchSpeedX, launchSpeedY, 0);
            if (_launchVelocity.X > MinimalForwardLaunchVelocity)
            {
                StartLaunchingTheBall(_launchVelocity);
            }
            else
            {
                MyOwner.PrintString("Not Enough Force To Launch!", FLinearColor.Green);
            }
        }

        [UFunction, BlueprintCallable]
        public void StartLaunchingTheBall(FVector launchVelocity)
        {
            if (myBall != null && gamemaster.bCanLaunchBall)
            {
                gamemaster.CallOnBallLaunch(launchVelocity, myBall);
            }
        }
        #endregion

        #region NudgeBall
        [UFunction, BlueprintCallable]
        public void NudgeBallLeft()
        {
            gamemaster.CallOnNudgeBallLeft(-50);
        }

        [UFunction, BlueprintCallable]
        public void NudgeBallRight()
        {
            gamemaster.CallOnNudgeBallRight(50);
        }
        #endregion
    }
}
