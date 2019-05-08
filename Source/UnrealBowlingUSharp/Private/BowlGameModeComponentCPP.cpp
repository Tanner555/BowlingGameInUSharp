// Fill out your copyright notice in the Description page of Project Settings.

#include "Public/BowlGameModeComponentCPP.h"
#include "GameFramework/Actor.h"
#include "BowlGameMasterComponentCPP.h"
#include "Kismet/GameplayStatics.h"
#include "Camera/PlayerCameraManager.h"
#include "GameFramework/GameModeBase.h"
#include "BowlingBallComponentCPP.h"
#include "MyBowlPlayerComponentCPP.h"
#include "Engine/World.h"
#include "Engine/StaticMeshActor.h"

#pragma region Initialization
// Sets default values for this component's properties
UBowlGameModeComponentCPP::UBowlGameModeComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;
	// ...
}
#pragma endregion

#pragma region ComponentGetters
UBowlGameMasterComponentCPP* UBowlGameModeComponentCPP::GetGameMaster()
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
#pragma endregion

#pragma region OtherGetters
int32 UBowlGameModeComponentCPP::GetStandingPinCount()
{
	return StandingPinCount;
}

TArray<int32> UBowlGameModeComponentCPP::GetAllBowlFrameResults()
{
	return TArray<int32>{
		Frame01_BowlA, Frame01_BowlB, Frame01_Results,
		Frame02_BowlA, Frame02_BowlB, Frame02_Results,
		Frame03_BowlA, Frame03_BowlB, Frame03_Results,
		Frame04_BowlA, Frame04_BowlB, Frame04_Results,
		Frame05_BowlA, Frame05_BowlB, Frame05_Results,
		Frame06_BowlA, Frame06_BowlB, Frame06_Results,
		Frame07_BowlA, Frame07_BowlB, Frame07_Results,
		Frame08_BowlA, Frame08_BowlB, Frame08_Results,
		Frame09_BowlA, Frame09_BowlB, Frame09_Results,
		Frame10_BowlA, Frame10_BowlB, Frame10_BowlC, Frame10_Results
	};
}

TArray<int32> UBowlGameModeComponentCPP::GetAllBowlFrameTurns()
{
	return TArray<int32>{
		Frame01_BowlA, Frame01_BowlB,
		Frame02_BowlA, Frame02_BowlB,
		Frame03_BowlA, Frame03_BowlB,
		Frame04_BowlA, Frame04_BowlB,
		Frame05_BowlA, Frame05_BowlB,
		Frame06_BowlA, Frame06_BowlB,
		Frame07_BowlA, Frame07_BowlB,
		Frame08_BowlA, Frame08_BowlB,
		Frame09_BowlA, Frame09_BowlB,
		Frame10_BowlA, Frame10_BowlB, Frame10_BowlC
	};
}

int32 UBowlGameModeComponentCPP::GetPinFallCount()
{
	return lastSettledCount - StandingPinCount;
}

TArray<int32> UBowlGameModeComponentCPP::GetBowlTurnListFromCount()
{
	TArray<int32> _bowlTurns = TArray<int32>();
	TArray<int32> _allbowlframeturns = GetAllBowlFrameTurns();
	for (int i = 0; i < BowlTurnCount; i++)
	{
		if (_allbowlframeturns.IsValidIndex(i)) {
			_bowlTurns.Add(_allbowlframeturns[i]);
		}
	}
	return _bowlTurns;
}

int32 UBowlGameModeComponentCPP::GetBowlTurnCount()
{
	return BowlTurnCount;
}
#pragma endregion

#pragma region Setters
/// A Setter For StandingPinCount, which also Calls A Few Update Events.
void UBowlGameModeComponentCPP::SetStandingPinCount(int32 pinCount)
{
	StandingPinCount = pinCount;
	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr))
	{
		_gamemaster->CallUpdatePinCount(pinCount);
	}
	UpdatePinCountBPEvent(pinCount);
}

void UBowlGameModeComponentCPP::SetResultsFromFrameTurns()
{
	Frame01_Results = Frame01_BowlA + Frame01_BowlB;
	Frame02_Results = Frame02_BowlA + Frame02_BowlB;
	Frame03_Results = Frame03_BowlA + Frame03_BowlB;
	Frame04_Results = Frame04_BowlA + Frame04_BowlB;
	Frame05_Results = Frame05_BowlA + Frame05_BowlB;
	Frame06_Results = Frame06_BowlA + Frame06_BowlB;
	Frame07_Results = Frame07_BowlA + Frame07_BowlB;
	Frame08_Results = Frame08_BowlA + Frame08_BowlB;
	Frame09_Results = Frame09_BowlA + Frame09_BowlB;
	Frame10_Results = Frame10_BowlA + Frame10_BowlB + Frame10_BowlC;
	UpdateBowlTurnFramesBPEvent();
}

void UBowlGameModeComponentCPP::SetCurrentBowlTurnValue(int32 _value)
{
	switch (BowlTurnCount)
	{
	case 1:
		Frame01_BowlA = _value;
		break;
	case 2:
		Frame01_BowlB = _value;
		break;
	case 3:
		Frame02_BowlA = _value;
		break;
	case 4:
		Frame02_BowlB = _value;
		break;
	case 5:
		Frame03_BowlA = _value;
		break;
	case 6:
		Frame03_BowlB = _value;
		break;
	case 7:
		Frame04_BowlA = _value;
		break;
	case 8:
		Frame04_BowlB = _value;
		break;
	case 9:
		Frame05_BowlA = _value;
		break;
	case 10:
		Frame05_BowlB = _value;
		break;
	case 11:
		Frame06_BowlA = _value;
		break;
	case 12:
		Frame06_BowlB = _value;
		break;
	case 13:
		Frame07_BowlA = _value;
		break;
	case 14:
		Frame07_BowlB = _value;
		break;
	case 15:
		Frame08_BowlA = _value;
		break;
	case 16:
		Frame08_BowlB = _value;
		break;
	case 17:
		Frame09_BowlA = _value;
		break;
	case 18:
		Frame09_BowlB = _value;
		break;
	case 19:
		Frame10_BowlA = _value;
		break;
	case 20:
		Frame10_BowlB = _value;
		break;
	case 21:
		Frame10_BowlC = _value;
		break;
	default:
		UE_LOG(LogTemp, Warning, TEXT("Couldn't Set Bowl Turn At: %s"), *FString::FromInt(BowlTurnCount));
		break;
	}
	UpdateBowlTurnFramesBPEvent();
}
#pragma endregion

#pragma region Overrides
void UBowlGameModeComponentCPP::InitializeComponent()
{
	MinimalForwardLaunchVelocity = 1500;
	ForwardMultipleVelocityFactor = 1.5f;
}

// Called when the game starts
void UBowlGameModeComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	// ...
	//UE_LOG(LogTemp, Warning, TEXT("My message is: %s"), *BallTag.ToString());
	auto _myOwner = GetOwner();
	auto _playerController = UGameplayStatics::GetPlayerController(this, 0);
	auto _playerPawn = UGameplayStatics::GetPlayerPawn(this, 0);
	TArray<AActor*> _ballActors;
	UGameplayStatics::GetAllActorsWithTag(this, BallTag, _ballActors);
	TArray<AActor*> bowlFloorActors;
	UGameplayStatics::GetAllActorsWithTag(this, BowlingFloorTag, bowlFloorActors);
	auto _gamemaster = GetGameMaster();

	if(ensure(_myOwner) && ensure(_playerController) && ensure(_playerPawn) && ensure(_gamemaster))
	{
		_playerController->bShowMouseCursor = true;
		if(_ballActors.Num() > 0 && _ballActors[0] != nullptr)
		{
			auto _ballComp = Cast<UBowlingBallComponentCPP>(_ballActors[0]->GetComponentByClass(UBowlingBallComponentCPP::StaticClass()));
			if(ensure(_ballComp != nullptr))
			{
				myBall = _ballComp;
			}
		}
		auto _playerComp = Cast<UMyBowlPlayerComponentCPP>(_playerPawn->GetComponentByClass(UMyBowlPlayerComponentCPP::StaticClass()));
		if(ensure(_playerComp != nullptr))
		{
			myBowler = _playerComp;
		}
		StandingPinCount = 10;
		BowlTurnCount = 1;
		if(ensure(_gamemaster->OnUpdatePinCount.IsAlreadyBound(this, &UBowlGameModeComponentCPP::UpdatePinCount) == false))
		{
			_gamemaster->OnUpdatePinCount.AddDynamic(this, &UBowlGameModeComponentCPP::UpdatePinCount);
		}

		if (ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UBowlGameModeComponentCPP::ResetPinCount) == false))
		{
			_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UBowlGameModeComponentCPP::ResetPinCount);
		}

		if (ensure(_gamemaster->BowlTurnIsFinished.IsAlreadyBound(this, &UBowlGameModeComponentCPP::OnTurnIsFinished) == false))
		{
			_gamemaster->BowlTurnIsFinished.AddDynamic(this, &UBowlGameModeComponentCPP::OnTurnIsFinished);
		}

		if (ensure(_gamemaster->OnSendBowlActionResults.IsAlreadyBound(this, &UBowlGameModeComponentCPP::OnSendBowlActionResults) == false))
		{
			_gamemaster->OnSendBowlActionResults.AddDynamic(this, &UBowlGameModeComponentCPP::OnSendBowlActionResults);
		}

		if (ensure(_gamemaster->Debug_Fill18ScoreSlots.IsAlreadyBound(this, &UBowlGameModeComponentCPP::Debug_Fill18ScoreSlots) == false))
		{
			_gamemaster->Debug_Fill18ScoreSlots.AddDynamic(this, &UBowlGameModeComponentCPP::Debug_Fill18ScoreSlots);
		}

		if(bowlFloorActors.Num() > 0 && bowlFloorActors[0] != nullptr)
		{
			auto _staticActor = Cast<AStaticMeshActor>(bowlFloorActors[0]);
			if(ensure(_staticActor != nullptr))
			{
				BowlFloorMeshActor = _staticActor;
				FVector _origin;
				FVector _bounds;
				BowlFloorMeshActor->GetActorBounds(false, _origin, _bounds);
				boundsYLeftEdge = _origin.Y - _bounds.Y;
				boundsYRightEdge = _origin.Y + _bounds.Y;
			}
		}
	}
}


// Called every frame
void UBowlGameModeComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UBowlGameModeComponentCPP::EndPlay(const EEndPlayReason::Type EndPlayReason)
{

}
#pragma endregion

#pragma region Handlers
void UBowlGameModeComponentCPP::OnSendBowlActionResults(EBowlActionCPP _action)
{
	SendBowlActionResultsAndWaitBPEvent(_action);
}

void UBowlGameModeComponentCPP::OnTurnIsFinished()
{
	auto _gamemaster = GetGameMaster();
	if (ensure(_gamemaster != nullptr)) {
		EBowlActionCPP _action = Bowl();
		SetResultsFromFrameTurns();
		_gamemaster->CallOnSendBowlActionResults(_action);
	}
}

void UBowlGameModeComponentCPP::UpdatePinCount(int32 _pinCount)
{
	StandingPinCount = _pinCount;
}

void UBowlGameModeComponentCPP::ResetPinCount(EBowlActionCPP _action)
{
	if (_action != EBowlActionCPP::Tidy)
	{
		StandingPinCount = 10;
		lastSettledCount = 10;
	}
}

void UBowlGameModeComponentCPP::Debug_Fill18ScoreSlots()
{
	int32 _lastMinuteYield = 0;
	while (BowlTurnCount < 19)
	{
		//Currently Set Random Value So That Two Values
		//Will Never Be Over 10
		BowlHelper(FMath::RandRange(0, 5));
		//Used To Prevent The Game From Crashing
		if (BowlTurnCount >= 20 || _lastMinuteYield >= 50)
		{
			UE_LOG(LogTemp, Warning, TEXT("Breaking Out Of Loop, No Crashing!"));
			break;
		}
		_lastMinuteYield++;
		SetResultsFromFrameTurns();
	}
}
#pragma endregion

#pragma region DraggingAndBallLaunch
void UBowlGameModeComponentCPP::OnStartDrag(FVector2D mousePos)
{
	dragStart = mousePos;	
	startTime = UGameplayStatics::GetTimeSeconds(this);
}

void UBowlGameModeComponentCPP::OnStopDrag(FVector2D mousePos)
{
	dragEnd = mousePos;
	endTime = UGameplayStatics::GetTimeSeconds(this);

	float dragDuration = endTime - startTime;

	//Horizontal
	float launchSpeedY = (dragEnd.X - dragStart.X) / dragDuration;
	//Forward
	float launchSpeedX = (dragStart.Y - dragEnd.Y) / dragDuration;

	FVector _launchVelocity = FVector(launchSpeedX * ForwardMultipleVelocityFactor, launchSpeedY, 0);
	//UE_LOG(LogTemp, Warning, TEXT("Launch Velocity X: %f. ForwardMultiple: %f. LaunchSpeedX: %f"), _launchVelocity.X, ForwardMultipleVelocityFactor, launchSpeedX);
	//UE_LOG(LogTemp, Warning, TEXT("Launch Velocity Y: %f. LaunchSpeedY: %f"), _launchVelocity.Y, launchSpeedY);
	//UE_LOG(LogTemp, Warning, TEXT("StartTime: %f. EndTime: %f"), startTime, endTime);
	//UE_LOG(LogTemp, Warning, TEXT("DragStartX: %f. DragEndX: %f."), dragStart.X, dragEnd.X);
	//UE_LOG(LogTemp, Warning, TEXT("DragStartY: %f. DragEndY: %f. DragDuration: %f"), dragStart.Y, dragEnd.Y, dragDuration);
	if (_launchVelocity.X > MinimalForwardLaunchVelocity)
	{
		StartLaunchingTheBall(_launchVelocity);
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("Not Enough Force To Launch!"));
	}
}

void UBowlGameModeComponentCPP::StartLaunchingTheBall(FVector launchVelocity)
{
	auto _gamemaster = GetGameMaster();
	if (myBall != nullptr && ensure(_gamemaster != nullptr) &&
		_gamemaster->bCanLaunchBall && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemaster->CallOnBallLaunch(launchVelocity, myBall);
	}
}
#pragma endregion

#pragma region PublicUFunctionCalls
void UBowlGameModeComponentCPP::NudgeBallLeft()
{
	auto _gamemaster = GetGameMaster();
	float _nudgeAmount = -50;
	if (ensure(_gamemaster != nullptr) && _gamemaster->bCanLaunchBall &&
		_gamemaster->bBowlTurnIsOver == false && myBall != nullptr)
	{
		FVector _ballPos = myBall->GetOwner()->GetActorLocation();
		float _nextBallY = _ballPos.Y + _nudgeAmount;
		if (_nextBallY > (boundsYLeftEdge + boundsYPaddingCheck))
		{
			_gamemaster->CallOnNudgeBallLeft(_nudgeAmount);
		}
	}
}

void UBowlGameModeComponentCPP::NudgeBallRight()
{
	auto _gamemaster = GetGameMaster();
	float _nudgeAmount = 50;
	if (ensure(_gamemaster != nullptr) && _gamemaster->bCanLaunchBall &&
		_gamemaster->bBowlTurnIsOver == false && myBall != nullptr)
	{
		FVector _ballPos = myBall->GetOwner()->GetActorLocation();
		float _nextBallY = _ballPos.Y + _nudgeAmount;
		if (_nextBallY < (boundsYRightEdge - boundsYPaddingCheck))
		{
			_gamemaster->CallOnNudgeBallRight(_nudgeAmount);
		}
	}
}

void UBowlGameModeComponentCPP::EndBowlingTurn()
{
	auto _gamemaster = GetGameMaster();
	if (ensure(_gamemaster != nullptr) && _gamemaster->bBowlTurnIsOver == false)
	{
		_gamemaster->CallBowlTurnIsFinished();
	}
}

void UBowlGameModeComponentCPP::GetBowlFrameProperties(EBowlFrameCPP bowlframe, int32& bowlAProperty, int32& bowlBProperty, int32& bowlCProperty, int32& bowlResultProperty)
{
	bowlAProperty = 0;
	bowlBProperty = 0;
	bowlCProperty = 0;
	bowlResultProperty = 0;

	switch (bowlframe)
	{
	case EBowlFrameCPP::Frame01:
		bowlAProperty = Frame01_BowlA;
		bowlBProperty = Frame01_BowlB;
		bowlResultProperty = Frame01_Results;
		break;
	case EBowlFrameCPP::Frame02:
		bowlAProperty = Frame02_BowlA;
		bowlBProperty = Frame02_BowlB;
		bowlResultProperty = Frame02_Results;
		break;
	case EBowlFrameCPP::Frame03:
		bowlAProperty = Frame03_BowlA;
		bowlBProperty = Frame03_BowlB;
		bowlResultProperty = Frame03_Results;
		break;
	case EBowlFrameCPP::Frame04:
		bowlAProperty = Frame04_BowlA;
		bowlBProperty = Frame04_BowlB;
		bowlResultProperty = Frame04_Results;
		break;
	case EBowlFrameCPP::Frame05:
		bowlAProperty = Frame05_BowlA;
		bowlBProperty = Frame05_BowlB;
		bowlResultProperty = Frame05_Results;
		break;
	case EBowlFrameCPP::Frame06:
		bowlAProperty = Frame06_BowlA;
		bowlBProperty = Frame06_BowlB;
		bowlResultProperty = Frame06_Results;
		break;
	case EBowlFrameCPP::Frame07:
		bowlAProperty = Frame07_BowlA;
		bowlBProperty = Frame07_BowlB;
		bowlResultProperty = Frame07_Results;
		break;
	case EBowlFrameCPP::Frame08:
		bowlAProperty = Frame08_BowlA;
		bowlBProperty = Frame08_BowlB;
		bowlResultProperty = Frame08_Results;
		break;
	case EBowlFrameCPP::Frame09:
		bowlAProperty = Frame09_BowlA;
		bowlBProperty = Frame09_BowlB;
		bowlResultProperty = Frame09_Results;
		break;
	case EBowlFrameCPP::Frame10:
		bowlAProperty = Frame10_BowlA;
		bowlBProperty = Frame10_BowlB;
		bowlCProperty = Frame10_BowlC;
		bowlResultProperty = Frame10_Results;
		break;
	default:
		break;
	}
}

void UBowlGameModeComponentCPP::CallNewTurnIsReadyAfterWaiting(EBowlActionCPP _action)
{
	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr))
	{
		_gamemaster->CallBowlNewTurnIsReady(_action);
	}
}
#pragma endregion

#pragma region Bowling
EBowlActionCPP UBowlGameModeComponentCPP::Bowl()
{
	int32 _pinFall = GetPinFallCount();
	lastSettledCount = StandingPinCount;
	return BowlHelper(_pinFall);
}

EBowlActionCPP UBowlGameModeComponentCPP::BowlHelper(int32 _pinFall)
{
	//Old Bowl Method
	SetCurrentBowlTurnValue(_pinFall);
	TArray<int32> _rolls = GetBowlTurnListFromCount();
	EBowlActionCPP _action = NextAction(_rolls);
	auto _gamemaster = GetGameMaster();
	if(_rolls.Num() <= 0 || ensure(_gamemaster != nullptr) == false)
	{
		return EBowlActionCPP::Undefined;
	}

	//Old Bowl Method End
	if (BowlTurnCount >= 21 || _action == EBowlActionCPP::EndGame)
	{
		UE_LOG(LogTemp, Warning, TEXT("Won Game From C#"));
		_gamemaster->CallOnWinGame();
	}
	else if (BowlTurnCount >= 19)
	{
		BowlTurnCount += 1;
	}
	//If Action Is Tidy Or Bowlturn is the Second One.
	//Second Turns Are Even Except For the Last Few Turns.
	else if (_action == EBowlActionCPP::Tidy ||
		BowlTurnCount % 2 == 0)
	{
		BowlTurnCount += 1;
	}
	//If Bowl Turn Count Is Not the Second One.
	//Second Turns Are Even Except For the Last Few Turns.
	else if (BowlTurnCount % 2 != 0)
	{
		//Most Likely End Of Turn
		BowlTurnCount += 2;
	}
	return _action;
}

EBowlActionCPP UBowlGameModeComponentCPP::NextAction(TArray<int32> rolls)
{
	EBowlActionCPP nextAction = EBowlActionCPP::Undefined;
	if(ensure(rolls.Num() > 0) == false)
	{
		return nextAction;
	}

	for (int i = 0; i < rolls.Num(); i++)
	{ // Step through rolls

		if (i == 20)
		{
			nextAction = EBowlActionCPP::EndGame;
		}
		else if (i >= 18 && rolls[i] == 10)
		{ // Handle last-frame special cases
			nextAction = EBowlActionCPP::Reset;
		}
		else if (i == 19)
		{
			if (rolls[18] == 10 && rolls[19] == 0)
			{
				nextAction = EBowlActionCPP::Tidy;
			}
			else if (rolls[18] + rolls[19] == 10)
			{
				nextAction = EBowlActionCPP::Reset;
			}
			else if (rolls[18] + rolls[19] >= 10)
			{  // Roll 21 awarded
				nextAction = EBowlActionCPP::Tidy;
			}
			else
			{
				nextAction = EBowlActionCPP::EndGame;
			}
		}
		else if (i % 2 == 0)
		{ // First bowl of frame
			if (rolls[i] == 10)
			{
				//rolls.Insert(i, 0); // Insert virtual 0 after strike
				nextAction = EBowlActionCPP::EndTurn;
			}
			else
			{
				nextAction = EBowlActionCPP::Tidy;
			}
		}
		else
		{ // Second bowl of frame
			nextAction = EBowlActionCPP::EndTurn;
		}
	}

	return nextAction;
}
#pragma endregion
