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
        public delegate void OneBoolArgHandler(bool _isTrue);
        public delegate void OneBoolOneBowlActionArgHandler(bool _isTrue, BowlAction _action);
        public delegate void OnePinArgHandler(BowlingPinComponent _pin);
        public delegate void BowlActionArgHandler(BowlAction _action);
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

        [UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        public OneBoolArgDelegateHandler BowlTurnIsFinishedDelegate { get; set; }
        [UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        public OneIntArgDelegateHandler UpdatePinCountDelegate { get; set; }
        //[UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        //public OnePinArgDelegateHandler OnPinHasFallenDelegate { get; set; }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {

        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            bBowlTurnIsOver = false;
            bCanLaunchBall = true;
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {

        }
        #endregion

        #region Events
        public event OneBoolOneBowlActionArgHandler BowlNewTurnIsReady;
        public event OneBoolArgHandler BowlTurnIsFinished;
        public event FVectorAndBallRefHandler OnBallLaunch;
        public event OneFloatArgHandler OnNudgeBallLeft;
        public event OneFloatArgHandler OnNudgeBallRight;
        public event OnePinArgHandler OnPinHasFallen;
        public event BowlActionArgHandler OnSendBowlActionResults;
        #endregion

        #region EventCalls
        public void CallOnBallLaunch(FVector launchVelocity, BowlingBallComponent bowlingBall)
        {
            bCanLaunchBall = false;
            if (OnBallLaunch != null) OnBallLaunch(launchVelocity, bowlingBall);
        }

        public void CallBowlNewTurnIsReady(BowlAction _action)
        {
            bBowlTurnIsOver = false;
            bCanLaunchBall = true;
            bool _bPlayerRoundIsOver = gamemode.IsPlayerRoundCompletelyOver();
            if (BowlNewTurnIsReady != null) BowlNewTurnIsReady(_bPlayerRoundIsOver, _action);
        }

        public void CallBowlTurnIsFinished()
        {
            //Only Call If Bowl Turn Isn't Finished Yet
            if (bBowlTurnIsOver) return;

            bBowlTurnIsOver = true;
            bool _bPlayerRoundIsOver = gamemode.IsPlayerRoundCompletelyOver();
            if (BowlTurnIsFinished != null) BowlTurnIsFinished(_bPlayerRoundIsOver);
            if (BowlTurnIsFinishedDelegate.IsBound)
            {
                BowlTurnIsFinishedDelegate.Invoke(_bPlayerRoundIsOver);
            }
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

        public void CallUpdatePinCount(int _count)
        {
            if (UpdatePinCountDelegate.IsBound)
            {
                UpdatePinCountDelegate.Invoke(_count);
            }
        }

        public void CallOnSendBowlActionResults(BowlAction _action)
        {
            if (OnSendBowlActionResults != null) OnSendBowlActionResults(_action);
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
