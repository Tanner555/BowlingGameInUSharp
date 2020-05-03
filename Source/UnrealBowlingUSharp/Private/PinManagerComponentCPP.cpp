// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/PinManagerComponentCPP.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "BowlingPinComponentCPP.h"
#include "GameFramework/GameModeBase.h"
#include "Kismet/GameplayStatics.h"

#pragma region CompAndOtherGetters
TArray<AActor*> UPinManagerComponentCPP::GetAllPins()
{
	auto _gamemode = GetBowlGameMode();
	if(ensure(_gamemode != nullptr) == false) return TArray<AActor*>();
	
	TArray<AActor*> outPinActors;
	UGameplayStatics::GetAllActorsWithTag(GetOwner(), _gamemode->PinTag, outPinActors);
	return outPinActors;
}

UBowlGameMasterComponentCPP* UPinManagerComponentCPP::GetGameMaster()
{
	if (bowlGameMaster == nullptr)
	{
		auto _gamemode = UGameplayStatics::GetGameMode(this);
		if (_gamemode != nullptr)
		{
			bowlGameMaster = Cast<UBowlGameMasterComponentCPP>(_gamemode->GetComponentByClass(UBowlGameMasterComponentCPP::StaticClass()));
		}
	}
	return bowlGameMaster;
}

UBowlGameModeComponentCPP* UPinManagerComponentCPP::GetBowlGameMode()
{
	if (bowlGameMode == nullptr)
	{
		auto _gamemode = UGameplayStatics::GetGameMode(this);
		if (_gamemode != nullptr)
		{
			bowlGameMode = Cast<UBowlGameModeComponentCPP>(_gamemode->GetComponentByClass(UBowlGameModeComponentCPP::StaticClass()));
		}
	}
	return bowlGameMode;
}
#pragma endregion

#pragma region InitAndOverrides
// Sets default values for this component's properties
UPinManagerComponentCPP::UPinManagerComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;
	//Init Arrays/Dictionaries
	PinLocations = TArray<FVector>();
	AllPinsStandingDictionary = TMap<FString, bool>();
	
}


// Called when the game starts
void UPinManagerComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr) == false) return;

	TArray<AActor*> outPinActors = GetAllPins();
	if (outPinActors.Num() > 0 &&
	       outPinActors[0] != nullptr)
	{
		PinPrefabClass = outPinActors[0]->GetClass();
	}
	
	for (auto _pin : outPinActors)
	{
		AttachPinToManager(_pin);
		PinLocations.Add(_pin->GetActorLocation());
	}

	InitializePinStandingDictionary(outPinActors);

	if (ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UPinManagerComponentCPP::BowlNewTurnIsReady) == false))
	{
		_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UPinManagerComponentCPP::BowlNewTurnIsReady);
	}
	if (ensure(_gamemaster->OnPinHasFallen.IsAlreadyBound(this, &UPinManagerComponentCPP::PinHasFallen) == false))
	{
		_gamemaster->OnPinHasFallen.AddDynamic(this, &UPinManagerComponentCPP::PinHasFallen);
	}
	if (ensure(_gamemaster->OnPinHasGottenBackUp.IsAlreadyBound(this, &UPinManagerComponentCPP::PinGottenBackUp) == false))
	{
		_gamemaster->OnPinHasGottenBackUp.AddDynamic(this, &UPinManagerComponentCPP::PinGottenBackUp);
	}
}


// Called every frame
void UPinManagerComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UPinManagerComponentCPP::EndPlay(const EEndPlayReason::Type EndPlayReason)
{

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

#pragma region PinFallenDictionaryHandling
void UPinManagerComponentCPP::InitializePinStandingDictionary(TArray<AActor*> pinActors)
{
	
}

void UPinManagerComponentCPP::UpdatePinHasStandingDictionary(UBowlingPinComponentCPP* _pin, bool _fallen)
{
	
}
#pragma endregion