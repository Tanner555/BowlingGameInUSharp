// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/FloorCaptureTriggerComponentCPP.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/GameModeBase.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "BowlingBallComponentCPP.h"


#pragma region Component Getters
UBowlGameMasterComponentCPP* UFloorCaptureTriggerComponentCPP::GetGameMaster()
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

UBowlGameModeComponentCPP* UFloorCaptureTriggerComponentCPP::GetBowlGameMode()
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

#pragma region OverridesAndInit
// Sets default values for this component's properties
UFloorCaptureTriggerComponentCPP::UFloorCaptureTriggerComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UFloorCaptureTriggerComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	// ...

}


// Called every frame
void UFloorCaptureTriggerComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}
#pragma endregion

#pragma region UFunctions
void UFloorCaptureTriggerComponentCPP::OnBeginOverlapWrapper(UPrimitiveComponent* OverlappedComp, AActor* OtherActor,
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
			auto _ballComp = Cast<UBowlingBallComponentCPP>(OtherActor->GetComponentByClass(UBowlingBallComponentCPP::StaticClass()));
			if(_ballComp != nullptr)
			{
				_ballComp->StopRollingSound();
				WaitForPinsToFall();
			}
		}
		else if(OtherActor->ActorHasTag(_gamemode->PinTag))
		{
			//Other Actor is Pin, Destroy It
			OtherActor->Destroy();
		}
	}
}

void UFloorCaptureTriggerComponentCPP::CallTurnIsFinishedAfterWaiting()
{
	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr) && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemaster->CallBowlTurnIsFinished();
	}
}
#pragma endregion
