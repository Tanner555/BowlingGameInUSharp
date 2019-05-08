// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "FloorCaptureTriggerComponent.generated.h"

class UBowlGameMasterComponent;
class UBowlGameModeComponent;


UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UFloorCaptureTriggerComponent : public UActorComponent
{
	GENERATED_BODY()

#pragma region OverridesAndInit
public:
	// Sets default values for this component's properties
	UFloorCaptureTriggerComponent();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;


#pragma endregion

#pragma region ComponentGetters
private:
	UBowlGameMasterComponent* bowlGameMaster;
	UBowlGameModeComponent* bowlGameMode;

	//Component Getters
	UBowlGameMasterComponent* GetGameMaster();
	UBowlGameModeComponent* GetBowlGameMode();
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
