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
    public class BowlingFloorCaptureTriggerComponent : UActorComponent
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
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region Overrides
        protected override void ReceiveBeginPlay_Implementation()
        {
            base.ReceiveBeginPlay_Implementation();
        }
        #endregion

        [UFunction, BlueprintCallable]
        public void OnBeginOverlapWrapper(UPrimitiveComponent OverlappedComp, AActor OtherActor, UPrimitiveComponent OtherComp, int OtherBodyIndex, bool bFromSweep, FHitResult SweepResult)
        {
            if (OtherActor != null)
            {
                MyOwner.PrintString("Other Actor: " + OtherActor.GetName(), FLinearColor.Green);
                if (OtherActor.ActorHasTag(gamemode.BallTag))
                {

                }
                else if (OtherActor.ActorHasTag(gamemode.PinTag))
                {

                }
            }
        }
    }
}
