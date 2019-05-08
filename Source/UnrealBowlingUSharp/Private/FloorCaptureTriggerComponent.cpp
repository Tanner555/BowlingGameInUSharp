// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/FloorCaptureTriggerComponent.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/GameModeBase.h"
#include "BowlGameMasterComponent.h"
#include "BowlGameModeComponent.h"


#pragma region Component Getters
UBowlGameMasterComponent* UFloorCaptureTriggerComponent::GetGameMaster()
{
	if (bowlGameMaster == nullptr)
	{
		auto _gamemode = UGameplayStatics::GetGameMode(this);
		if (_gamemode != nullptr)
		{
			bowlGameMaster = Cast<UBowlGameMasterComponent>(_gamemode->GetComponentByClass(UBowlGameMasterComponent::StaticClass()));
		}
	}
	return bowlGameMaster;
}

UBowlGameModeComponent* UFloorCaptureTriggerComponent::GetBowlGameMode()
{
	if (bowlGameMode == nullptr)
	{
		auto _gamemode = UGameplayStatics::GetGameMode(this);
		if (_gamemode != nullptr)
		{
			bowlGameMode = Cast<UBowlGameModeComponent>(_gamemode->GetComponentByClass(UBowlGameModeComponent::StaticClass()));
		}
	}
	return bowlGameMode;
}
#pragma endregion

#pragma region OverridesAndInit
// Sets default values for this component's properties
UFloorCaptureTriggerComponent::UFloorCaptureTriggerComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UFloorCaptureTriggerComponent::BeginPlay()
{
	Super::BeginPlay();

	// ...

}


// Called every frame
void UFloorCaptureTriggerComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}
#pragma endregion

#pragma region UFunctions
void UFloorCaptureTriggerComponent::OnBeginOverlapWrapper(UPrimitiveComponent* OverlappedComp, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, FHitResult SweepResult)
{
	if(OtherActor != nullptr)
	{
		auto _gamemode = GetBowlGameMode();
		auto _gamemaster = GetGameMaster();
		if(ensure(_gamemode != nullptr) == false ||
			ensure(_gamemaster != nullptr) == false)
		{
			//GameMode or GameMaster is Null, Return
			return;
		}

		if(OtherActor->ActorHasTag(_gamemode->BallTag) &&
		_gamemaster->bBowlTurnIsOver == false)
		{
			WaitForPinsToFall();
		}
		else if(OtherActor->ActorHasTag(_gamemode->PinTag))
		{
			//Other Actor is Pin, Destroy It
			OtherActor->Destroy();
		}
	}
}

void UFloorCaptureTriggerComponent::CallTurnIsFinishedAfterWaiting()
{
	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr) && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemaster->CallBowlTurnIsFinished();
	}
}
#pragma endregion
