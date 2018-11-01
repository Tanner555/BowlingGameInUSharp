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
    class TestActorComponent1 : UActorComponent
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int MyNumber123456789 { get; set; }

        //protected override void ReceiveBeginPlay_Implementation()
        //{
        //    //base.ReceiveBeginPlay_Implementation();
        //    MyNumber123456789 = 90;

        //    if(GetOwner().World != null)
        //    {
        //        //GetOwner().World.PrintString("Hello From Component", true, false, FLinearColor.AliceBlue, 3f);
        //    }
        //}

        [UFunction, BlueprintCallable]
        public bool TestCastMyAddress()
        {
            UWorld _world = GetOwner().World;
            return _world != null;
        }
    }
}
