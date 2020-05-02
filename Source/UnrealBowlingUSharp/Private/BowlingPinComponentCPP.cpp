// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlingPinComponentCPP.h"
#include "BowlGameMasterComponentCPP.h"
#include "BowlGameModeComponentCPP.h"
#include "PinManagerComponentCPP.h"
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
			//AttachToParentWithOldPosition();
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
				//PlayPinStrikeSounds(Other);
			}
		}
	}
}

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
