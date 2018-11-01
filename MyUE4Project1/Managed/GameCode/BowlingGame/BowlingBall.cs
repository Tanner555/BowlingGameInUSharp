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
        public float DefaultMoveSpeed { get; set; }

        [UProperty, EditDefaultsOnly, BlueprintReadWrite, Category("Bowling")]
        public UStaticMeshComponent MyMeshComponent { get; set; }
        #endregion

        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize();
            DefaultMoveSpeed = 6f;
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            if(MyMeshComponent == null)
            {
                PrintString("Please Assign A mesh component to the uproperty", FLinearColor.OrangeRed);
            }
            else
            {
                MyMeshComponent.AddImpulse(new FVector(DefaultMoveSpeed * 300, 0, 0), MyMeshComponent.GetAttachSocketName(), true);
            }
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
            //SetActorLocation(myPos + new FVector(DefaultMoveSpeed, 0, 0), false, out myHit, false);
        }
    }
}
