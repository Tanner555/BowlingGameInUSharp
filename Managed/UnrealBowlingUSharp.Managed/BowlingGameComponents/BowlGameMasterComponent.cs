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
    public class BowlGameMasterComponent : UActorComponent
    {
        #region IgnoreProperties
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
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);

        public bool bBowlTurnIsOver { get; protected set; }
        public bool bCanLaunchBall { get; protected set; }
        #endregion

        #region UProperties

        #endregion

        #region Fields
        protected static WorldStaticVar<BowlGameMasterComponent> ThisInstance = new WorldStaticVar<BowlGameMasterComponent>();
        #endregion

        #region Getters
        public static BowlGameMasterComponent GetInstance(UObject worldContextObject)
        {
            var _instanceHelper = ThisInstance.Get(worldContextObject);
            if (_instanceHelper == null)
            {
                _instanceHelper = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<BowlGameMasterComponent>();
                ThisInstance.Set(worldContextObject, _instanceHelper);
            }
            return _instanceHelper;
        }
        #endregion

        #region Delegates
        public delegate void GeneralEventHandler();
        public delegate void FVectorAndBallRefHandler(FVector launchVelocity, BowlingBallComponent bowlingBall);
        public delegate void OneFloatArgHandler(float famount);
        public delegate void OneIntArgHandler(int iamount);
        public delegate void OneBoolArgHandler(bool _isTrue);
        //public delegate void OneBoolOneBowlActionArgHandler(bool _isTrue, EBowlAction _action);
        public delegate void OnePinArgHandler(BowlingPinComponent _pin);
        public delegate void BowlActionArgHandler(EBowlAction _action);
        #endregion

        #region FMulticastDelegates
        public class GeneralDelegateHandler : FMulticastDelegate<GeneralDelegateHandler.Signature>
        {
            public delegate void Signature();
        }

        public class OneBoolArgDelegateHandler : FMulticastDelegate<OneBoolArgDelegateHandler.Signature>
        {
            public delegate void Signature(bool _isTrue);
        }

        public class OnePinArgDelegateHandler : FMulticastDelegate<OnePinArgDelegateHandler.Signature>
        {
            public delegate void Signature(BowlingPinComponent _pin);
        }

        public class OneIntArgDelegateHandler : FMulticastDelegate<OneIntArgDelegateHandler.Signature>
        {
            public delegate void Signature(int _number);
        }

        //[UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        //public OneBoolArgDelegateHandler BowlTurnIsFinishedDelegate { get; set; }
        [UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        public OneIntArgDelegateHandler UpdatePinCountDelegate { get; set; }
        //[UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        //public OnePinArgDelegateHandler OnPinHasFallenDelegate { get; set; }
        //[UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        //public GeneralDelegateHandler OnWinGameDelegate { get; set; }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {

        }

        public override void BeginPlay()
        {
            bBowlTurnIsOver = false;
            bCanLaunchBall = true;
        }

        public override void EndPlay(EEndPlayReason endPlayReason)
        {
            
        }
        #endregion

        #region Events
        /// <summary>
        /// Isn't Called On Begin Play, Only After Pin Sweep Is Finished
        /// </summary>
        public event BowlActionArgHandler BowlNewTurnIsReady;

        public event GeneralEventHandler BowlTurnIsFinished;
        public event FVectorAndBallRefHandler OnBallLaunch;
        public event OneFloatArgHandler OnNudgeBallLeft;
        public event OneFloatArgHandler OnNudgeBallRight;
        public event OnePinArgHandler OnPinHasFallen;
        public event OnePinArgHandler OnPinHasGottenBackUp;
        public event OneIntArgHandler OnUpdatePinCount;
        public event BowlActionArgHandler OnSendBowlActionResults;
        public event GeneralEventHandler OnWinGame;
        //Debug
        public event GeneralEventHandler Debug_OnSimulateStrike;
        public event GeneralEventHandler Debug_Fill18ScoreSlots;
        #endregion

        #region EventCalls
        public void CallOnBallLaunch(FVector launchVelocity, BowlingBallComponent bowlingBall)
        {
            bCanLaunchBall = false;
            if (OnBallLaunch != null) OnBallLaunch(launchVelocity, bowlingBall);
        }

        public void CallBowlNewTurnIsReady(EBowlAction _action)
        {
            bBowlTurnIsOver = false;
            bCanLaunchBall = true;
            if (BowlNewTurnIsReady != null) BowlNewTurnIsReady(_action);
        }

        public void CallBowlTurnIsFinished()
        {
            //Only Call If Bowl Turn Isn't Finished Yet
            if (bBowlTurnIsOver) return;

            bBowlTurnIsOver = true;
            if (BowlTurnIsFinished != null) BowlTurnIsFinished();
        }

        public void CallOnNudgeBallLeft(float famount)
        {
            if (OnNudgeBallLeft != null) OnNudgeBallLeft(famount);
        }

        public void CallOnNudgeBallRight(float famount)
        {
            if (OnNudgeBallRight != null) OnNudgeBallRight(famount);
        }

        public void CallOnPinHasFallen(BowlingPinComponent _pin)
        {
            if (OnPinHasFallen != null) OnPinHasFallen(_pin);
            //if (OnPinHasFallenDelegate.IsBound)
            //{
            //    OnPinHasFallenDelegate.Invoke(_pin);
            //}
        }

        public void CallOnPinHasGottenBackUp(BowlingPinComponent _pin)
        {
            if (OnPinHasGottenBackUp != null) OnPinHasGottenBackUp(_pin);
        }

        public void CallOnUpdatePinCount(int _pinCount)
        {
            if (OnUpdatePinCount != null) OnUpdatePinCount(_pinCount);
        }

        public void CallUpdatePinCount(int _count)
        {
            if (UpdatePinCountDelegate.IsBound)
            {
                UpdatePinCountDelegate.Invoke(_count);
            }
        }

        public void CallOnSendBowlActionResults(EBowlAction _action)
        {
            if (OnSendBowlActionResults != null) OnSendBowlActionResults(_action);
        }

        public void CallOnWinGame()
        {
            if (OnWinGame != null) OnWinGame();
        }

        //Debug
        public void CallDebug_OnSimulateStrike()
        {
            if (Debug_OnSimulateStrike != null)
            {
                Debug_OnSimulateStrike();
            }
        }

        public void CallDebug_Fill18ScoreSlots()
        {
            if(Debug_Fill18ScoreSlots != null)
            {
                Debug_Fill18ScoreSlots();
            }
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
