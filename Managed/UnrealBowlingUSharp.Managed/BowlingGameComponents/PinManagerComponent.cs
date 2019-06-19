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
    public class UPinManagerComponent : UActorComponent
    {
        #region IgnoreProperties
        [UPropertyIgnore]
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

        [UPropertyIgnore]
        protected UBowlGameMasterComponent gamemaster => UBowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIgnore]
        protected UBowlGameModeComponent gamemode => UBowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region UProperties
    
        #endregion

        #region Fields
        protected static WorldStaticVar<UPinManagerComponent> ThisInstance = new WorldStaticVar<UPinManagerComponent>();
        protected List<FVector> PinLocations = new List<FVector>();
        //[UProperty, EditAnywhere, BlueprintReadWrite, Category("Pin Management")]
        private UClass PinPrefabClass = null;
        private Dictionary<string, bool> AllPinsStandingDictionary = new Dictionary<string, bool>();
        #endregion

        #region Getter
        List<AActor> GetAllPins()
        {
            List<AActor> outPinActors;
            UGameplayStatics.GetAllActorsWithTag(MyOwner, gamemode.PinTag, out outPinActors);
            return outPinActors;
        }

        public static UPinManagerComponent GetInstance(UObject worldContextObject)
        {
            var _instanceHelper = ThisInstance.Get(worldContextObject);
            if (_instanceHelper == null)
            {
                var _gamemode = UBowlGameModeComponent.GetInstance(worldContextObject);
                if (_gamemode != null)
                {
                    List<AActor> sweepActors;
                    UGameplayStatics.GetAllActorsWithTag(worldContextObject, _gamemode.PinManagerTag, out sweepActors);
                    if (sweepActors[0] != null)
                    {
                        _instanceHelper = sweepActors[0].GetComponentByClass<UPinManagerComponent>();
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

        public override void BeginPlay()
        {
            List<AActor> outPinActors = GetAllPins();
            if (outPinActors != null && outPinActors.Count > 0 &&
                outPinActors[0] != null)
            {
                PinPrefabClass = outPinActors[0].GetClass();
            }

            foreach (var _pin in outPinActors)
            {
                //AttachPinToManager(_pin);
                PinLocations.Add(_pin.GetActorLocation());
            }

            InitializePinStandingDictionary(outPinActors);

            gamemaster.BowlNewTurnIsReady += BowlNewTurnIsReady;
            gamemaster.OnPinHasFallen += PinHasFallen;
            gamemaster.OnPinHasGottenBackUp += PinGottenBackUp;
        }

        public override void EndPlay(EEndPlayReason endPlayReason)
        {
            if (gamemaster != null)
            {
                gamemaster.BowlNewTurnIsReady -= BowlNewTurnIsReady;
                gamemaster.OnPinHasFallen -= PinHasFallen;
                gamemaster.OnPinHasGottenBackUp -= PinGottenBackUp;
            }
        }
        #endregion

        #region Handlers
        void PinHasFallen(UBowlingPinComponent _pin)
        {
            UpdatePinHasStandingDictionary(_pin, true);
        }

        void PinGottenBackUp(UBowlingPinComponent _pin)
        {
            UpdatePinHasStandingDictionary(_pin, false);
        }

        void BowlNewTurnIsReady(EBowlAction _action)
        {
            if(_action != EBowlAction.Tidy)
            {
                List<AActor> _outPins = RespawnPins();
                InitializePinStandingDictionary(_outPins);
            }
        }
        #endregion

        #region Spawn-Attach-Pins
        public List<AActor> RespawnPins()
        {
            List<AActor> _outPins = new List<AActor>();
            if (PinPrefabClass == null)
            {
                MyOwner.PrintString("No PinPrefab On Pin Manager BP", FLinearColor.Red, printToLog: true);
                return null;
            }

            foreach (var _pinLocation in PinLocations)
            {
                _outPins.Add(SpawnPin(_pinLocation));
            }

            return _outPins;
        }

        AActor SpawnPin(FVector _pinLocation)
        {
            FRotator _pinRot = FRotator.ZeroRotator;
            return MyOwner.World.SpawnActor(PinPrefabClass, ref _pinLocation, ref _pinRot);
        }

        void AttachPinToManager(AActor _pin)
        {
            _pin.AttachToActor(MyOwner, new FName("None"),
                EAttachmentRule.KeepWorld, EAttachmentRule.KeepWorld,
                EAttachmentRule.KeepWorld, true);
        }
        #endregion

        #region PinFallenDictionaryHandling
        void InitializePinStandingDictionary(List<AActor> pinActors)
        {
            AllPinsStandingDictionary = new Dictionary<string, bool>();
            foreach (AActor _pin in pinActors)
            {
                if(_pin != null)
                {
                    AllPinsStandingDictionary.Add(_pin.GetName(), true);
                }
            }
        }

        void UpdatePinHasStandingDictionary(UBowlingPinComponent _pin, bool _fallen)
        {
            string _key = _pin.MyOwner.GetName();
            if (AllPinsStandingDictionary.ContainsKey(_key))
            {
                AllPinsStandingDictionary[_key] = !_fallen;
                int _pinStandingCount = 0;
                foreach (bool _pinStanding in AllPinsStandingDictionary.Values)
                {
                    if (_pinStanding) _pinStandingCount++;
                }
                gamemaster.CallOnUpdatePinCount(_pinStandingCount);
            }
        }
        #endregion
    }
}
