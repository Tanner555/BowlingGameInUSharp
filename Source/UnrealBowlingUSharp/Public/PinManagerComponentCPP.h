// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "PinManagerComponentCPP.generated.h"


enum class EBowlActionCPP : unsigned char;
class UBowlingPinComponentCPP;
class UBowlGameMasterComponentCPP;
class UBowlGameModeComponentCPP;
UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UPinManagerComponentCPP : public UActorComponent
{
	GENERATED_BODY()

#pragma region UPropsAndFields
private:
	//Comps
    UBowlGameMasterComponentCPP* bowlGameMaster;
	UBowlGameModeComponentCPP* bowlGameMode;
	//Other
	TArray<FVector> PinLocations;
	TMap<FString, bool> AllPinsStandingDictionary;
	UClass* PinPrefabClass;
#pragma endregion 

#pragma region CompAndOtherGetters
protected:
	UBowlGameMasterComponentCPP* GetGameMaster();
	UBowlGameModeComponentCPP* GetBowlGameMode();
	UFUNCTION(BlueprintCallable, Category="PinManager")
	TArray<AActor*> GetAllPins();
#pragma endregion 
	
#pragma region InitAndOverrides
public:	
	// Sets default values for this component's properties
	UPinManagerComponentCPP();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;


	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;
#pragma endregion

#pragma region Handlers
	UFUNCTION(BlueprintCallable, Category="PinManager")
	void PinHasFallen(UBowlingPinComponentCPP* _pin);
	UFUNCTION(BlueprintCallable, Category="PinManager")
	void PinGottenBackUp(UBowlingPinComponentCPP* _pin);
	UFUNCTION(BlueprintCallable, Category="PinManager")
	void BowlNewTurnIsReady(EBowlActionCPP _action);
	
#pragma endregion

#pragma region Spawn-Attach-Pins
public:
	UFUNCTION(BlueprintCallable, Category="PinManager")
	TArray<AActor*> RespawnPins();
	UFUNCTION(BlueprintCallable, Category="PinManager")
	AActor* SpawnPin(FVector _pinLocation);
	UFUNCTION(BlueprintCallable, Category="PinManager")
	void AttachPinToManager(AActor* _pin);
#pragma endregion

#pragma region PinFallenDictionaryHandling
protected:
	void InitializePinStandingDictionary(TArray<AActor*> pinActors);
	void UpdatePinHasStandingDictionary(UBowlingPinComponentCPP* _pin, bool _fallen);
#pragma endregion 
};
