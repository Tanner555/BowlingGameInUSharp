// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "BowlingPinComponentCPP.generated.h"


class UPinManagerComponentCPP;
enum class EBowlActionCPP : unsigned char;
class UBowlGameModeComponentCPP;
class UBowlGameMasterComponentCPP;
UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UBowlingPinComponentCPP : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBowlingPinComponentCPP();

#pragma region Fields
private:
	
	UBowlGameMasterComponentCPP* bowlGameMaster;
	UBowlGameModeComponentCPP* bowlGameMode;
	UPinManagerComponentCPP* myPinManager;

	bool bPinHasFallen = false;
	float standingThreshold = 15;
	bool bDebugInstantStrike = false;
	float pinStrikeSoundWaitTime = 0.5;
	float offsetAsPercentageMultiplier = 1.5;
#pragma endregion

#pragma region UProperties
protected:	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Bowling")
    UStaticMeshComponent* MyColliderMeshComponent;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category="Initialization")
	USoundBase* PinStrikeSoundVolume1;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category="Initialization")
    USoundBase* PinStrikeSoundVolume2;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category="Initialization")
    USoundBase* PinStrikeSoundVolume3;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category="Initialization")
    USoundBase* PinStrikeSoundVolume4;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category="Initialization")
    USoundBase* PinStrikeSoundVolume5;
#pragma endregion 

#pragma region Overrides
protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;
#pragma endregion

#pragma region UFunctions
private:
    //Component Getters
    UBowlGameMasterComponentCPP* GetGameMaster();
	UBowlGameModeComponentCPP* GetBowlGameMode();
	UPinManagerComponentCPP* GetPinManager();
	
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	bool SE_CheckForPinHasFallen();
#pragma endregion

#pragma region Handlers
protected:
	void OnTurnIsFinished();
	void OnSendBowlActionResults(EBowlActionCPP _action);
	void NewBowlTurnHasStarted(EBowlActionCPP _action);
	void OnSimulateStrike();
#pragma endregion

};
