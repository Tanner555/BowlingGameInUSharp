// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/MyBowlPlayerComponentCPP.h"
#include "Kismet/GameplayStatics.h"
#include "BowlingBallComponentCPP.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "GameFramework/GameModeBase.h"

// Sets default values for this component's properties
UMyBowlPlayerComponentCPP::UMyBowlPlayerComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;
	BallFollowLimitDistance = 3200.0f;
	// ...
}

#pragma region Component Getters
UBowlGameMasterComponentCPP* UMyBowlPlayerComponentCPP::GetGameMaster()
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

UBowlGameModeComponentCPP* UMyBowlPlayerComponentCPP::GetBowlGameMode()
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

#pragma region Overrides
// Called when the game starts
void UMyBowlPlayerComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	auto _gamemaster = GetGameMaster();
	if (ensure(_gamemaster != nullptr) == false)
	{
		return;
	}

	if (ensure(_gamemaster->OnBallLaunch.IsAlreadyBound(this, &UMyBowlPlayerComponentCPP::StartFollowingBall) == false))
	{
		_gamemaster->OnBallLaunch.AddDynamic(this, &UMyBowlPlayerComponentCPP::StartFollowingBall);
	}
	if (ensure(_gamemaster->OnNudgeBallLeft.IsAlreadyBound(this, &UMyBowlPlayerComponentCPP::NudgeBallLeft) == false))
	{
		_gamemaster->OnNudgeBallLeft.AddDynamic(this, &UMyBowlPlayerComponentCPP::NudgeBallLeft);
	}
	if (ensure(_gamemaster->OnNudgeBallRight.IsAlreadyBound(this, &UMyBowlPlayerComponentCPP::NudgeBallRight) == false))
	{
		_gamemaster->OnNudgeBallRight.AddDynamic(this, &UMyBowlPlayerComponentCPP::NudgeBallRight);
	}
	if (ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UMyBowlPlayerComponentCPP::NewTurnIsReady) == false))
	{
		_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UMyBowlPlayerComponentCPP::NewTurnIsReady);
	}
	if (ensure(_gamemaster->OnWinGame.IsAlreadyBound(this, &UMyBowlPlayerComponentCPP::OnWinGame) == false))
	{
		_gamemaster->OnWinGame.AddDynamic(this, &UMyBowlPlayerComponentCPP::OnWinGame);
	}

	MyStartLocation = GetOwner()->GetActorLocation();
	MyStartRotation = GetOwner()->GetActorRotation();	
}


// Called every frame
void UMyBowlPlayerComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
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
void UMyBowlPlayerComponentCPP::OnWinGame()
{

}

void UMyBowlPlayerComponentCPP::NewTurnIsReady(EBowlActionCPP _action)
{
	GetOwner()->SetActorLocation(MyStartLocation, false);
	GetOwner()->SetActorRotation(MyStartRotation);
}

void UMyBowlPlayerComponentCPP::StartFollowingBall(FVector launchVelocity, UBowlingBallComponentCPP* bowlingBall)
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

void UMyBowlPlayerComponentCPP::NudgeBallLeft(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}

void UMyBowlPlayerComponentCPP::NudgeBallRight(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}
#pragma endregion

#pragma region UFunctions
void UMyBowlPlayerComponentCPP::OnDragStart(FVector2D mousePos)
{
	auto _gamemode = GetBowlGameMode();
	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemode != nullptr) && ensure(_gamemaster != nullptr) &&
		_gamemaster->bCanLaunchBall && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemode->OnStartDrag(mousePos);
	}
}

void UMyBowlPlayerComponentCPP::OnDragStop(FVector2D mousePos)
{
	auto _gamemode = GetBowlGameMode();
	auto _gamemaster = GetGameMaster();
	if (ensure(_gamemode != nullptr) && ensure(_gamemaster != nullptr) &&
		_gamemaster->bCanLaunchBall && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemode->OnStopDrag(mousePos);
	}
}
void UMyBowlPlayerComponentCPP::Debug_InstantStrike()
{
	auto _gamemaster = GetGameMaster();
	if(_gamemaster != nullptr)
	{
		_gamemaster->CallDebug_OnSimulateStrike();
	}
}
void UMyBowlPlayerComponentCPP::Debug_Fill18ScoreSlots()
{
	auto _gamemaster = GetGameMaster();
	if (_gamemaster != nullptr)
	{
		_gamemaster->CallDebug_Fill18ScoreSlots();
	}
}
#pragma endregion
