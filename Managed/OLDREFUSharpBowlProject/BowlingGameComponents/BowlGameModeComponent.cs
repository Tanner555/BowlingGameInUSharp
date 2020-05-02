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
using UnrealEngine.TimeManagement;
using USharpBowlProject;

namespace OLDREFUSharpBowlProject
{
    #region GlobalEnums
    [UEnum]
    public enum EBowlFrame : System.Byte
    {
        Frame01 = 0,
        Frame02 = 1,
        Frame03 = 2,
        Frame04 = 3,
        Frame05 = 4,
        Frame06 = 5,
        Frame07 = 6,
        Frame08 = 7,
        Frame09 = 8,
        Frame10 = 9
    }
    #endregion

    [UClass, Blueprintable, BlueprintType]
    public class UBowlGameModeComponent : UActorComponent
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
        public int StandingPinCount
        {
            get { return _standingPinCount; }
            set
            {
                _standingPinCount = value;
                gamemaster.CallUpdatePinCount(_standingPinCount);
                UpdatePinCountBPEvent(_standingPinCount);
            }
        }
        private int _standingPinCount = 0;

        /// <summary>
        /// Should Start At 1 Before Taking First Bowl Turn,
        /// Gets AllTurnsList Starting at 0, Excludes Final Turn Number
        /// </summary>
        [UPropertyIgnore]
        public int BowlTurnCount { get; set; }

        [UPropertyIgnore]
        public FName BallTag => new FName("Ball");
        [UPropertyIgnore]
        public FName PinTag => new FName("Pin");
        [UPropertyIgnore]
        public FName PinManagerTag => new FName("PinManager");
        [UPropertyIgnore]
        public FName BowlingFloorTag => new FName("BowlingFloor");

        [UPropertyIgnore]
        protected UBowlGameMasterComponent gamemaster => UBowlGameMasterComponent.GetInstance(MyOwner);

        [UPropertyIgnore]
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

        [UPropertyIgnore]
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

        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public float MinimalForwardLaunchVelocity { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public TSubclassOf<AActor> BowlingBallSubClassReference { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public float ForwardMultipleVelocityFactor { get; set; }

        [UProperty, EditAnywhere, BlueprintReadOnly, Category("Bowling")]
        public ULevelSequence CleanUpSweepLevelSequence { get; set; }

        [UProperty, EditAnywhere, BlueprintReadOnly, Category("Bowling")]
        public ULevelSequence ClearSweepLevelSequence { get; set; }
        #endregion

        #region BowlUProperties
        [UPropertyIgnore]
        public int Frame01_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame01_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame01_Results { get; set; }
        [UPropertyIgnore]
        public int Frame02_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame02_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame02_Results { get; set; }
        [UPropertyIgnore]
        public int Frame03_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame03_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame03_Results { get; set; }
        [UPropertyIgnore]
        public int Frame04_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame04_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame04_Results { get; set; }
        [UPropertyIgnore]
        public int Frame05_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame05_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame05_Results { get; set; }
        [UPropertyIgnore]
        public int Frame06_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame06_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame06_Results { get; set; }
        [UPropertyIgnore]
        public int Frame07_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame07_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame07_Results { get; set; }
        [UPropertyIgnore]
        public int Frame08_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame08_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame08_Results { get; set; }
        [UPropertyIgnore]
        public int Frame09_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame09_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame09_Results { get; set; }
        [UPropertyIgnore]
        public int Frame10_BowlA { get; set; }
        [UPropertyIgnore]
        public int Frame10_BowlB { get; set; }
        [UPropertyIgnore]
        public int Frame10_BowlC { get; set; }
        [UPropertyIgnore]
        public int Frame10_Results { get; set; }
        #endregion

        #region Fields
        protected APlayerCameraManager myCameraManager = null;
        protected UBowlingBallComponent myBall = null;
        protected UMyBowlPlayerComponent myBowler = null;

        private FVector2D dragStart, dragEnd;
        private float startTime, endTime;

        private AStaticMeshActor BowlFloorMeshActor = null;
        private float boundsYLeftEdge;
        private float boundsYRightEdge;
        private float boundsYPaddingCheck = 10.0f;

        protected static WorldStaticVar<UBowlGameModeComponent> ThisInstance = new WorldStaticVar<UBowlGameModeComponent>();

        public int lastSettledCount = 10;
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            MinimalForwardLaunchVelocity = 1500;
            ForwardMultipleVelocityFactor = 1.5f;
        }

        public override void BeginPlay()
        {
            MyOwner.World.GetPlayerController(0).ShowMouseCursor = true;

            List<AActor> ballActors;
            MyOwner.World.GetAllActorsWithTag(BallTag, out ballActors);
            SetBallFromBallFindCollection(ballActors);
            myBowler = MyOwner.World.GetPlayerPawn(0).GetComponentByClass<UMyBowlPlayerComponent>();
            StandingPinCount = 10;
            BowlTurnCount = 1;
            gamemaster.OnUpdatePinCount += UpdatePinCount;
            gamemaster.BowlNewTurnIsReady += ResetPinCount;
            gamemaster.BowlTurnIsFinished += OnTurnIsFinished;
            gamemaster.OnSendBowlActionResults += OnSendBowlActionResults;
            gamemaster.Debug_Fill18ScoreSlots += Debug_Fill18ScoreSlots;

            List<AActor> bowlFloorActors;
            MyOwner.World.GetAllActorsWithTag(BowlingFloorTag, out bowlFloorActors);
            if (bowlFloorActors[0] != null)
            {
                var _staticActor = bowlFloorActors[0].Cast<AStaticMeshActor>();
                if (_staticActor != null)
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

        public override void EndPlay(EEndPlayReason endPlayReason)
        {
            StopAllCoroutines();
            if (gamemaster != null)
            {
                gamemaster.OnUpdatePinCount -= UpdatePinCount;
                gamemaster.BowlNewTurnIsReady -= ResetPinCount;
                gamemaster.BowlTurnIsFinished -= OnTurnIsFinished;
                gamemaster.OnSendBowlActionResults -= OnSendBowlActionResults;
                gamemaster.Debug_Fill18ScoreSlots -= Debug_Fill18ScoreSlots;
            }
        }
        #endregion

        #region Getters
        public static UBowlGameModeComponent GetInstance(UObject worldContextObject)
        {
            var _instanceHelper = ThisInstance.Get(worldContextObject);
            if (_instanceHelper == null)
            {
                _instanceHelper = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<UBowlGameModeComponent>();
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
            List<int> _allbowlframeturns = AllBowlFrameTurns;
            for (int i = 0; i < BowlTurnCount; i++)
            {
                _bowlTurns.Add(_allbowlframeturns[i]);
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
                var _ballComp = balls[0].GetComponentByClass<UBowlingBallComponent>();
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
            UpdateBowlTurnFramesBPEvent();
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
            UpdateBowlTurnFramesBPEvent();
        }
        #endregion

        #region Handlers
        void OnSendBowlActionResults(EBowlAction _action)
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

            if (_action != EBowlAction.Tidy)
            {
                _myPlayer = ULevelSequencePlayer.CreateLevelSequencePlayer(this, ClearSweepLevelSequence, _settings, out _mySequenceActor);
            }
            else
            {
                _myPlayer = ULevelSequencePlayer.CreateLevelSequencePlayer(this, CleanUpSweepLevelSequence, _settings, out _mySequenceActor);
            }

            _myPlayer.Play();
            //TODO: Get Accurate Wait For Seconds Length, Used To Be GetLength()
            float _waitLength = 6.0f;
            WaitTillSweepingIsDone(_waitLength, _action);
        }

        void OnTurnIsFinished()
        {
            EBowlAction _action = Bowl();
            SetResultsFromFrameTurns();
            gamemaster.CallOnSendBowlActionResults(_action);
        }

        void UpdatePinCount(int _pinCount)
        {
            StandingPinCount = _pinCount;
        }

        void ResetPinCount(EBowlAction _action)
        {
            if(_action != EBowlAction.Tidy)
            {
                StandingPinCount = 10;
                lastSettledCount = 10;
            }
        }

        void Debug_Fill18ScoreSlots()
        {
            Random _random = new Random();
            int _lastMinuteYield = 0;
            while(BowlTurnCount < 19)
            {
                //Currently Set Random Value So That Two Values
                //Will Never Be Over 10
                BowlHelper(_random.Next(0, 6));
                //Used To Prevent The Game From Crashing
                if (BowlTurnCount >= 20 || _lastMinuteYield >= 50)
                {
                    MyOwner.PrintString("Breaking Out Of Loop, No Crashing!", FLinearColor.Red);
                    break;
                }
                _lastMinuteYield++;
                SetResultsFromFrameTurns();
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
        public int GetStandingPinCount()
        {
            return StandingPinCount;
        }

        [UFunction, BlueprintCallable]
        public void GetBowlFrameProperties(EBowlFrame bowlframe, out int bowlAProperty, out int bowlBProperty, out int bowlCProperty, out int bowlResultProperty)
        {
            bowlAProperty = 0;
            bowlBProperty = 0;
            bowlCProperty = 0;
            bowlResultProperty = 0;

            switch (bowlframe)
            {
                case EBowlFrame.Frame01:
                    bowlAProperty = Frame01_BowlA;
                    bowlBProperty = Frame01_BowlB;
                    bowlResultProperty = Frame01_Results;
                    break;
                case EBowlFrame.Frame02:
                    bowlAProperty = Frame02_BowlA;
                    bowlBProperty = Frame02_BowlB;
                    bowlResultProperty = Frame02_Results;
                    break;
                case EBowlFrame.Frame03:
                    bowlAProperty = Frame03_BowlA;
                    bowlBProperty = Frame03_BowlB;
                    bowlResultProperty = Frame03_Results;
                    break;
                case EBowlFrame.Frame04:
                    bowlAProperty = Frame04_BowlA;
                    bowlBProperty = Frame04_BowlB;
                    bowlResultProperty = Frame04_Results;
                    break;
                case EBowlFrame.Frame05:
                    bowlAProperty = Frame05_BowlA;
                    bowlBProperty = Frame05_BowlB;
                    bowlResultProperty = Frame05_Results;
                    break;
                case EBowlFrame.Frame06:
                    bowlAProperty = Frame06_BowlA;
                    bowlBProperty = Frame06_BowlB;
                    bowlResultProperty = Frame06_Results;
                    break;
                case EBowlFrame.Frame07:
                    bowlAProperty = Frame07_BowlA;
                    bowlBProperty = Frame07_BowlB;
                    bowlResultProperty = Frame07_Results;
                    break;
                case EBowlFrame.Frame08:
                    bowlAProperty = Frame08_BowlA;
                    bowlBProperty = Frame08_BowlB;
                    bowlResultProperty = Frame08_Results;
                    break;
                case EBowlFrame.Frame09:
                    bowlAProperty = Frame09_BowlA;
                    bowlBProperty = Frame09_BowlB;
                    bowlResultProperty = Frame09_Results;
                    break;
                case EBowlFrame.Frame10:
                    bowlAProperty = Frame10_BowlA;
                    bowlBProperty = Frame10_BowlB;
                    bowlCProperty = Frame10_BowlC;
                    bowlResultProperty = Frame10_Results;
                    break;
                default:
                    break;
            }
        }

        [UFunction, BlueprintCallable]
        public int GetBowlTurnCount()
        {
            return BowlTurnCount;
        }
        #endregion

        #region BlueprintImplementedEvents
        [UFunction, BlueprintImplementableEvent]
        public void UpdatePinCountBPEvent(int pinCount)
        {

        }

        [UFunction, BlueprintImplementableEvent]
        public void UpdateBowlTurnFramesBPEvent()
        {

        }
        #endregion

        #region SweepingAnimationWaitCalls
        public void WaitTillSweepingIsDone(float _animLength, EBowlAction _action)
        {
            StartCoroutine(WaitTillSweepingIsDoneCoroutine(_animLength, _action));
        }

        private IEnumerator WaitTillSweepingIsDoneCoroutine(float _animLength, EBowlAction _action)
        {
            yield return new WaitForSeconds(_animLength);
            CallNewTurnIsReadyAfterWaiting(_action);
        }

        void CallNewTurnIsReadyAfterWaiting(EBowlAction _action)
        {
            gamemaster.CallBowlNewTurnIsReady(_action);
        }
        #endregion

        #region Bowling
        [UFunctionIgnore]
        public EBowlAction Bowl()
        {
            int _pinFall = GetPinFallCount();
            lastSettledCount = StandingPinCount;
            return BowlHelper(_pinFall);
        }

        /// <summary>
        /// Used For Helping Bowl Method And Generating Fake Bowl Results
        /// </summary>
        /// <param name="_pinFall"></param>
        /// <returns></returns>
        [UFunctionIgnore]
        public EBowlAction BowlHelper(int _pinFall)
        {
            //Old Bowl Method
            SetCurrentBowlTurnValue(_pinFall);
            List<int> _rolls = GetBowlTurnListFromCount();
            EBowlAction _action = BowlActionMaster.NextAction(_rolls);
            //Old Bowl Method End
            if (BowlTurnCount >= 21 || _action == EBowlAction.EndGame)
            {
                MyOwner.PrintString("Won Game From C#", FLinearColor.Green, printToLog: true);
                gamemaster.CallOnWinGame();
            }
            else if (BowlTurnCount >= 19)
            {
                BowlTurnCount += 1;
            }
            //If Action Is Tidy Or Bowlturn is the Second One.
            //Second Turns Are Even Except For the Last Few Turns.
            else if (_action == EBowlAction.Tidy ||
                BowlTurnCount % 2 == 0)
            {
                BowlTurnCount += 1;
            }
            //If Bowl Turn Count Is Not the Second One.
            //Second Turns Are Even Except For the Last Few Turns.
            else if (BowlTurnCount % 2 != 0)
            {
                //Most Likely End Of Turn
                BowlTurnCount += 2;
            }
            return _action;
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
