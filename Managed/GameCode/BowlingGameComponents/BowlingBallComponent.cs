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
    public class BowlingBallComponent : UActorComponent
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
        private FVector myPos
        {
            get { return MyOwner.GetActorLocation(); }
        }

        [UPropertyIngore]
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region Fields
        private FHitResult myHit;
        #endregion

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

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize();
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //LaunchBall();
            gamemaster.BowlTurnIsFinished += BowlTurnIsFinished;
            gamemaster.OnBallLaunch += LaunchBall;
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
            //SetActorLocation(myPos + new FVector(DefaultMoveSpeed, 0, 0), false, out myHit, false);
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            if (gamemaster != null)
            {
                gamemaster.BowlTurnIsFinished -= BowlTurnIsFinished;
                gamemaster.OnBallLaunch -= LaunchBall;
            }
        }
        #endregion

        #region Handlers
        void BowlTurnIsFinished()
        {
            MyOwner.PrintString("BowlTurnIsFinishedd", FLinearColor.Green);
        }

        void LaunchBall(FVector launchVelocity, BowlingBallComponent bowlingBall)
        {
            if (MyMeshComponent == null)
            {
                MyOwner.PrintString("Please Assign A mesh component to the uproperty", FLinearColor.OrangeRed);
            }
            else if (MyAudioSourceComponent == null)
            {
                MyOwner.PrintString("Please Assign an audio component to the uproperty", FLinearColor.OrangeRed);
            }
            else if (BallRollingSound == null)
            {
                MyOwner.PrintString("Please Assign a sound clip to the ball rolling sound uproperty", FLinearColor.OrangeRed);
            }
            else
            {
                MyMeshComponent.AddImpulse(launchVelocity, MyMeshComponent.GetAttachSocketName(), true);
                MyAudioSourceComponent.Sound = BallRollingSound;
                MyAudioSourceComponent.Play();
            }
        }
        #endregion
    }
}
