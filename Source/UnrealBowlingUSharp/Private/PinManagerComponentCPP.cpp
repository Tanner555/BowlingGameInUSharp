// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/PinManagerComponentCPP.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "BowlingPinComponentCPP.h"
#include "Engine/World.h"
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
	UpdatePinHasStandingDictionary(_pin, true);
}

void UPinManagerComponentCPP::PinGottenBackUp(UBowlingPinComponentCPP* _pin)
{
	UpdatePinHasStandingDictionary(_pin, false);
}

void UPinManagerComponentCPP::BowlNewTurnIsReady(EBowlActionCPP _action)
{
	if(_action != EBowlActionCPP::Tidy)
	{
		TArray<AActor*> _outPins = RespawnPins();
		InitializePinStandingDictionary(_outPins);
	}
}
#pragma endregion

#pragma region Spawn-Attach-Pins
TArray<AActor*> UPinManagerComponentCPP::RespawnPins()
{
	TArray<AActor*> _outPins;
	if (PinPrefabClass == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("No PinPrefab On Pin Manager BP"));
		return TArray<AActor*>();
	}
	
	for (auto _pinLocation : PinLocations)
	{
		_outPins.Add(SpawnPin(_pinLocation));
	}
	
	return _outPins;
}

AActor* UPinManagerComponentCPP::SpawnPin(FVector _pinLocation)
{
	if(ensure(PinPrefabClass != nullptr) == false) return nullptr;
	
	FRotator _pinRot = FRotator::ZeroRotator;
	return GetOwner()->GetWorld()->SpawnActor(PinPrefabClass, &_pinLocation, &_pinRot);
}

void UPinManagerComponentCPP::AttachPinToManager(AActor* _pin)
{
	if(ensure(_pin != nullptr) == false) return;
	
	_pin->AttachToActor(GetOwner(),
    FAttachmentTransformRules(EAttachmentRule::KeepWorld,
        EAttachmentRule::KeepWorld, EAttachmentRule::KeepWorld, true));
}
#pragma endregion

#pragma region PinFallenDictionaryHandling
void UPinManagerComponentCPP::InitializePinStandingDictionary(TArray<AActor*> pinActors)
{
	//Dict Already Init, Just Need To Empty
	AllPinsStandingDictionary.Empty();
	for (AActor* _pin : pinActors)
	{
		if(_pin != nullptr)
		{
			AllPinsStandingDictionary.Add(_pin->GetName(), true);
		}
	}
}

void UPinManagerComponentCPP::UpdatePinHasStandingDictionary(UBowlingPinComponentCPP* _pin, bool _fallen)
{
	auto _gamemaster = GetGameMaster();
	if(_gamemaster == nullptr) return;
	
	FString _key = _pin->GetOwner()->GetName();
	if (AllPinsStandingDictionary.Contains(_key))
	{
		AllPinsStandingDictionary[_key] = !_fallen;
		int _pinStandingCount = 0;
		TArray<bool> _outValueArray = TArray<bool>();
		AllPinsStandingDictionary.GenerateValueArray(_outValueArray);
		for (bool _pinStanding : _outValueArray)
		{
			if (_pinStanding) _pinStandingCount++;
		}
		_gamemaster->CallUpdatePinCount(_pinStandingCount);
	}
}
#pragma endregion