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
using USharpBowlProject;

namespace OLDREFUSharpBowlProject
{
    [UClass, Blueprintable, BlueprintType]
    public class UMyBowlPlayerComponent : UActorComponent
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
        [UPropertyIgnore]
        protected float PawnStartXPoint { get; set; }
        [UPropertyIgnore]
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
        [UPropertyIgnore]
        protected UBowlGameMasterComponent gamemaster => UBowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIgnore]
        protected UBowlGameModeComponent gamemode => UBowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region Fields
        protected bool bShouldFollowBall = false;
        private FHitResult myHit;
        private UBowlingBallComponent myBall = null;
        private FVector MyStartLocation;
        private FRotator MyStartRotation;
        #endregion

        #region Testing
        [UFunction, BlueprintCallable]
        public string GetMyMessage()
        {
            return "Hello There From MyPawnClass1234556789";
        }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
            BallFollowLimitDistance = 3200.0f;
        }

        public override void BeginPlay()
        {
            PawnStartXPoint = MyOwner.GetActorLocation().X;
            gamemaster.OnBallLaunch += StartFollowingBall;
            gamemaster.OnNudgeBallLeft += NudgeBallLeft;
            gamemaster.OnNudgeBallRight += NudgeBallRight;
            gamemaster.BowlNewTurnIsReady += NewTurnIsReady;
            gamemaster.OnWinGame += OnWinGame;

            MyStartLocation = MyOwner.GetActorLocation();
            MyStartRotation = MyOwner.GetActorRotation();
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

        public override void EndPlay(EEndPlayReason endPlayReason)
        {
            if (gamemaster != null)
            {
                gamemaster.OnBallLaunch -= StartFollowingBall;
                gamemaster.OnNudgeBallLeft -= NudgeBallLeft;
                gamemaster.OnNudgeBallRight -= NudgeBallRight;
                gamemaster.BowlNewTurnIsReady -= NewTurnIsReady;
                gamemaster.OnWinGame -= OnWinGame;
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

        [UFunction, BlueprintCallable]
        public UBowlGameModeComponent GetBowlGameModeComponent()
        {
            return gamemode;
        }

        [UFunction, BlueprintCallable]
        public UBowlGameMasterComponent GetBowlGameMasterComponent()
        {
            return gamemaster;
        }
        #endregion

        #region Handlers
        void OnWinGame()
        {
            //OnWinGameImplementEvent();
            if (OnWinGameDelegate.IsBound)
            {
                OnWinGameDelegate.Invoke();
            }
        }

        void NewTurnIsReady(EBowlAction _action)
        {
            MyOwner.SetActorLocation(
                MyStartLocation, false, out myHit, false
                );
            MyOwner.SetActorRotation(
                MyStartRotation, false
                );
        } 

        void StartFollowingBall(FVector launchVelocity, UBowlingBallComponent bowlingBall)
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

        void NudgeBallLeft(float famount)
        {
            FHitResult _hit;
            MyOwner.SetActorLocation(
                MyOwner.GetActorLocation() +
                new FVector(0, famount, 0), false, out _hit, false);
        }

        void NudgeBallRight(float famount)
        {
            FHitResult _hit;
            MyOwner.SetActorLocation(
                MyOwner.GetActorLocation() +
                new FVector(0, famount, 0), false, out _hit, false);
        }
        #endregion

        #region BlueprintImplementableEvents
        ////[UFunction, BlueprintImplementedEvent]
        ////public void OnWinGameImplementEvent()
        ////{

        ////}
        #endregion

        #region TestingDelegates
        [UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        public UBowlGameMasterComponent.FGeneralDelegateHandler OnWinGameDelegate { get; set; }
        #endregion

        #region PublicUFunctions
        [UFunction, BlueprintCallable]
        public void OnDragStart(FVector2D mousePos)
        {
            if (gamemode != null && gamemaster.bCanLaunchBall &&
                gamemaster.bBowlTurnIsOver == false)
            {
                gamemode.OnStartDrag(mousePos);
            }

        }

        [UFunction, BlueprintCallable]
        public void OnDragStop(FVector2D mousePos)
        {
            if (gamemode != null && gamemaster.bCanLaunchBall &&
                gamemaster.bBowlTurnIsOver == false)
            {
                gamemode.OnStopDrag(mousePos);
            }
        }

        [UFunction, BlueprintCallable]
        public void Debug_InstantStrike()
        {
            gamemaster.CallDebug_OnSimulateStrike();
        }

        [UFunction, BlueprintCallable]
        public void Debug_Fill18ScoreSlots()
        {
            gamemaster.CallDebug_Fill18ScoreSlots();
        }
        #endregion
    }
}
