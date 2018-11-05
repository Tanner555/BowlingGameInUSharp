using System;
using System.Collections;
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
using System.Threading.Tasks;

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
        public FName PinManagerTag => new FName("PinManager");

        [UPropertyIngore]
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        #endregion

        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public float MinimalForwardLaunchVelocity { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public TSubclassOf<AActor> BowlingBallSubClassReference { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public float ForwardMultipleVelocityFactor { get; set; }
        #endregion

        #region Fields
        protected APlayerCameraManager myCameraManager = null;
        protected BowlingBallComponent myBall = null;
        protected MyBowlPlayerComponent myBowler = null;

        private FVector2D dragStart, dragEnd;
        private float startTime, endTime;

        protected static WorldStaticVar<BowlGameModeComponent> ThisInstance = new WorldStaticVar<BowlGameModeComponent>();
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            MinimalForwardLaunchVelocity = 1500;
            ForwardMultipleVelocityFactor = 1.5f;
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            MyOwner.World.GetPlayerController(0).ShowMouseCursor = true;

            List<AActor> ballActors;
            MyOwner.World.GetAllActorsWithTag(BallTag, out ballActors);
            SetBallFromBallFindCollection(ballActors);
            myBowler = MyOwner.World.GetPlayerPawn(0).GetComponentByClass<MyBowlPlayerComponent>();
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {

        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            StopAllCoroutines();
        }
        #endregion

        #region Getter
        public static BowlGameModeComponent GetInstance(UObject worldContextObject)
        {
            var _instanceHelper = ThisInstance.Get(worldContextObject);
            if (_instanceHelper == null)
            {
                _instanceHelper = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<BowlGameModeComponent>();
                ThisInstance.Set(worldContextObject, _instanceHelper);
            }
            return _instanceHelper; 
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

            FVector _launchVelocity = new FVector(launchSpeedX * ForwardMultipleVelocityFactor, launchSpeedY, 0);
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

        #region PublicUFunctionCalls
        [UFunction, BlueprintCallable]
        public void NudgeBallLeft()
        {
            if (gamemaster.bCanLaunchBall)
            {
                gamemaster.CallOnNudgeBallLeft(-50);
            }
        }

        [UFunction, BlueprintCallable]
        public void NudgeBallRight()
        {
            if (gamemaster.bCanLaunchBall)
            {
                gamemaster.CallOnNudgeBallRight(50);
            }
        }

        [UFunction, BlueprintCallable]
        public void EndBowlingTurn()
        {
            if (gamemaster.bBowlTurnIsOver == false)
            {
                gamemaster.CallBowlTurnIsFinished();
            }
        }
        #endregion

        #region SweepingAnimationWaitCalls
        [UFunction, BlueprintCallable]
        public void WaitTillSweepingIsDone(float _animLength)
        {
            StartCoroutine(this, WaitTillSweepingIsDoneCoroutine(_animLength));
        }

        private IEnumerator WaitTillSweepingIsDoneCoroutine(float _animLength)
        {
            yield return new WaitForSeconds(_animLength);
            CallNewTurnIsReadyAfterWaiting();
        }

        void CallNewTurnIsReadyAfterWaiting()
        {
            gamemaster.CallBowlNewTurnIsReady();
        }
        #endregion

        #region UnusedCode
        //void AnotherTestMethod()
        //{
        //    var _c = StartCoroutine(this, WaitTillSweepingIsDoneCoroutine(_animLength));
        //}

        //private IEnumerator WaitTillSweepingIsDoneCoroutine(float _animLength)
        //{
        //    yield return new WaitForSeconds(_animLength);
        //    MyOwner.PrintString("Waiting For: " + _animLength.ToString(), FLinearColor.Green);
        //}
        #endregion
    }
}
