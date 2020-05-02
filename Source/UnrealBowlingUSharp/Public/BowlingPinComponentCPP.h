// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "BowlingPinComponentCPP.generated.h"


class UAudioComponent;
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
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Initialization")
    UStaticMeshComponent* MyColliderMeshComponent;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Initialization")
	UAudioComponent* MyAudioSourceComponent;

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

	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;
#pragma endregion

#pragma region Initialization
protected:
	UFUNCTION(BlueprintCallable, Category = "BowlPinComp")
    void MyBeginPlayInitializer(UStaticMeshComponent* _collidermesh, UAudioComponent* _uaudiocomponent);
	
	void MyBeginPlayPostInitialization();	
#pragma endregion 

#pragma region UFunctions
protected:	
	UFUNCTION(BlueprintCallable, Category = "BowlPinComp")
	void ReceiveHitWrapper(class UPrimitiveComponent* MyComp, AActor* Other, class UPrimitiveComponent* OtherComp, bool bSelfMoved, FVector HitLocation, FVector HitNormal, FVector NormalImpulse, const FHitResult& Hit);
private:
    //Component Getters
    UBowlGameMasterComponentCPP* GetGameMaster();
	UBowlGameModeComponentCPP* GetBowlGameMode();
	UPinManagerComponentCPP* GetPinManager();
	
	UFUNCTION(BlueprintCallable, Category = "BowlPinComp")
	bool SE_CheckForPinHasFallen();
#pragma endregion

#pragma region Handlers
protected:
	UFUNCTION(BlueprintCallable, Category = "BowlHandlers")
	void OnTurnIsFinished();
	UFUNCTION(BlueprintCallable, Category = "BowlHandlers")
	void OnSendBowlActionResults(EBowlActionCPP _action);
	UFUNCTION(BlueprintCallable, Category = "BowlHandlers")
	void NewBowlTurnHasStarted(EBowlActionCPP _action);
	UFUNCTION(BlueprintCallable, Category = "BowlHandlers")
	void OnSimulateStrike();
#pragma endregion

#pragma region OtherMethods
	void PlayPinStrikeSounds(AActor* _other);
	void AttachToParentWithOldPosition();
#pragma endregion 
};
