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
    class MySuperPositionReport : UActorComponent
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int MyNumber123456789 { get; set; }

        [UFunction, BlueprintCallable]
        public void MyTestFunc1()
        {

        }
    }
}
