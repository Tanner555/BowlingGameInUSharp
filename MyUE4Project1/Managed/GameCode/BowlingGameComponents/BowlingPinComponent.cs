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
    class BowlingPinComponent : UActorComponent
    {
        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //GetOwner().PrintString("Reporting For Duty", FLinearColor.Green, printToLog: true);
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
        }

        [UFunction, BlueprintCallable]
        protected void ReceiveHitWrapper(UPrimitiveComponent MyComp, AActor Other, UPrimitiveComponent OtherComp, bool SelfMoved, FVector HitLocation, FVector HitNormal, FVector NormalImpulse, FHitResult Hit)
        {
            //TODO: Consider Using Tags Instead of Getting Component
            if (Other != null && Other.GetComponentByClass<BowlingBallComponent>() != null)
            {
                GetOwner().PrintString("I was hit by " + Other.GetName(), FLinearColor.Green, printToLog: true);
            }
        }
        #endregion

    }
}
