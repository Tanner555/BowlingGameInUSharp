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
    public class MyBowlPlayerComponent : UActorComponent
    {
        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public string MyString12345 { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public float BallFollowLimitDistance { get; set; }
        [UProperty, BlueprintReadOnly, Category("Bowling")]
        public float DefaultBallFollowOffset { get; set; }
        #endregion

        #region IgnoreProperties
        [UPropertyIngore]
        protected float PawnStartXPoint { get; set; }
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
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region Fields
        protected bool bShouldFollowBall = false;
        private FHitResult myHit;
        private BowlingBallComponent myBall = null;
        #endregion

        #region Testing
        [UFunction, BlueprintCallable]
        public string GetMyMessage()
        {
            return "Hello There From MyPawnClass123455";
        }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
            BallFollowLimitDistance = 3200.0f;
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            PawnStartXPoint = MyOwner.GetActorLocation().X;
            gamemaster.OnBallLaunch += StartFollowingBall;
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            if (bShouldFollowBall && myBall != null)
            {
                var _myPos = MyOwner.GetActorLocation();

                if (_myPos.X >= BallFollowLimitDistance)
                {
                    bShouldFollowBall = false;
                    return;
                }

                var _ballPos = myBall.MyOwner.GetActorLocation();
                var _xTravelPos = _ballPos.X + DefaultBallFollowOffset;
                //PrintString("Ball Pos: " + _ballPos, FLinearColor.Green, printToLog:true);
                MyOwner.SetActorLocation(
                    new FVector(_xTravelPos, _myPos.Y, _myPos.Z),
                    true, out myHit, false
                );

            }
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            if (gamemaster != null)
            {
                gamemaster.OnBallLaunch -= StartFollowingBall;
            }
        }
        #endregion

        #region Getters
        [UFunction, BlueprintCallable]
        public FVector GetMyBallPosition()
        {
            if (myBall != null)
            {
                return myBall.MyOwner.GetActorLocation();
            }
            return new FVector(0, 0, 0);
        }
        #endregion

        #region Handlers
        void StartFollowingBall(FVector launchVelocity, BowlingBallComponent bowlingBall)
        {
            myBall = bowlingBall;
            if (myBall != null)
            {
                DefaultBallFollowOffset = MyOwner.GetActorLocation().X - myBall.MyOwner.GetActorLocation().X;
                bShouldFollowBall = true;
            }
            else
            {
                bShouldFollowBall = false;
            }
        }
        #endregion

        [UFunction, BlueprintCallable]
        public void OnDragStart(FVector2D mousePos)
        {
            if (gamemode != null)
            {
                gamemode.OnStartDrag(mousePos);
            }
            
        }

        [UFunction, BlueprintCallable]
        public void OnDragStop(FVector2D mousePos)
        {
            if (gamemode != null)
            {
                gamemode.OnStopDrag(mousePos);
            }
        }
    }
}
