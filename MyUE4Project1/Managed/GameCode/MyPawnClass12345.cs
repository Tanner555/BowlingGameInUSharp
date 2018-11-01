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
    class MyPawnClass12345 : APawn
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public string MyString12345 { get; set; }

        [UFunction, BlueprintCallable]
        public string GetMyMessage()
        {
            return "Hello There From MyPawnClass123455";
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //PrintString("Hello From " + GetName(), FLinearColor.Red);
        }
    }
}
