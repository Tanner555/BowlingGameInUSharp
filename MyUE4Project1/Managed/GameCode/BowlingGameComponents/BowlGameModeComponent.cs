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
            if (ThisInstance == null)
            {
                ThisInstance = this;
            }
            else
            {
                MyOwner.PrintString("More than one instance of " + GetName() + " in scene", FLinearColor.Red);
            }

            List<AActor> ballActors;
            MyOwner.World.GetAllActorsWithTag(BallTag, out ballActors);
            SetBallFromBallFindCollection(ballActors);
            //SetBallFromBallFindCollection(MyOwner.World.GetAllActorsOfClassList<BowlingBallComponent>());
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {

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
    }
}
