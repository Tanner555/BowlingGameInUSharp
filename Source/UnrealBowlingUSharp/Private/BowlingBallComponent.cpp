// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlingBallComponent.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/GameModeBase.h"
#include "BowlGameMasterComponent.h"
#include "BowlGameModeComponent.h"
#include "Sound/SoundBase.h"
#include "Components/StaticMeshComponent.h"
#include "Components/AudioComponent.h"

#pragma region Component Getters
UBowlGameMasterComponent* UBowlingBallComponent::GetGameMaster()
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

UBowlGameModeComponent* UBowlingBallComponent::GetBowlGameMode()
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

#pragma region InitAndOverrides
void UBowlingBallComponent::MyBeginPlayInitializer(UStaticMeshComponent* _mymeshcomponent,
	UAudioComponent* _myaudiosourcecomponent)
{
	this->MyMeshComponent = _mymeshcomponent;
	this->MyAudioSourceComponent = _myaudiosourcecomponent;
}

// Sets default values for this component's properties
UBowlingBallComponent::UBowlingBallComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UBowlingBallComponent::BeginPlay()
{
	Super::BeginPlay();

	auto _gamemaster = GetGameMaster();
	if(ensure(_gamemaster != nullptr) == false)
	{
		return;
	}

	if (ensure(_gamemaster->BowlTurnIsFinished.IsAlreadyBound(this, &UBowlingBallComponent::BowlTurnIsFinished) == false))
	{
		_gamemaster->BowlTurnIsFinished.AddDynamic(this, &UBowlingBallComponent::BowlTurnIsFinished);
	}
	if (ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UBowlingBallComponent::NewTurnIsReady) == false))
	{
		_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UBowlingBallComponent::NewTurnIsReady);
	}
	if (ensure(_gamemaster->OnBallLaunch.IsAlreadyBound(this, &UBowlingBallComponent::LaunchBall) == false))
	{
		if (bLaunchBallThroughBlueprints == false) {
			_gamemaster->OnBallLaunch.AddDynamic(this, &UBowlingBallComponent::LaunchBall);
		}
	}
	if (ensure(_gamemaster->OnNudgeBallLeft.IsAlreadyBound(this, &UBowlingBallComponent::NudgeBallLeft) == false))
	{
		_gamemaster->OnNudgeBallLeft.AddDynamic(this, &UBowlingBallComponent::NudgeBallLeft);
	}
	if (ensure(_gamemaster->OnNudgeBallRight.IsAlreadyBound(this, &UBowlingBallComponent::NudgeBallRight) == false))
	{
		_gamemaster->OnNudgeBallRight.AddDynamic(this, &UBowlingBallComponent::NudgeBallRight);
	}

	MyStartLocation = GetOwner()->GetActorLocation();
	MyStartRotation = GetOwner()->GetActorRotation();
}

// Called every frame
void UBowlingBallComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}
#pragma endregion

#pragma region Handlers
void UBowlingBallComponent::NewTurnIsReady(EBowlAction _action)
{
	if (MyMeshComponent == nullptr) return;

	GetOwner()->SetActorLocation(MyStartLocation, false);
	GetOwner()->SetActorRotation(MyStartRotation);

	MyMeshComponent->SetSimulatePhysics(false);
	MyMeshComponent->SetSimulatePhysics(true);
}

void UBowlingBallComponent::BowlTurnIsFinished()
{

}

void UBowlingBallComponent::LaunchBall(FVector _launchVelocity, UBowlingBallComponent* bowlingBall)
{
	if (bLaunchBallThroughBlueprints) return;

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

void UBowlingBallComponent::NudgeBallLeft(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}

void UBowlingBallComponent::NudgeBallRight(float famount)
{
	GetOwner()->SetActorLocation(
		GetOwner()->GetActorLocation() + FVector(0, famount, 0),
		false
	);
}
#pragma endregion