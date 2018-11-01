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
    class MyActor123 : AActor
    {
        //[UProperty, EditAnywhere, BlueprintReadWrite]
        //[Category("TestActor")]
        //public UStaticMeshComponent MeshComponent { get; set; }
        //TSubclassOf

        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int Value123 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("MyCategory")]
        public string Value456 { get; set; }

        //[UProperty, EditAnywhere, BlueprintReadWrite]
        //public  Value123 { get; set; }

        [UFunction, BlueprintCallable]
        public string GetMyMessage()
        {
            //return "Hello There from " + UClass.GetClass<MyActor123>().GetName();
            //var _spawnParams = new UnrealEngine.Runtime.Native.FActorSpawnParameters()
            //{
            //    Name = new FName("Hello")
            //};
            //return "Spawn Params: " + _spawnParams.Name;
            return "Hello there";
        }

        [UFunction, BlueprintCallable]
        public bool TestCastMyAddress()
        {
            UWorld _world = this.World;
            return _world != null;
        }

        //[UFunction, BlueprintCallable]
        //public void TestPrintString(string _msg)
        //{
        //    UWorld _world = this.World;
        //    if (_world != null)
        //    {
        //        _world.PrintString(_msg, true, false, FLinearColor.AliceBlue, 3f);
        //    }
        //}

        //[UFunction, BlueprintCallable, BlueprintImplementedEvent]
        //public void PrintStringFromBlueprints(string _msg)
        //{
        //
        //}

        [UFunction, BlueprintCallable]
        public AActor SpawnAnActor()
        {
            var _location = GetActorLocation();
            var _rotation = GetActorRotation();
            
            var _spawnParams = new FActorSpawnParameters()
            {
                SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod.DontSpawnIfColliding
            };

            UWorld _world = this.World;
            if(_world != null)
            {
                var _myClass = UClass.GetClass<MyActor123>();

                return _world.SpawnActor(_myClass,
                    ref _location, ref _rotation, ref _spawnParams);
            }
            return null;
        }

        public override void Initialize(FObjectInitializer initializer)
        {
            //base.Initialize(initializer);

            //MeshComponent = initializer.CreateDefaultSubobject(this, 
            //    new FName("StaticMeshComponent0"), 
            //    UClass.GetClass<UStaticMeshComponent>(),
            //    UClass.GetClass<UStaticMeshComponent>(), false, false, false) as UStaticMeshComponent;

        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            //base.ReceiveBeginPlay_Implementation();
            //PrintStringFromBlueprints("Hello there");
            //USystemLibrary.PrintString(this, "Hello world", true, false, new FLinearColor() { R = 1, G = 0, B = 0, A = 1 }, 10);
            //PrintString("Hello world", FLinearColor.Red);
            //Value123 = 100;
            
        }

        protected override void ReceiveTick_Implementation(float DeltaSeconds)
        {
            //base.ReceiveTick_Implementation(DeltaSeconds);
            //PrintStringFromBlueprints("Hello there");
            //Console.WriteLine("Hello there");
            
        }
    }
}
