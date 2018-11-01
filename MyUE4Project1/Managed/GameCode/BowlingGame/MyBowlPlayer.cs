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
    class MyBowlPlayer : APawn
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public string MyString12345 { get; set; }

        //[UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        [UPropertyIngore]
        protected float PawnStartXPoint { get; set; }
        [UPropertyIngore]
        protected float CameraFollowXOffset { get; set; }

        private BowlingBall myBall = null;

        bool bShouldFollowBall = false; 

        [UFunction, BlueprintCallable]
        public string GetMyMessage()
        {
            return "Hello There From MyPawnClass123455";
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //FHitResult _hit;
            //base.ReceiveBeginPlay_Implementation();
            //PrintString("Hello From " + GetName(), FLinearColor.Red);
            //SetActorLocation(GetActorLocation() + 
            //    new FVector(2000, 0, 0),
            //    false, out _hit, false);
            PawnStartXPoint = GetActorLocation().X;
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
            if (bShouldFollowBall && myBall != null)
            {
                FHitResult _hit;
                var _myPos = GetActorLocation();
                var _ballPos = myBall.GetActorLocation();
                //SetActorLocation(
                //    _myPos +
                //    new FVector(_ballPos.X, _myPos.Y, _myPos.Z),
                //    false, out _hit, false
                //);
            }
        }

        public void StartFollowingBall(BowlingBall _ball)
        {
            myBall = _ball;
            if(myBall != null)
            {
                CameraFollowXOffset = myBall.GetActorLocation().X - PawnStartXPoint;
                bShouldFollowBall = true;
            }
            else
            {
                bShouldFollowBall = false;
            }
        }

    }
}
