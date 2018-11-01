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
    class BowlingBall : AActor
    {
        [UPropertyIngore]
        private FVector myPos
        {
            get { return GetActorLocation(); }
        }

        private FHitResult myHit;

        #region MyUProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public FVector LaunchVelocity { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public UStaticMeshComponent MyMeshComponent { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public UAudioComponent MyAudioSourceComponent { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase BallRollingSound { get; set; }
        #endregion

        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize();
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //LaunchBall();
        }

        [UFunction, BlueprintCallable]
        public void LaunchBall(FVector launchVelocity)
        {
            if (MyMeshComponent == null)
            {
                PrintString("Please Assign A mesh component to the uproperty", FLinearColor.OrangeRed);
            }
            else if(MyAudioSourceComponent == null)
            {
                PrintString("Please Assign an audio component to the uproperty", FLinearColor.OrangeRed);
            }
            else if(BallRollingSound == null)
            {
                PrintString("Please Assign a sound clip to the ball rolling sound uproperty", FLinearColor.OrangeRed);
            }
            else
            {
                MyMeshComponent.AddImpulse(launchVelocity, MyMeshComponent.GetAttachSocketName(), true);
                MyAudioSourceComponent.Sound = BallRollingSound;
                MyAudioSourceComponent.Play();
            }
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
            //SetActorLocation(myPos + new FVector(DefaultMoveSpeed, 0, 0), false, out myHit, false);
        }
    }
}
