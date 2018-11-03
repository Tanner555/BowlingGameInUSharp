﻿using System.Text;
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
    public class BowlGameMasterComponent : UActorComponent
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
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);

        public bool bBowlTurnIsOver { get; protected set; }
        public bool bCanLaunchBall { get; protected set; }

        [UPropertyIngore]
        protected static bool bCanSetInstance;
        #endregion

        #region UProperties

        #endregion

        #region Fields
        protected static BowlGameMasterComponent ThisInstance = null;
        #endregion

        #region Getters
        public static BowlGameMasterComponent GetInstance(UObject worldContextObject)
        {
            if (ThisInstance == null)
            {
                ThisInstance = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<BowlGameMasterComponent>();
            }
            return ThisInstance;
        }
        #endregion

        #region Delegates
        public delegate void GeneralEventHandler();
        public delegate void FVectorAndBallRefHandler(FVector launchVelocity, BowlingBallComponent bowlingBall);
        public delegate void OneFloatArgHandler(float famount);
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //Set ThisInstance To Null, Otherwise Value Doesn't Get Destroyed and Will Crash Engine.
            ThisInstance = null;
            bCanSetInstance = true;
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            bBowlTurnIsOver = false;
            bCanLaunchBall = true;
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            bCanSetInstance = false;
            ThisInstance = null;
        }
        #endregion

        #region Events
        public event GeneralEventHandler BowlNewTurnIsReady;
        public event GeneralEventHandler BowlTurnIsFinished;
        public event FVectorAndBallRefHandler OnBallLaunch;
        public event OneFloatArgHandler OnNudgeBallLeft;
        public event OneFloatArgHandler OnNudgeBallRight;
        #endregion

        #region EventCalls
        public void CallOnBallLaunch(FVector launchVelocity, BowlingBallComponent bowlingBall)
        {
            bCanLaunchBall = false;
            if (OnBallLaunch != null) OnBallLaunch(launchVelocity, bowlingBall);
        }

        public void CallBowlNewTurnIsReady()
        {
            bBowlTurnIsOver = false;
            bCanLaunchBall = true;
            if (BowlNewTurnIsReady != null) BowlNewTurnIsReady();
        }

        public void CallBowlTurnIsFinished()
        {
            bBowlTurnIsOver = true;
            if (BowlTurnIsFinished != null) BowlTurnIsFinished();
            //TODO: Implement Pin Cleanup Functionality Before Starting New Turn
            CallBowlNewTurnIsReady();
        }

        public void CallOnNudgeBallLeft(float famount)
        {
            if (OnNudgeBallLeft != null) OnNudgeBallLeft(famount);
        }

        public void CallOnNudgeBallRight(float famount)
        {
            if (OnNudgeBallRight != null) OnNudgeBallRight(famount);
        }
        #endregion

        #region UnusedCode
        //public class GeneralDelegateHandler : FMulticastDelegate<GeneralDelegateHandler.Signature>
        //{
        //    public delegate void Signature();
        //}

        //[UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        //public GeneralDelegateHandler BowlTurnIsFinishedTest { get; set; }

        //[UFunction, BlueprintCallable]
        //public void CallBowlTurnIsFinishedTest()
        //{
        //    if (BowlTurnIsFinishedTest.IsBound)
        //    {
        //        BowlTurnIsFinishedTest.Invoke();
        //    }
        //}
        #endregion
    }
}
