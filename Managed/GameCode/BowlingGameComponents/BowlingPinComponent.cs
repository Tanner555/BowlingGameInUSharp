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
        #endregion

        #region UProperties
        [UProperty, EditAnywhere, BlueprintReadWrite, Category("Bowling")]
        public UStaticMeshComponent MyColliderMeshComponent { get; set; }

        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            if(pinManager != null)
            {
                var _pinManagerBP = pinManager.MyOwner;
                if(MyColliderMeshComponent != null)
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

            gamemaster.OnSendBowlActionResults += OnSendBowlActionResults;
            gamemaster.BowlNewTurnIsReady += NewBowlTurnHasStarted;
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
                
            }
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            //MyOwner.DetachFromActor(EDetachmentRule.KeepWorld, EDetachmentRule.KeepWorld, EDetachmentRule.KeepWorld);
            //GetOwner().PrintString("Ending Play", FLinearColor.Green, printToLog: true);
            if(gamemaster != null)
            {
                gamemaster.OnSendBowlActionResults -= OnSendBowlActionResults;
                gamemaster.BowlNewTurnIsReady -= NewBowlTurnHasStarted;
                //Pin should have fallen
                //Checking Just In Case
                if(bPinHasFallen == false)
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
        #endregion

        #region Handlers
        void OnSendBowlActionResults(BowlAction _action)
        {
            var _pinManager = pinManager;
            //Only If Collider Mesh Comp Has Been Assigned AND
            //Parent Actor is the PinManager Actor Blueprint
            if (MyColliderMeshComponent != null &&
                _pinManager != null &&
                (MyOwner.GetParentActor() == _pinManager.MyOwner && bPinHasFallen == false))
            {
                if (_action == BowlAction.Tidy)
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

        void NewBowlTurnHasStarted(bool _roundIsOver, BowlAction _action)
        {
            if (MyColliderMeshComponent != null)
            {
                MyColliderMeshComponent.SetSimulatePhysics(true);
            }
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
            bPinHasFallen = _tiltInX > standingThreshold && _tiltInY > standingThreshold;
            if (bPinHasFallen)
            {
                gamemaster.CallOnPinHasFallen(this);
            }
            return bPinHasFallen;
        }
        #endregion

        #region OtherMethods
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
