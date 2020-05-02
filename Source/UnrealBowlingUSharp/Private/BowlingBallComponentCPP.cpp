// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlingBallComponentCPP.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/GameModeBase.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "Sound/SoundBase.h"
#include "Components/StaticMeshComponent.h"
#include "Components/AudioComponent.h"

#pragma region Component Getters
UBowlGameMasterComponentCPP* UBowlingBallComponentCPP::GetGameMaster()
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

UBowlGameModeComponentCPP* UBowlingBallComponentCPP::GetBowlGameMode()
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
void UBowlingBallComponentCPP::MyBeginPlayInitializer(UStaticMeshComponent* _mymeshcomponent,
	UAudioComponent* _myaudiosourcecomponent)
{
	this->MyMeshComponent = _mymeshcomponent;
	this->MyAudioSourceComponent = _myaudiosourcecomponent;
}

// Sets default values for this component's properties
UBowlingBallComponentCPP::UBowlingBallComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UBowlingBallComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr) == false)
	{
		return;
	}

	if (ensure(_gamemaster->BowlTurnIsFinished.IsAlreadyBound(this, &UBowlingBallComponentCPP::BowlTurnIsFinished) == false))
	{
		_gamemaster->BowlTurnIsFinished.AddDynamic(this, &UBowlingBallComponentCPP::BowlTurnIsFinished);
	}
	if (ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UBowlingBallComponentCPP::NewTurnIsReady) == false))
	{
		_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UBowlingBallComponentCPP::NewTurnIsReady);
	}
	if (ensure(_gamemaster->OnBallLaunch.IsAlreadyBound(this, &UBowlingBallComponentCPP::LaunchBall) == false))
	{
		_gamemaster->OnBallLaunch.AddDynamic(this, &UBowlingBallComponentCPP::LaunchBall);
	}
	if (ensure(_gamemaster->OnNudgeBallLeft.IsAlreadyBound(this, &UBowlingBallComponentCPP::NudgeBallLeft) == false))
	{
		_gamemaster->OnNudgeBallLeft.AddDynamic(this, &UBowlingBallComponentCPP::NudgeBallLeft);
	}
	if (ensure(_gamemaster->OnNudgeBallRight.IsAlreadyBound(this, &UBowlingBallComponentCPP::NudgeBallRight) == false))
	{
		_gamemaster->OnNudgeBallRight.AddDynamic(this, &UBowlingBallComponentCPP::NudgeBallRight);
	}

	MyStartLocation = GetOwner()->GetActorLocation();
	MyStartRotation = GetOwner()->GetActorRotation();
}

// Called every frame
void UBowlingBallComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}
#pragma endregion

#pragma region Handlers
void UBowlingBallComponentCPP::NewTurnIsReady(EBowlActionCPP _action)
{
	if (MyMeshComponent == nullptr) return;

	GetOwner()->SetActorLocation(MyStartLocation, false);
	GetOwner()->SetActorRotation(MyStartRotation);

	MyMeshComponent->SetSimulatePhysics(false);
	MyMeshComponent->SetSimulatePhysics(true);
}

void UBowlingBallComponentCPP::BowlTurnIsFinished()
{

}

void UBowlingBallComponentCPP::LaunchBall(FVector _launchVelocity, UBowlingBallComponentCPP* bowlingBall)
{
	if (MyMeshComponent == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("Please Assign A mesh component to the uproperty"));
		return;
	}
	else if (MyAudioSourceComponent == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("Please Assign an audio component to the uproperty"));
		return;
	}
	else if (BallRollingSound == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("Please Assign a sound clip to the ball rolling sound uproperty"));
		return;
	}
	else
	{
		//UE_LOG(LogTemp, Warning, TEXT("LaunchBall: Launch Velocity X: %f. Launch Velocity Y: %f"), _launchVelocity.X, _launchVelocity.Y);
		MyMeshComponent->AddImpulse(LaunchVelocity, MyMeshComponent->GetAttachSocketName(), true);
		MyAudioSourceComponent->Sound = BallRollingSound;
		MyAudioSourceComponent->Play();
	}
}

void UBowlingBallComponentCPP::NudgeBallLeft(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}

void UBowlingBallComponentCPP::NudgeBallRight(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}
#pragma endregion