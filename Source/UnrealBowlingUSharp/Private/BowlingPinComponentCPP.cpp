// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlingPinComponentCPP.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "PinManagerComponentCPP.h"
#include "GameFramework/GameModeBase.h"
#include "Kismet/GameplayStatics.h"

// Sets default values for this component's properties
UBowlingPinComponentCPP::UBowlingPinComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}

#pragma region ComponentGetters
UBowlGameMasterComponentCPP* UBowlingPinComponentCPP::GetGameMaster()
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

UBowlGameModeComponentCPP* UBowlingPinComponentCPP::GetBowlGameMode()
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

UPinManagerComponentCPP* UBowlingPinComponentCPP::GetPinManager()
{
	if(myPinManager == nullptr)
	{
		auto _bowlGameMode = GetBowlGameMode();
		if(_bowlGameMode != nullptr)
		{
			TArray<AActor*> sweepActors;
			UGameplayStatics::GetAllActorsWithTag(this, _bowlGameMode->PinManagerTag, sweepActors);
			if (sweepActors.Num() > 0 && sweepActors[0] != nullptr)
			{
				auto _comp = sweepActors[0]->GetComponentByClass(UPinManagerComponentCPP::StaticClass());
				if(_comp != nullptr)
				{
					myPinManager = Cast<UPinManagerComponentCPP>(_comp);
				}
			}
		} 
	}
	return myPinManager;
}
#pragma endregion

#pragma region Overrides
// Called when the game starts
void UBowlingPinComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	// ...

}


// Called every frame
void UBowlingPinComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}
#pragma endregion

#pragma region UFunctions
bool UBowlingPinComponentCPP::SE_CheckForPinHasFallen()
{
	return false;
}
#pragma endregion

#pragma region Handlers
/// <summary>
/// Also Called When Won Game 
/// </summary>
void UBowlingPinComponentCPP::OnTurnIsFinished()
{
	//StopAllCoroutines();
}

void UBowlingPinComponentCPP::OnSendBowlActionResults(EBowlActionCPP _action)
{
	
}

void UBowlingPinComponentCPP::NewBowlTurnHasStarted(EBowlActionCPP _action)
{
	
}

//Debug
void UBowlingPinComponentCPP::OnSimulateStrike()
{
	
}
#pragma endregion
