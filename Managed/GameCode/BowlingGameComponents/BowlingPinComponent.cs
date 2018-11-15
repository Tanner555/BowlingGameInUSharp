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
using System.Collections;

namespace HelloUSharp
{
    [UClass, Blueprintable, BlueprintType]
    public class BowlingPinComponent : UActorComponent
    {
        #region UPropertyIgnore
        [UPropertyIngore]
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected PinManagerComponent pinManager => PinManagerComponent.GetInstance(MyOwner);

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
        public UAudioComponent MyAudioSourceComponent { get; set; }
        #endregion

        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public UStaticMeshComponent MyColliderMeshComponent { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase PinStrikeSoundVolume1 { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase PinStrikeSoundVolume2 { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase PinStrikeSoundVolume3 { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase PinStrikeSoundVolume4 { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Initialization")]
        public USoundBase PinStrikeSoundVolume5 { get; set; }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            gamemaster.BowlTurnIsFinished += OnTurnIsFinished;
            gamemaster.OnWinGame += OnTurnIsFinished;
            gamemaster.OnSendBowlActionResults += OnSendBowlActionResults;
            gamemaster.BowlNewTurnIsReady += NewBowlTurnHasStarted;
            gamemaster.Debug_OnSimulateStrike += OnSimulateStrike;
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
        }

        [UFunction, BlueprintCallable]
        protected void ReceiveHitWrapper(UPrimitiveComponent MyComp, AActor Other, UPrimitiveComponent OtherComp, bool SelfMoved, FVector HitLocation, FVector HitNormal, FVector NormalImpulse, FHitResult Hit)
        {
            if (Other != null && Other.ActorHasTag(gamemode.BallTag))
            {
                if(bHitFirstPin.Get(this) == false)
                {
                    bHitFirstPin.Set(this, true);
                    if (MyAudioSourceComponent == null)
                    {
                        MyOwner.PrintString("Please Assign an audio component to the uproperty", FLinearColor.OrangeRed);
                    }
                    else if (PinStrikeSoundVolume1 == null)
                    {
                        MyOwner.PrintString("Please Assign a sound clip to the PinStrikeSoundVolume1 sound uproperty", FLinearColor.OrangeRed);
                    }
                    else if (PinStrikeSoundVolume2 == null)
                    {
                        MyOwner.PrintString("Please Assign a sound clip to the PinStrikeSoundVolume2 sound uproperty", FLinearColor.OrangeRed);
                    }
                    else if (PinStrikeSoundVolume3 == null)
                    {
                        MyOwner.PrintString("Please Assign a sound clip to the PinStrikeSoundVolume3 sound uproperty", FLinearColor.OrangeRed);
                    }
                    else if (PinStrikeSoundVolume4 == null)
                    {
                        MyOwner.PrintString("Please Assign a sound clip to the PinStrikeSoundVolume4 sound uproperty", FLinearColor.OrangeRed);
                    }
                    else if (PinStrikeSoundVolume5 == null)
                    {
                        MyOwner.PrintString("Please Assign a sound clip to the PinStrikeSoundVolume5 sound uproperty", FLinearColor.OrangeRed);
                    }
                    else
                    {
                        StartCoroutine(this, WaitForBallToHitPins(pinStrikeSoundWaitTime, Other.GetVelocity(), Other));
                    }
                }
            }
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            StopAllCoroutines();
            //MyOwner.DetachFromActor(EDetachmentRule.KeepWorld, EDetachmentRule.KeepWorld, EDetachmentRule.KeepWorld);
            //GetOwner().PrintString("Ending Play", FLinearColor.Green, printToLog: true);
            if(gamemaster != null)
            {
                gamemaster.BowlTurnIsFinished -= OnTurnIsFinished;
                gamemaster.OnWinGame -= OnTurnIsFinished;
                gamemaster.OnSendBowlActionResults -= OnSendBowlActionResults;
                gamemaster.BowlNewTurnIsReady -= NewBowlTurnHasStarted;
                gamemaster.Debug_OnSimulateStrike -= OnSimulateStrike;
                //Pin should have fallen
                //Checking Just In Case
                if (bPinHasFallen == false)
                {
                    bPinHasFallen = true;
                    gamemaster.CallOnPinHasFallen(this);
                }
            }
        }
        #endregion

        #region Fields
        bool bPinHasFallen = false;
        float standingThreshold = 15f;
        bool bDebugInstantStrike = false;

        protected static WorldStaticVar<bool> bHitFirstPin = new WorldStaticVar<bool>();
        float pinStrikeSoundWaitTime = 0.5f;
        #endregion

        #region Handlers
        /// <summary>
        /// Also Called When Won Game 
        /// </summary>
        void OnTurnIsFinished()
        {
            StopAllCoroutines();
        }

        void OnSendBowlActionResults(EBowlAction _action)
        {
            var _pinManager = pinManager;
            //Only If Collider Mesh Comp Has Been Assigned AND
            //Parent Actor is the PinManager Actor Blueprint
            if (MyColliderMeshComponent != null &&
                _pinManager != null &&
                bPinHasFallen == false)
            {
                if (_action == EBowlAction.Tidy)
                {
                    AttachToParentWithOldPosition();
                    MyColliderMeshComponent.SetSimulatePhysics(false);
                }
                else
                {
                    MyColliderMeshComponent.SetSimulatePhysics(true);
                }
            }
        }

        void NewBowlTurnHasStarted(EBowlAction _action)
        {
            if(bHitFirstPin.Get(this) == true)
            {
                bHitFirstPin.Set(this, false);
            }

            if (bPinHasFallen)
            {
                //Destroy Pin If It Hasn't Been Sweeped Into the Floor
                MyOwner.DestroyActor();
            }
            else if (MyColliderMeshComponent != null)
            {
                MyColliderMeshComponent.SetSimulatePhysics(true);
            }
        }

        //Debug
        void OnSimulateStrike()
        {
            bDebugInstantStrike = true;
        }
        #endregion

        #region PublicMethodCalls
        [UFunction, BlueprintCallable]
        public bool SE_CheckForPinHasFallen()
        {
            //GetOwner().PrintString("SE_CheckForPinHasFallen", FLinearColor.Green, printToLog: true);
            FVector _rotationInEuler = MyOwner.GetActorRotation().Euler();
            float _tiltInX = FMath.Abs(_rotationInEuler.X);
            float _tiltInY = FMath.Abs(_rotationInEuler.Y);
            bool _previouslyFallen = bPinHasFallen;
            bPinHasFallen = _tiltInX > standingThreshold || _tiltInY > standingThreshold;

            if (bDebugInstantStrike)
            {
                bPinHasFallen = true;
            }

            if (bPinHasFallen && _previouslyFallen != bPinHasFallen)
            {
                gamemaster.CallOnPinHasFallen(this);
            }
            else if(bPinHasFallen == false && 
                _previouslyFallen != bPinHasFallen &&
                bDebugInstantStrike == false)
            {
                //If Pin Has Gotten Back Up Because
                //Pin Has Fallen, But Now PinHasFallen Equals False
                //MyOwner.PrintString("Pin has gotten back up", FLinearColor.Green, printToLog: true);
                gamemaster.CallOnPinHasGottenBackUp(this);
            }

            return bPinHasFallen;
        }
        #endregion

        #region Initialization
        [UFunction, BlueprintCallable]
        public void MyBeginPlayInitializer(UStaticMeshComponent _collidermesh, UAudioComponent _uaudiocomponent)
        {
            MyColliderMeshComponent = _collidermesh;
            MyAudioSourceComponent = _uaudiocomponent;
            MyBeginPlayPostInitialization();
        }

        /// <summary>
        /// Took Part Of BeginPlay Code Out Because It Depends On Initialization
        /// Which Should Be Called On Begin Play Inside Blueprints
        /// </summary>
        private void MyBeginPlayPostInitialization()
        {
            if (pinManager != null)
            {
                var _pinManagerBP = pinManager.MyOwner;
                if (MyColliderMeshComponent != null)
                {
                    AttachToParentWithOldPosition();
                    MyColliderMeshComponent.SetSimulatePhysics(false);
                    MyColliderMeshComponent.SetSimulatePhysics(true);
                }
                else
                {
                    MyOwner.PrintString("Please Attach Collider Mesh To Pin Comp UProperty", FLinearColor.Red, printToLog: true);
                }
            }
            else
            {
                GetOwner().PrintString("Couldn't Find Pin Manager Component", FLinearColor.Red, printToLog: true);
            }
        }
        #endregion

        #region OtherMethods
        IEnumerator WaitForBallToHitPins(float _waitLength, FVector _velocity, AActor _other)
        {
            yield return new WaitForSeconds(_waitLength);
            PlayPinStrikeSounds(_velocity, _other);
        }

        void PlayPinStrikeSounds(FVector _velocity, AActor _other)
        {
            MyOwner.PrintString("Ready To Pin Strike With Velocity: " + _velocity.ToString(), FLinearColor.Green, printToLog: true);
            MyAudioSourceComponent.Sound = PinStrikeSoundVolume4;
            MyAudioSourceComponent.Play();
        }

        void AttachToParentWithOldPosition()
        {
            var _pinManagerBP = pinManager.MyOwner;
            FHitResult _hit;
            FVector _oldLoc = MyOwner.GetActorLocation();
            FRotator _oldRot = MyOwner.GetActorRotation();

            MyOwner.AttachToActor(_pinManagerBP, _pinManagerBP.GetAttachParentSocketName(),
                EAttachmentRule.KeepRelative, EAttachmentRule.KeepRelative, EAttachmentRule.KeepWorld, true);
            MyOwner.SetActorLocation(_oldLoc, false, out _hit, false);
            MyOwner.SetActorRotation(_oldRot, false);
        }
        #endregion
    }
}
