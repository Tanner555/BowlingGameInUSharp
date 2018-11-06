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
using UnrealEngine.LevelSequence;
using UnrealEngine.MovieScene;

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
        public FName BowlingFloorTag => new FName("BowlingFloor");

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

        [UPropertyIngore]
        public int StandingPinCount
        {
            get { return _standingPinCount; }
            set
            {
                _standingPinCount = value;
                gamemaster.CallUpdatePinCount(_standingPinCount);
            }
        }
        private int _standingPinCount = 0;

        [UProperty, EditAnywhere, BlueprintReadOnly, Category("Bowling")]
        public ULevelSequence CleanUpSweepLevelSequence { get; set; }

        [UProperty, EditAnywhere, BlueprintReadOnly, Category("Bowling")]
        public ULevelSequence ClearSweepLevelSequence { get; set; }
        #endregion

        #region Fields
        protected APlayerCameraManager myCameraManager = null;
        protected BowlingBallComponent myBall = null;
        protected MyBowlPlayerComponent myBowler = null;

        private FVector2D dragStart, dragEnd;
        private float startTime, endTime;

        private AStaticMeshActor BowlFloorMeshActor = null;
        private float boundsYLeftEdge;
        private float boundsYRightEdge;
        private float boundsYPaddingCheck = 10.0f;

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
            StandingPinCount = 10;
            gamemaster.OnPinHasFallen += UpdatePinCount;
            gamemaster.BowlNewTurnIsReady += ResetPinCount;
            gamemaster.BowlTurnIsFinished += OnTurnIsFinished;

            List<AActor> bowlFloorActors;
            MyOwner.World.GetAllActorsWithTag(BowlingFloorTag, out bowlFloorActors);
            if(bowlFloorActors[0] != null)
            {
                var _staticActor = bowlFloorActors[0].Cast<AStaticMeshActor>();
                if(_staticActor != null)
                {
                    BowlFloorMeshActor = _staticActor;
                    FVector _origin;
                    FVector _bounds;
                    BowlFloorMeshActor.GetActorBounds(false, out _origin, out _bounds);
                    boundsYLeftEdge = _origin.Y - _bounds.Y;
                    boundsYRightEdge = _origin.Y + _bounds.Y;
                }
                else
                {
                    MyOwner.PrintString("Couldn't Find BowlFloor Blueprint Actor", FLinearColor.Red, printToLog: true);
                }
            }
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {

        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            StopAllCoroutines();
            if(gamemaster != null)
            {
                gamemaster.OnPinHasFallen -= UpdatePinCount;
                gamemaster.BowlNewTurnIsReady -= ResetPinCount;
                gamemaster.BowlTurnIsFinished -= OnTurnIsFinished;
            }
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

        #region Handlers
        void OnTurnIsFinished(bool _isRoundOver)
        {
            if (CleanUpSweepLevelSequence == null ||
                ClearSweepLevelSequence == null)
            {
                MyOwner.PrintString("Please Assign Animation Clips To Clear and Cleanup Level Sequence UProperties", FLinearColor.Red);
                return;
            }

            ALevelSequenceActor _mySequenceActor;
            ULevelSequencePlayer _myPlayer;
            FMovieSceneSequencePlaybackSettings _settings = new FMovieSceneSequencePlaybackSettings
            {
                StartTime = 0,
                RestoreState = true,
                PlayRate = 1.0f
            };

            if (_isRoundOver)
            {
                _myPlayer = ULevelSequencePlayer.CreateLevelSequencePlayer(this, ClearSweepLevelSequence, _settings, out _mySequenceActor);
            }
            else
            {
                _myPlayer = ULevelSequencePlayer.CreateLevelSequencePlayer(this, CleanUpSweepLevelSequence, _settings, out _mySequenceActor);
            }
            
            _myPlayer.Play();
            float _waitLength = _myPlayer.GetLength();
            WaitTillSweepingIsDone(_waitLength);
        }

        void UpdatePinCount(BowlingPinComponent _pin)
        {
            StandingPinCount--;
        }

        void ResetPinCount(bool _roundIsOver)
        {
            if (_roundIsOver)
            {
                StandingPinCount = 10;
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
            if (myBall != null && 
                gamemaster.bCanLaunchBall && 
                gamemaster.bBowlTurnIsOver == false)
            {
                gamemaster.CallOnBallLaunch(launchVelocity, myBall);
            }
        }
        #endregion

        #region PublicUFunctionCalls
        [UFunction, BlueprintCallable]
        public void NudgeBallLeft()
        {
            float _nudgeAmount = -50;
            if (gamemaster.bCanLaunchBall &&
                gamemaster.bBowlTurnIsOver == false &&
                myBall != null)
            {
                FVector _ballPos = myBall.MyOwner.GetActorLocation();
                float _nextBallY = _ballPos.Y + _nudgeAmount;
                if (_nextBallY > (boundsYLeftEdge + boundsYPaddingCheck))
                {
                    gamemaster.CallOnNudgeBallLeft(_nudgeAmount);
                }
            }
        }

        [UFunction, BlueprintCallable]
        public void NudgeBallRight()
        {
            float _nudgeAmount = 50;
            if (gamemaster.bCanLaunchBall &&
                gamemaster.bBowlTurnIsOver == false &&
                myBall != null)
            {
                FVector _ballPos = myBall.MyOwner.GetActorLocation();
                float _nextBallY = _ballPos.Y + _nudgeAmount;
                if (_nextBallY < (boundsYRightEdge - boundsYPaddingCheck))
                {
                    gamemaster.CallOnNudgeBallRight(_nudgeAmount);
                }
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

        [UFunction, BlueprintCallable]
        public bool IsPlayerRoundCompletelyOver()
        {
            return false;
        }

        [UFunction, BlueprintCallable]
        public int GetStandingPinCount()
        {
            return StandingPinCount;
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
        //    var _coroutine = StartCoroutine(this, WaitTillSweepingIsDoneCoroutine(_animLength));
        //}

        //private IEnumerator WaitTillSweepingIsDoneCoroutine(float _animLength)
        //{
        //    yield return new WaitForSeconds(_animLength);
        //    MyOwner.PrintString("Waiting For: " + _animLength.ToString(), FLinearColor.Green);
        //}
        #endregion
    }
}
