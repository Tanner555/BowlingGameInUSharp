﻿using System;
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

        #region BowlUProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame01_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame01_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame01_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame02_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame02_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame02_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame03_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame03_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame03_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame04_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame04_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame04_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame05_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame05_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame05_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame06_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame06_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame06_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame07_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame07_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame07_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame08_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame08_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame08_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame09_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame09_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame09_Results { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame10_BowlA { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame10_BowlB { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame10_BowlC { get; set; }
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowl Scores")]
        public int Frame10_Results { get; set; }
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

        private int lastSettledCount = 10;
        /// <summary>
        /// Should Start At 1 Before Taking First Bowl Turn,
        /// Gets AllTurnsList Starting at 0, Excludes Final Turn Number
        /// </summary>
        private int BowlTurnCount = 0;

        private List<int> AllBowlFrameResults
        {
            get
            {
                return new List<int>
                {
                    Frame01_BowlA, Frame01_BowlB, Frame01_Results,
                    Frame02_BowlA, Frame02_BowlB, Frame02_Results,
                    Frame03_BowlA, Frame03_BowlB, Frame03_Results,
                    Frame04_BowlA, Frame04_BowlB, Frame04_Results,
                    Frame05_BowlA, Frame05_BowlB, Frame05_Results,
                    Frame06_BowlA, Frame06_BowlB, Frame06_Results,
                    Frame07_BowlA, Frame07_BowlB, Frame07_Results,
                    Frame08_BowlA, Frame08_BowlB, Frame08_Results,
                    Frame09_BowlA, Frame09_BowlB, Frame09_Results,
                    Frame10_BowlA, Frame10_BowlB, Frame10_BowlC, Frame10_Results
                };
            }
        }

        private List<int> AllBowlFrameTurns
        {
            get
            {
                return new List<int>
                {
                    Frame01_BowlA, Frame01_BowlB,
                    Frame02_BowlA, Frame02_BowlB,
                    Frame03_BowlA, Frame03_BowlB,
                    Frame04_BowlA, Frame04_BowlB,
                    Frame05_BowlA, Frame05_BowlB,
                    Frame06_BowlA, Frame06_BowlB,
                    Frame07_BowlA, Frame07_BowlB,
                    Frame08_BowlA, Frame08_BowlB,
                    Frame09_BowlA, Frame09_BowlB,
                    Frame10_BowlA, Frame10_BowlB, Frame10_BowlC
                };
            }
        }
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
            gamemaster.OnSendBowlActionResults += OnSendBowlActionResults;

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
                gamemaster.OnSendBowlActionResults -= OnSendBowlActionResults;
            }
        }
        #endregion

        #region Getters
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

        public int GetPinFallCount()
        {
            return lastSettledCount - StandingPinCount;
        }

        public List<int> GetBowlTurnListFromCount()
        {
            List<int> _bowlTurns = new List<int>();
            for (int i = 0; i < BowlTurnCount; i++)
            {
                _bowlTurns.Add(AllBowlFrameTurns[i]);
            }
            return _bowlTurns;
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

        public void SetResultsFromFrameTurns()
        {
            Frame01_Results = Frame01_BowlA + Frame01_BowlB;
            Frame02_Results = Frame02_BowlA + Frame02_BowlB;
            Frame03_Results = Frame03_BowlA + Frame03_BowlB;
            Frame04_Results = Frame04_BowlA + Frame04_BowlB;
            Frame05_Results = Frame05_BowlA + Frame05_BowlB;
            Frame06_Results = Frame06_BowlA + Frame06_BowlB;
            Frame07_Results = Frame07_BowlA + Frame07_BowlB;
            Frame08_Results = Frame08_BowlA + Frame08_BowlB;
            Frame09_Results = Frame09_BowlA + Frame09_BowlB;
            Frame10_Results = Frame10_BowlA + Frame10_BowlB + Frame10_BowlC;
        }
        
        public void SetCurrentBowlTurnValue(int _value)
        {
            switch (BowlTurnCount)
            {
                case 1:
                    Frame01_BowlA = _value;
                    break;
                case 2:
                    Frame01_BowlB = _value;
                    break;
                case 3:
                    Frame02_BowlA = _value;
                    break;
                case 4:
                    Frame02_BowlB = _value;
                    break;
                case 5:
                    Frame03_BowlA = _value;
                    break;
                case 6:
                    Frame03_BowlB = _value;
                    break;
                case 7:
                    Frame04_BowlA = _value;
                    break;
                case 8:
                    Frame04_BowlB = _value;
                    break;
                case 9:
                    Frame05_BowlA = _value;
                    break;
                case 10:
                    Frame05_BowlB = _value;
                    break;
                case 11:
                    Frame06_BowlA = _value;
                    break;
                case 12:
                    Frame06_BowlB = _value;
                    break;
                case 13:
                    Frame07_BowlA = _value;
                    break;
                case 14:
                    Frame07_BowlB = _value;
                    break;
                case 15:
                    Frame08_BowlA = _value;
                    break;
                case 16:
                    Frame08_BowlB = _value;
                    break;
                case 17:
                    Frame09_BowlA = _value;
                    break;
                case 18:
                    Frame09_BowlB = _value;
                    break;
                case 19:
                    Frame10_BowlA = _value;
                    break;
                case 20:
                    Frame10_BowlB = _value;
                    break;
                case 21:
                    Frame10_BowlC = _value;
                    break;
                default:
                    MyOwner.PrintString("Couldn't Set Bowl Turn At: " + BowlTurnCount, FLinearColor.Red, printToLog: true);
                    break;
            }
        }
        #endregion

        #region Handlers
        void OnSendBowlActionResults(BowlAction _action)
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

            if (_action != BowlAction.Tidy)
            {
                _myPlayer = ULevelSequencePlayer.CreateLevelSequencePlayer(this, ClearSweepLevelSequence, _settings, out _mySequenceActor);
            }
            else
            {
                _myPlayer = ULevelSequencePlayer.CreateLevelSequencePlayer(this, CleanUpSweepLevelSequence, _settings, out _mySequenceActor);
            }

            _myPlayer.Play();
            float _waitLength = _myPlayer.GetLength();
            WaitTillSweepingIsDone(_waitLength, _action);
        }

        void OnTurnIsFinished(bool _isRoundOver)
        {
            int _pinFall = GetPinFallCount();
            lastSettledCount = StandingPinCount;
            BowlAction _action = Bowl(_pinFall);
            SetResultsFromFrameTurns();
            gamemaster.CallOnSendBowlActionResults(_action);
        }

        void UpdatePinCount(BowlingPinComponent _pin)
        {
            StandingPinCount--;
        }

        void ResetPinCount(bool _roundIsOver, BowlAction _action)
        {
            if(_action != BowlAction.Tidy)
            {
                StandingPinCount = 10;
                lastSettledCount = 10;
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
        public void WaitTillSweepingIsDone(float _animLength, BowlAction _action)
        {
            StartCoroutine(this, WaitTillSweepingIsDoneCoroutine(_animLength, _action));
        }

        private IEnumerator WaitTillSweepingIsDoneCoroutine(float _animLength, BowlAction _action)
        {
            yield return new WaitForSeconds(_animLength);
            CallNewTurnIsReadyAfterWaiting(_action);
        }

        void CallNewTurnIsReadyAfterWaiting(BowlAction _action)
        {
            gamemaster.CallBowlNewTurnIsReady(_action);
        }
        #endregion

        #region Bowling
        [UFunctionIgnore]
        public BowlAction Bowl(int pinFall)
        {
            BowlTurnCount += 1;
            SetCurrentBowlTurnValue(pinFall);
            List<int> _rolls = GetBowlTurnListFromCount();
            return BowlActionMaster.NextAction(_rolls);
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
