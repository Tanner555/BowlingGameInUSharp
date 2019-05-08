// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/MyBowlPlayerComponent.h"
#include "Kismet/GameplayStatics.h"
#include "BowlingBallComponent.h"
#include "BowlGameMasterComponent.h"
#include "BowlGameModeComponent.h"
#include "GameFramework/GameModeBase.h"

// Sets default values for this component's properties
UMyBowlPlayerComponent::UMyBowlPlayerComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;
	BallFollowLimitDistance = 3200.0f;
	// ...
}

#pragma region Component Getters
UBowlGameMasterComponent* UMyBowlPlayerComponent::GetGameMaster()
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

UBowlGameModeComponent* UMyBowlPlayerComponent::GetBowlGameMode()
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

#pragma region Overrides
// Called when the game starts
void UMyBowlPlayerComponent::BeginPlay()
{
	Super::BeginPlay();

	auto _gamemaster = GetGameMaster();
	if (ensure(_gamemaster != nullptr) == false)
	{
		return;
	}

	if (ensure(_gamemaster->OnBallLaunch.IsAlreadyBound(this, &UMyBowlPlayerComponent::StartFollowingBall) == false))
	{
		_gamemaster->OnBallLaunch.AddDynamic(this, &UMyBowlPlayerComponent::StartFollowingBall);
	}
	if (ensure(_gamemaster->OnNudgeBallLeft.IsAlreadyBound(this, &UMyBowlPlayerComponent::NudgeBallLeft) == false))
	{
		_gamemaster->OnNudgeBallLeft.AddDynamic(this, &UMyBowlPlayerComponent::NudgeBallLeft);
	}
	if (ensure(_gamemaster->OnNudgeBallRight.IsAlreadyBound(this, &UMyBowlPlayerComponent::NudgeBallRight) == false))
	{
		_gamemaster->OnNudgeBallRight.AddDynamic(this, &UMyBowlPlayerComponent::NudgeBallRight);
	}
	if (ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UMyBowlPlayerComponent::NewTurnIsReady) == false))
	{
		_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UMyBowlPlayerComponent::NewTurnIsReady);
	}
	if (ensure(_gamemaster->OnWinGame.IsAlreadyBound(this, &UMyBowlPlayerComponent::OnWinGame) == false))
	{
		_gamemaster->OnWinGame.AddDynamic(this, &UMyBowlPlayerComponent::OnWinGame);
	}

	MyStartLocation = GetOwner()->GetActorLocation();
	MyStartRotation = GetOwner()->GetActorRotation();
}


// Called every frame
void UMyBowlPlayerComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	if (bShouldFollowBall && myBall != nullptr)
	{
		auto _myPos = GetOwner()->GetActorLocation();

		if (_myPos.X >= BallFollowLimitDistance)
		{
			bShouldFollowBall = false;
			return;
		}

		auto _ballPos = myBall->GetOwner()->GetActorLocation();
		auto _xTravelPos = _ballPos.X + DefaultBallFollowOffset;
		//PrintString("Ball Pos: " + _ballPos, FLinearColor.Green, printToLog:true);
		GetOwner()->SetActorLocation(
			FVector(_xTravelPos, _myPos.Y, _myPos.Z),
			true, false
		);

	}
}
#pragma endregion

#pragma region Handlers
void UMyBowlPlayerComponent::OnWinGame()
{

}

void UMyBowlPlayerComponent::NewTurnIsReady(EBowlAction _action)
{
	GetOwner()->SetActorLocation(MyStartLocation, false);
	GetOwner()->SetActorRotation(MyStartRotation);
}

void UMyBowlPlayerComponent::StartFollowingBall(FVector launchVelocity, UBowlingBallComponent* bowlingBall)
{
	myBall = bowlingBall;
	if (myBall != nullptr)
	{
		DefaultBallFollowOffset = GetOwner()->GetActorLocation().X - myBall->GetOwner()->GetActorLocation().X;
		bShouldFollowBall = true;
	}
	else
	{
		bShouldFollowBall = false;
	}
}

void UMyBowlPlayerComponent::NudgeBallLeft(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}

void UMyBowlPlayerComponent::NudgeBallRight(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}
#pragma endregion

#pragma region UFunctions
void UMyBowlPlayerComponent::OnDragStart(FVector2D mousePos)
{
	auto _gamemode = GetBowlGameMode();
	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemode != nullptr) && ensure(_gamemaster != nullptr) &&
		_gamemaster->bCanLaunchBall && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemode->OnStartDrag(mousePos);
	}
}

void UMyBowlPlayerComponent::OnDragStop(FVector2D mousePos)
{
	auto _gamemode = GetBowlGameMode();
	auto _gamemaster = GetGameMaster();
	if (ensure(_gamemode != nullptr) && ensure(_gamemaster != nullptr) &&
		_gamemaster->bCanLaunchBall && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemode->OnStopDrag(mousePos);
	}
}
void UMyBowlPlayerComponent::Debug_InstantStrike()
{
	auto _gamemaster = GetGameMaster();
	if(_gamemaster != nullptr)
	{
		_gamemaster->CallDebug_OnSimulateStrike();
	}
}
void UMyBowlPlayerComponent::Debug_Fill18ScoreSlots()
{
	auto _gamemaster = GetGameMaster();
	if (_gamemaster != nullptr)
	{
		_gamemaster->CallDebug_Fill18ScoreSlots();
	}
}
#pragma endregion
