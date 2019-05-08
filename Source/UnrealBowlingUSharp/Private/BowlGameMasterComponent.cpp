// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlGameMasterComponent.h"
#include "Engine/World.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/GameModeBase.h"
#include "BowlGameModeComponent.h"

#pragma region Initialization
// Sets default values for this component's properties
UBowlGameMasterComponent::UBowlGameMasterComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}
#pragma endregion

#pragma region Overrides
// Called when the game starts
void UBowlGameMasterComponent::BeginPlay()
{
	Super::BeginPlay();

	bBowlTurnIsOver = false;
	bCanLaunchBall = true;
}

// Called every frame
void UBowlGameMasterComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UBowlGameMasterComponent::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	if(BowlNewTurnIsReady.IsBound())
	{
		BowlNewTurnIsReady.Clear();
	}

	if(BowlTurnIsFinished.IsBound())
	{
		BowlTurnIsFinished.Clear();
	}

	if(OnBallLaunch.IsBound())
	{
		OnBallLaunch.Clear();
	}

	if(OnNudgeBallLeft.IsBound())
	{
		OnNudgeBallLeft.Clear();
	}

	if(OnNudgeBallRight.IsBound())
	{
		OnNudgeBallRight.Clear();
	}

	if(OnPinHasFallen.IsBound())
	{
		OnPinHasFallen.Clear();
	}

	if(OnPinHasGottenBackUp.IsBound())
	{
		OnPinHasGottenBackUp.Clear();
	}
	
	if(OnUpdatePinCount.IsBound())
	{
		OnUpdatePinCount.Clear();
	}

	if(OnSendBowlActionResults.IsBound())
	{
		OnSendBowlActionResults.Clear();
	}

	if(OnWinGame.IsBound())
	{
		OnWinGame.Clear();
	}

	if(Debug_OnSimulateStrike.IsBound())
	{
		Debug_OnSimulateStrike.Clear();
	}

	if(Debug_Fill18ScoreSlots.IsBound())
	{
		Debug_Fill18ScoreSlots.Clear();
	}
}
#pragma endregion

#pragma region EventCalls
void UBowlGameMasterComponent::CallOnBallLaunch(FVector launchVelocity, UBowlingBallComponent* bowlingBall)
{
	bCanLaunchBall = false;
	if (OnBallLaunch.IsBound()) OnBallLaunch.Broadcast(launchVelocity, bowlingBall);
}

void UBowlGameMasterComponent::CallBowlNewTurnIsReady(EBowlAction _action)
{
	bBowlTurnIsOver = false;
	bCanLaunchBall = true;
	if (BowlNewTurnIsReady.IsBound()) BowlNewTurnIsReady.Broadcast(_action);
}

void UBowlGameMasterComponent::CallBowlTurnIsFinished()
{
	//Only Call If Bowl Turn Isn't Finished Yet
	if (bBowlTurnIsOver) return;;

	bBowlTurnIsOver = true;
	if (BowlTurnIsFinished.IsBound()) BowlTurnIsFinished.Broadcast();
}

void UBowlGameMasterComponent::CallOnNudgeBallLeft(float famount)
{
	if (OnNudgeBallLeft.IsBound()) OnNudgeBallLeft.Broadcast(famount);
}

void UBowlGameMasterComponent::CallOnNudgeBallRight(float famount)
{
	if (OnNudgeBallRight.IsBound()) OnNudgeBallRight.Broadcast(famount);
}

void UBowlGameMasterComponent::CallOnPinHasFallen(UBowlingPinComponent* _pin)
{
	if (OnPinHasFallen.IsBound()) OnPinHasFallen.Broadcast(_pin);
}

void UBowlGameMasterComponent::CallOnPinHasGottenBackUp(UBowlingPinComponent* _pin)
{
	if (OnPinHasGottenBackUp.IsBound()) OnPinHasGottenBackUp.Broadcast(_pin);
}

void UBowlGameMasterComponent::CallUpdatePinCount(int32 _count)
{
	if (OnUpdatePinCount.IsBound()) OnUpdatePinCount.Broadcast(_count);
}

void UBowlGameMasterComponent::CallOnSendBowlActionResults(EBowlAction _action)
{
	if (OnSendBowlActionResults.IsBound()) OnSendBowlActionResults.Broadcast(_action);
}

void UBowlGameMasterComponent::CallOnWinGame()
{
	if (OnWinGame.IsBound()) OnWinGame.Broadcast();
}

void UBowlGameMasterComponent::CallDebug_OnSimulateStrike()
{
	if (Debug_OnSimulateStrike.IsBound()) Debug_OnSimulateStrike.Broadcast();
}

void UBowlGameMasterComponent::CallDebug_Fill18ScoreSlots()
{
	if (Debug_Fill18ScoreSlots.IsBound()) Debug_Fill18ScoreSlots.Broadcast();
}
#pragma endregion

#pragma region Getters
UBowlGameModeComponent* UBowlGameMasterComponent::GetBowlGameMode()
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
