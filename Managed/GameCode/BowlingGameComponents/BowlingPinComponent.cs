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

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            if(pinManager != null)
            {
                //var _pinManagerBP = pinManager.MyOwner;
                //MyOwner.AttachToActor(_pinManagerBP, _pinManagerBP.GetAttachParentSocketName(),
                //    EAttachmentRule.KeepWorld, EAttachmentRule.KeepWorld, EAttachmentRule.KeepWorld, true);
            }
            else
            {
                GetOwner().PrintString("Couldn't Find Pin Manager Component", FLinearColor.Red, printToLog: true);
            }
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

        }
        #endregion

        #region Fields
        bool bPinHasFallen = false;
        float standingThreshold = 3f;
        #endregion

        #region PublicMethodCalls
        [UFunction, BlueprintCallable]
        public void SE_CheckForPinHasFallen()
        {
            //GetOwner().PrintString("SE_CheckForPinHasFallen", FLinearColor.Green, printToLog: true);
            FVector _rotationInEuler = MyOwner.GetActorRotation().Euler();
            float _tiltInX = FMath.Abs(_rotationInEuler.X);
            float _tiltInY = FMath.Abs(_rotationInEuler.Y);
            bPinHasFallen = _tiltInX < standingThreshold && _tiltInY < standingThreshold;
        }

        [UFunction, BlueprintCallable]
        public bool PinHasFallen()
        {
            return bPinHasFallen;
        }
        #endregion
    }
}
