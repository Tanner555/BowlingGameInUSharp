// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlingPinComponentCPP.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "PinManagerComponentCPP.h"
#include "Components/AudioComponent.h"
#include "Components/StaticMeshComponent.h"
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

	auto _gamemaster = GetGameMaster();
	auto _gamemode = GetBowlGameMode();

	//if gamemode or gamemaster are null, return.
	if(ensure(_gamemaster != nullptr && _gamemode != nullptr) == false) return;

	//Bind Handlers
	if(ensure(_gamemaster->BowlTurnIsFinished.IsAlreadyBound(this, &UBowlingPinComponentCPP::OnTurnIsFinished) == false))
	{
		_gamemaster->BowlTurnIsFinished.AddDynamic(this, &UBowlingPinComponentCPP::OnTurnIsFinished);
	}

	if(ensure(_gamemaster->OnWinGame.IsAlreadyBound(this, &UBowlingPinComponentCPP::OnTurnIsFinished) == false))
	{
		_gamemaster->OnWinGame.AddDynamic(this, &UBowlingPinComponentCPP::OnTurnIsFinished);
	}
	
	if(ensure(_gamemaster->OnSendBowlActionResults.IsAlreadyBound(this, &UBowlingPinComponentCPP::OnSendBowlActionResults) == false))
	{
		_gamemaster->OnSendBowlActionResults.AddDynamic(this, &UBowlingPinComponentCPP::OnSendBowlActionResults);
	}
	
	if(ensure(_gamemaster->BowlNewTurnIsReady.IsAlreadyBound(this, &UBowlingPinComponentCPP::NewBowlTurnHasStarted) == false))
	{
		_gamemaster->BowlNewTurnIsReady.AddDynamic(this, &UBowlingPinComponentCPP::NewBowlTurnHasStarted);
	}
	
	if(ensure(_gamemaster->Debug_OnSimulateStrike.IsAlreadyBound(this, &UBowlingPinComponentCPP::OnSimulateStrike) == false))
	{
		_gamemaster->Debug_OnSimulateStrike.AddDynamic(this, &UBowlingPinComponentCPP::OnSimulateStrike);
	}
	
	if(_gamemode->bHitFirstPin == true)
	{
		_gamemode->SetHitFirstPin(false);
	}
}

// Called every frame
void UBowlingPinComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UBowlingPinComponentCPP::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	auto _gamemaster = GetGameMaster();

	//if gamemode or gamemaster are null, return.
	if(_gamemaster == nullptr) return;
		
	//Pin should have fallen
	//Checking Just In Case
	if (bPinHasFallen == false)
	{
		bPinHasFallen = true;
		_gamemaster->CallOnPinHasFallen(this);
	}
	
}
#pragma endregion

#pragma region Initialization
void UBowlingPinComponentCPP::MyBeginPlayInitializer(UStaticMeshComponent* _collidermesh,
    UAudioComponent* _uaudiocomponent)
{
	MyColliderMeshComponent = _collidermesh;
	MyAudioSourceComponent = _uaudiocomponent;
	MyBeginPlayPostInitialization();
}

void UBowlingPinComponentCPP::MyBeginPlayPostInitialization()
{
	auto _pinManager = GetPinManager();
	if (_pinManager != nullptr)
	{
		if (MyColliderMeshComponent != nullptr)
		{
			AttachToParentWithOldPosition();
			MyColliderMeshComponent->SetSimulatePhysics(false);
			MyColliderMeshComponent->SetSimulatePhysics(true);
		}
		else
		{
			UE_LOG(LogTemp, Warning, TEXT("Please Attach Collider Mesh To Pin Comp UProperty"));
		}
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("Couldn't Find Pin Manager Component"));
	}
}
#pragma endregion 

#pragma region UFunctions
void UBowlingPinComponentCPP::ReceiveHitWrapper(UPrimitiveComponent* MyComp, AActor* Other,
    UPrimitiveComponent* OtherComp, bool bSelfMoved, FVector HitLocation, FVector HitNormal, FVector NormalImpulse,
    const FHitResult& Hit)
{
	auto _gamemaster = GetGameMaster();
	auto _gamemode = GetBowlGameMode();

	//if gamemode or gamemaster are null, return.
	if(ensure(_gamemaster != nullptr && _gamemode != nullptr) == false) return;
	
	if (Other != nullptr && Other->ActorHasTag(_gamemode->BallTag))
	{
		if(_gamemode->bHitFirstPin == false)
		{
			_gamemode->SetHitFirstPin(true);
			
			if (MyAudioSourceComponent == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("Please Assign an audio component to the uproperty"));
			}
			else if (PinStrikeSoundVolume1 == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("Please Assign a sound clip to the PinStrikeSoundVolume1 sound uproperty"));		
			}
			else if (PinStrikeSoundVolume2 == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("Please Assign a sound clip to the PinStrikeSoundVolume2 sound uproperty"));
			}
			else if (PinStrikeSoundVolume3 == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("Please Assign a sound clip to the PinStrikeSoundVolume3 sound uproperty"));
			}
			else if (PinStrikeSoundVolume4 == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("Please Assign a sound clip to the PinStrikeSoundVolume4 sound uproperty"));
			}
			else if (PinStrikeSoundVolume5 == nullptr)
			{
				UE_LOG(LogTemp, Warning, TEXT("Please Assign a sound clip to the PinStrikeSoundVolume5 sound uproperty"));
			}
			else
			{
				PlayPinStrikeSounds(Other);
			}
		}
	}
}

bool UBowlingPinComponentCPP::SE_CheckForPinHasFallen()
{	
	auto _gamemaster = GetGameMaster();
	if(_gamemaster == nullptr) return false;
	FVector _rotationInEuler = GetOwner()->GetActorRotation().Euler();
	float _tiltInX = FMath::Abs(_rotationInEuler.X);
	float _tiltInY = FMath::Abs(_rotationInEuler.Y);
	bool _previouslyFallen = bPinHasFallen;
	bPinHasFallen = _tiltInX > standingThreshold || _tiltInY > standingThreshold;

	if (bDebugInstantStrike)
	{
		bPinHasFallen = true;
	}

	if (bPinHasFallen && _previouslyFallen != bPinHasFallen)
	{
		//If Pin Has Fallen, Call Event
		_gamemaster->CallOnPinHasFallen(this);
	}
	else if(bPinHasFallen == false && 
        _previouslyFallen != bPinHasFallen &&
        bDebugInstantStrike == false)
	{
		//If Pin Has Gotten Back Up Because
		//Pin Has Fallen, But Now PinHasFallen Equals False
		_gamemaster->CallOnPinHasGottenBackUp(this);
	}

	return bPinHasFallen;
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
	auto _pinManager = GetPinManager();
	if(ensure(_pinManager != nullptr) == false) return;
	
	//Only If Collider Mesh Comp Has Been Assigned AND
	//Parent Actor is the PinManager Actor Blueprint
	if (MyColliderMeshComponent != nullptr &&
        _pinManager != nullptr &&
        bPinHasFallen == false)
	{
		if (_action == EBowlActionCPP::Tidy)
		{
			AttachToParentWithOldPosition();
			MyColliderMeshComponent->SetSimulatePhysics(false);
		}
		else
		{
			MyColliderMeshComponent->SetSimulatePhysics(true);
		}
	}
}

void UBowlingPinComponentCPP::NewBowlTurnHasStarted(EBowlActionCPP _action)
{
	auto _gamemode = GetBowlGameMode();
	if(ensure(_gamemode != nullptr) == false) return;
	
	if(_gamemode->bHitFirstPin == true)
	{
		_gamemode->SetHitFirstPin(false);
	}

	if (bPinHasFallen)
	{
		//Destroy Pin If It Hasn't Been Sweeped Into the Floor
		GetOwner()->Destroy();
	}
	else if (MyColliderMeshComponent != nullptr)
	{
		MyColliderMeshComponent->SetSimulatePhysics(true);
	}
}

//Debug
void UBowlingPinComponentCPP::OnSimulateStrike()
{
	bDebugInstantStrike = true;
}
#pragma endregion

#pragma region OtherMethods
void UBowlingPinComponentCPP::PlayPinStrikeSounds(AActor* _other)
{
	auto _gamemode = GetBowlGameMode();
	if(_gamemode == nullptr || _other == nullptr) return;
	
	int _settledPins = _gamemode->lastSettledCount;
	float _center = 50;
	float _otherYLoc = _other->GetActorLocation().Y;
	float _offset = _otherYLoc >= 0 ?
        FMath::Abs(_otherYLoc - _center) : FMath::Abs(_otherYLoc + _center);
	float _highoffset = 350;
	float _offsetAsPercentageDecimal = (_offset / _highoffset);
	float _highVelX = 2500;
	float _lowVelX = 1000;
	float _velX = FMath::Clamp(_other->GetVelocity().X, _lowVelX, _highVelX);
	float _velXAsPercentageDecimal = ((_velX - _lowVelX) / (_highVelX - _lowVelX));
	//The Less Pins, The Higher The Percentage
	float _pinSettledPenaltyAsPercentage = 1 - (_settledPins / 10);
	float _VelXMinusOffsetYDecimal = _velXAsPercentageDecimal - (_offsetAsPercentageDecimal * offsetAsPercentageMultiplier) - _pinSettledPenaltyAsPercentage;
	if (_offsetAsPercentageDecimal > 0.80f)
	{
		_VelXMinusOffsetYDecimal = FMath::Min(-0.5f, _VelXMinusOffsetYDecimal - 0.5f);
	}

	//MyOwner.PrintString($"Pin Strike. Vel: {_velX} VelAsPerc: {_velXAsPercentageDecimal} Off: {_offset} OffAsPerc: {_offsetAsPercentageDecimal} VelMinusOffset: {_VelXMinusOffsetYDecimal}", FLinearColor.Green, printToLog: true);
	
	if(_VelXMinusOffsetYDecimal <= -0.5)
	{
		MyAudioSourceComponent->Sound = FMath::RandRange(0, 1) == 0 ?
                PinStrikeSoundVolume1 : PinStrikeSoundVolume2;
	}
	else if(_VelXMinusOffsetYDecimal <= 0.2)
	{
		MyAudioSourceComponent->Sound = FMath::RandRange(0, 1) == 0 ?
                PinStrikeSoundVolume3 : PinStrikeSoundVolume4;
	}
	else
	{
		MyAudioSourceComponent->Sound = PinStrikeSoundVolume5;
	}

	MyAudioSourceComponent->Play();
}

void UBowlingPinComponentCPP::AttachToParentWithOldPosition()
{
	const auto _pinManager = GetPinManager();
	if (_pinManager == nullptr) return;

	const auto _pinManagerBP = _pinManager->GetOwner();
	if(_pinManagerBP == nullptr) return;
	
	FHitResult* _hit = nullptr;
	const FVector _oldLoc = GetOwner()->GetActorLocation();
	const FRotator _oldRot = GetOwner()->GetActorRotation();

	GetOwner()->AttachToActor(_pinManagerBP,
		FAttachmentTransformRules(EAttachmentRule::KeepRelative,
			EAttachmentRule::KeepRelative, EAttachmentRule::KeepWorld, true),
		_pinManagerBP->GetAttachParentSocketName());
	GetOwner()->SetActorLocation(_oldLoc, false, _hit, ETeleportType::None);
	GetOwner()->SetActorRotation(_oldRot, ETeleportType::None);
}
#pragma endregion