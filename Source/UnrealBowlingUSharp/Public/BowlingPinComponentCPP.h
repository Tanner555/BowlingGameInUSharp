// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "BowlingPinComponentCPP.generated.h"


UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UBowlingPinComponentCPP : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBowlingPinComponentCPP();

#pragma region Overrides
protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;
#pragma endregion

#pragma region UFunctions
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	bool SE_CheckForPinHasFallen();
#pragma endregion

};
