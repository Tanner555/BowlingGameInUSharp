// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "FloorCaptureTriggerComponentCPP.generated.h"

class UBowlGameMasterComponentCPP;
class UBowlGameModeComponentCPP;


UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UFloorCaptureTriggerComponentCPP : public UActorComponent
{
	GENERATED_BODY()

#pragma region OverridesAndInit
public:
	// Sets default values for this component's properties
	UFloorCaptureTriggerComponentCPP();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;


#pragma endregion

#pragma region ComponentGetters
private:
	UBowlGameMasterComponentCPP* bowlGameMaster;
	UBowlGameModeComponentCPP* bowlGameMode;

	//Component Getters
	UBowlGameMasterComponentCPP* GetGameMaster();
	UBowlGameModeComponentCPP* GetBowlGameMode();
#pragma endregion

#pragma region UFunctions
public:
	UFUNCTION(BlueprintCallable, Category = "FloorCaptureTrigger")
	void OnBeginOverlapWrapper(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, FHitResult SweepResult);
	UFUNCTION(BlueprintCallable, Category = "FloorCaptureTrigger")
	void CallTurnIsFinishedAfterWaiting();
	//Blueprint Emplemented Events
	UFUNCTION(BlueprintImplementableEvent, Category = "BowlPlayer")
	void WaitForPinsToFall();
#pragma endregion

};
