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
    class BowlGameMode : AGameMode
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int Value12345 { get; set; }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //PrintString("Hello From " + GetName(), FLinearColor.Green);
        }
    }
}
