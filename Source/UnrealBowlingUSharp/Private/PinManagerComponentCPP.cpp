// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/PinManagerComponentCPP.h"

#include "BowlingPinComponentCPP.h"

#pragma region InitAndOverrides
// Sets default values for this component's properties
UPinManagerComponentCPP::UPinManagerComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UPinManagerComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	// ...
	// List<AActor> outPinActors = GetAllPins();
	// if (outPinActors != null && outPinActors.Count > 0 &&
 //        outPinActors[0] != null)
	// {
	// 	PinPrefabClass = outPinActors[0].GetClass();
	// }
 //
	// foreach (var _pin in outPinActors)
	// {
	// 	//AttachPinToManager(_pin);
	// 	PinLocations.Add(_pin.GetActorLocation());
	// }
 //
	// InitializePinStandingDictionary(outPinActors);
 //
	// gamemaster.BowlNewTurnIsReady += BowlNewTurnIsReady;
	// gamemaster.OnPinHasFallen += PinHasFallen;
	// gamemaster.OnPinHasGottenBackUp += PinGottenBackUp;
}


// Called every frame
void UPinManagerComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UPinManagerComponentCPP::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	// if (gamemaster != null)
	// {
	// 	gamemaster.BowlNewTurnIsReady -= BowlNewTurnIsReady;
	// 	gamemaster.OnPinHasFallen -= PinHasFallen;
	// 	gamemaster.OnPinHasGottenBackUp -= PinGottenBackUp;
	// }
}
#pragma endregion

#pragma region Handlers
void UPinManagerComponentCPP::PinHasFallen(UBowlingPinComponentCPP* _pin)
{
	//UpdatePinHasStandingDictionary(_pin, true);
}

void UPinManagerComponentCPP::PinGottenBackUp(UBowlingPinComponentCPP* _pin)
{
	//UpdatePinHasStandingDictionary(_pin, false);
}

void UPinManagerComponentCPP::BowlNewTurnIsReady(EBowlActionCPP _action)
{
	// if(_action != EBowlAction.Tidy)
	// {
	// 	List<AActor> _outPins = RespawnPins();
	// 	InitializePinStandingDictionary(_outPins);
	// }
}
#pragma endregion

#pragma region Spawn-Attach-Pins
TArray<AActor*> UPinManagerComponentCPP::RespawnPins()
{
	// List<AActor> _outPins = new List<AActor>();
	// if (PinPrefabClass == null)
	// {
	// 	MyOwner.PrintString("No PinPrefab On Pin Manager BP", FLinearColor.Red, printToLog: true);
	// 	return null;
	// }
	//
	// foreach (var _pinLocation in PinLocations)
	// {
	// 	_outPins.Add(SpawnPin(_pinLocation));
	// }
	//
	// return _outPins;
	return TArray<AActor*>();
}

AActor* UPinManagerComponentCPP::SpawnPin(FVector _pinLocation)
{
	// FRotator _pinRot = FRotator.ZeroRotator;
	// return MyOwner.World.SpawnActor(PinPrefabClass, ref _pinLocation, ref _pinRot);
	return nullptr;
}

void UPinManagerComponentCPP::AttachPinToManager(AActor* _pin)
{
	// _pin.AttachToActor(MyOwner, new FName("None"),
 //    EAttachmentRule.KeepWorld, EAttachmentRule.KeepWorld,
 //    EAttachmentRule.KeepWorld, true);
}
#pragma endregion
