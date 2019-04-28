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
        public UStaticMeshComponent MyMeshComponent { get; set; }

        [UPropertyIngore]
        public UAudioComponent MyAudioSourceComponent { get; set; }

        [UPropertyIngore]
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region Fields
        private FHitResult myHit;
        private FVector MyStartLocation;
        private FRotator MyStartRotation;
        #endregion

        #region MyUProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public FVector LaunchVelocity { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase BallRollingSound { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase BallNudgeSound { get; set; }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize();
        }

        public override void BeginPlay()
        {
            //base.ReceiveBeginPlay_Implementation();
            //LaunchBall();
            gamemaster.BowlTurnIsFinished += BowlTurnIsFinished;
            gamemaster.BowlNewTurnIsReady += NewTurnIsReady;
            //gamemaster.BowlTurnIsFinishedTest.Bind(BowlTurnIsFinished);
            gamemaster.OnBallLaunch += LaunchBall;
            gamemaster.OnNudgeBallLeft += NudgeBallLeft;
            gamemaster.OnNudgeBallRight += NudgeBallRight;

            MyStartLocation = MyOwner.GetActorLocation();
            MyStartRotation = MyOwner.GetActorRotation();
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
            //SetActorLocation(myPos + new FVector(DefaultMoveSpeed, 0, 0), false, out myHit, false);
        }

        public override void EndPlay(EEndPlayReason endPlayReason)
        {
            if (gamemaster != null)
            {
                gamemaster.BowlTurnIsFinished -= BowlTurnIsFinished;
                gamemaster.BowlNewTurnIsReady -= NewTurnIsReady;
                //gamemaster.BowlTurnIsFinishedTest.Unbind(BowlTurnIsFinished);
                gamemaster.OnBallLaunch -= LaunchBall;
                gamemaster.OnNudgeBallLeft -= NudgeBallLeft;
                gamemaster.OnNudgeBallRight -= NudgeBallRight;
            }
        }
        #endregion

        #region Handlers
        void NewTurnIsReady(EBowlAction _action)
        {
            if (MyMeshComponent == null) return;

            MyOwner.SetActorLocation(
                MyStartLocation, false, out myHit, false
                );
            MyOwner.SetActorRotation(
                MyStartRotation, false
                );

            MyMeshComponent.SetSimulatePhysics(false);
            MyMeshComponent.SetSimulatePhysics(true);
        }

        //Only Public Because I was testing Delegate Binding
        [UFunction, BlueprintCallable]
        public void BowlTurnIsFinished()
        {
            //MyOwner.PrintString("BowlTurnIsFinishedd", FLinearColor.Green);
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

        void NudgeBallLeft(float famount)
        {
            FHitResult _hit;
            MyOwner.SetActorLocation(
                MyOwner.GetActorLocation() +
                new FVector(0, famount, 0), false, out _hit, false);

            if (BallNudgeSound != null)
            {
                MyAudioSourceComponent.Sound = BallNudgeSound;
                MyAudioSourceComponent.Play();
            }
        }

        void NudgeBallRight(float famount)
        {
            FHitResult _hit;
            MyOwner.SetActorLocation(
                MyOwner.GetActorLocation() +
                new FVector(0, famount, 0), false, out _hit, false);

            if (BallNudgeSound != null)
            {
                MyAudioSourceComponent.Sound = BallNudgeSound;
                MyAudioSourceComponent.Play();
            }
        }
        #endregion

        #region Initialization
        [UFunction, BlueprintCallable]
        public void MyBeginPlayInitializer(UStaticMeshComponent _mymeshcomponent, UAudioComponent _myaudiosourcecomponent)
        {
            MyMeshComponent = _mymeshcomponent;
            MyAudioSourceComponent = _myaudiosourcecomponent;
        }
        #endregion
    }
}
