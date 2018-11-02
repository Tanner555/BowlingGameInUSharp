using System;
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
    class BowlGameModeComponent : UActorComponent
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
        public FName BallTag { get { return new FName("Ball"); } }

        [UPropertyIngore]
        public static BowlGameModeComponent ThisInstance { get; protected set; }
        #endregion

        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int Value12345 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public TSubclassOf<AActor> BowlingBallSubClassReference { get; set; }
        #endregion

        #region Fields
        protected APlayerCameraManager myCameraManager = null;
        protected BowlingBallComponent myBall = null;
        protected MyBowlPlayerComponent myBowler = null;

        private FVector2D dragStart, dragEnd;
        private float startTime, endTime;
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //This instance doesn't get destroyed when game ends
            //So Don't Check For An Existing Instance
            ThisInstance = this;

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
            //base.ReceiveEndPlay_Implementation(EndPlayReason);
            ThisInstance = null;
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
            StartLaunchingTheBall(_launchVelocity);
        }

        [UFunction, BlueprintCallable]
        public void StartLaunchingTheBall(FVector launchVelocity)
        {
            var _bowlPlayer = MyOwner.World.GetPlayerPawn(0).GetComponentByClass<MyBowlPlayerComponent>();
            if (myBall != null && _bowlPlayer != null)
            {
                MyOwner.PrintString("Launching Ball", FLinearColor.AliceBlue, printToLog: true);
                myBall.LaunchBall(launchVelocity);
                _bowlPlayer.StartFollowingBall(myBall);
            }
        }
        #endregion

        #region NudgeBall
        [UFunction, BlueprintCallable]
        public void NudgeBallLeft()
        {
            FHitResult _hit;
            if(myBall != null)
            {
                myBall.MyOwner.SetActorLocation(
                    myBall.MyOwner.GetActorLocation() +
                    new FVector(0, -50, 0), false, out _hit, false);
            }
            if(myBowler != null)
            {
                myBowler.MyOwner.SetActorLocation(
                    myBowler.MyOwner.GetActorLocation() +
                    new FVector(0, -50, 0), false, out _hit, false);
            }
        }

        [UFunction, BlueprintCallable]
        public void NudgeBallRight()
        {
            FHitResult _hit;
            if (myBall != null)
            {
                myBall.MyOwner.SetActorLocation(
                    myBall.MyOwner.GetActorLocation() +
                    new FVector(0, 50, 0), false, out _hit, false);
            }
            if (myBowler != null)
            {
                myBowler.MyOwner.SetActorLocation(
                    myBowler.MyOwner.GetActorLocation() +
                    new FVector(0, 50, 0), false, out _hit, false);
            }
        }
        #endregion
    }
}
