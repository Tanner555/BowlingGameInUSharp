using System;
using System.Collections;
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
    public class PinManagerComponent : UActorComponent
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

        #region UProperties
        //[UProperty, EditAnywhere, BlueprintReadWrite, Category("Pin Management")]
        private AActor PinPrefab;
        #endregion

        #region Fields
        protected static WorldStaticVar<PinManagerComponent> ThisInstance = new WorldStaticVar<PinManagerComponent>();
        protected List<FVector> PinLocations = new List<FVector>();
        #endregion

        #region Getter
        public static PinManagerComponent GetInstance(UObject worldContextObject)
        {
            var _instanceHelper = ThisInstance.Get(worldContextObject);
            if (_instanceHelper == null)
            {
                var _gamemode = BowlGameModeComponent.GetInstance(worldContextObject);
                if (_gamemode != null)
                {
                    List<AActor> sweepActors;
                    UGameplayStatics.GetAllActorsWithTag(worldContextObject, _gamemode.PinManagerTag, out sweepActors);
                    if (sweepActors[0] != null)
                    {
                        _instanceHelper = sweepActors[0].GetComponentByClass<PinManagerComponent>();
                        ThisInstance.Set(worldContextObject, _instanceHelper);
                    }
                }
            }
            return _instanceHelper;
        }
        #endregion

        #region Overrides
        public override void Initialize(FObjectInitializer initializer)
        {
            
        }

        protected override void ReceiveBeginPlay_Implementation()
        {
            List<AActor> outPinActors;
            UGameplayStatics.GetAllActorsWithTag(MyOwner, gamemode.PinTag, out outPinActors);
            if(outPinActors != null && outPinActors.Count > 0 &&
                outPinActors[0] != null)
            {
                PinPrefab = outPinActors[0];
            }

            foreach (var _pin in outPinActors)
            {
                //AttachPinToManager(_pin);
                PinLocations.Add(_pin.GetActorLocation());
            }
            gamemaster.BowlNewTurnIsReady += BowlNewTurnIsReady;
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            if(gamemaster != null)
            {
                gamemaster.BowlNewTurnIsReady -= BowlNewTurnIsReady;
            }
        }
        #endregion

        #region Handlers
        void BowlNewTurnIsReady(bool _isRoundOver, BowlAction _action)
        {
            if(_action != BowlAction.Tidy)
            {
                RespawnPins();
            }
        }
        #endregion

        #region Spawn-Attach-Pins
        public void RespawnPins()
        {
            if (PinPrefab == null)
            {
                MyOwner.PrintString("No PinPrefab On Pin Manager BP", FLinearColor.Red, printToLog: true);
                return;
            }

            foreach (var _pinLocation in PinLocations)
            {
                SpawnPin(_pinLocation);
            }
        }

        AActor SpawnPin(FVector _pinLocation)
        {
            FRotator _pinRot = FRotator.ZeroRotator;
            return MyOwner.World.SpawnActor(PinPrefab.GetClass(), ref _pinLocation, ref _pinRot);
        }

        void AttachPinToManager(AActor _pin)
        {
            _pin.AttachToActor(MyOwner, new FName("None"),
                EAttachmentRule.KeepWorld, EAttachmentRule.KeepWorld,
                EAttachmentRule.KeepWorld, true);
        }
        #endregion
    }
}
