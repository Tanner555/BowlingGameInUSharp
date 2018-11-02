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
    class MyBowlPlayerComponent : UActorComponent
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
        [UPropertyIngore]
        protected float PawnStartXPoint { get; set; }
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
        #endregion

        #region Fields
        public BowlingBallComponent myBall = null;
        protected bool bShouldFollowBall = false;
        private FHitResult myHit;
        #endregion

        #region Testing
        [UFunction, BlueprintCallable]
        public string GetMyMessage()
        {
            return "Hello There From MyPawnClass123455";
        }
        #endregion

        #region Overrides
        protected override void ReceiveBeginPlay_Implementation()
        {
            PawnStartXPoint = MyOwner.GetActorLocation().X;
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
        #endregion

        [UFunction, BlueprintCallable]
        public void OnDragStart(FVector2D mousePos)
        {
            var _gamemode = MyOwner.World.GetGameMode().GetComponentByClass<BowlGameModeComponent>();
            if (_gamemode != null)
            {
                _gamemode.OnStartDrag(mousePos);
            }
            
        }

        [UFunction, BlueprintCallable]
        public void OnDragStop(FVector2D mousePos)
        {
            var _gamemode = MyOwner.World.GetGameMode().GetComponentByClass<BowlGameModeComponent>();
            if (_gamemode != null)
            {
                _gamemode.OnStopDrag(mousePos);
            }
        }

        public void StartFollowingBall(BowlingBallComponent _ball)
        {
            myBall = _ball;
            DefaultBallFollowOffset = MyOwner.GetActorLocation().X - myBall.MyOwner.GetActorLocation().X;
            if (myBall != null)
            {
                bShouldFollowBall = true;
            }
            else
            {
                bShouldFollowBall = false;
            }
        }
    }
}
