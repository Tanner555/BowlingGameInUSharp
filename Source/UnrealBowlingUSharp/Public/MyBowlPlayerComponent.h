// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "MyBowlPlayerComponent.generated.h"

enum class EBowlAction : uint8;
class UBowlGameMasterComponent;
class UBowlGameModeComponent;
class UBowlingBallComponent;

UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UMyBowlPlayerComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UMyBowlPlayerComponent();

#pragma region UProperties
public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlPlayer)
	float BallFollowLimitDistance;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlPlayer)
	float DefaultBallFollowOffset;
#pragma endregion

#pragma region Fields
private:
	UBowlGameMasterComponent* bowlGameMaster;
	UBowlGameModeComponent* bowlGameMode;
	FVector MyStartLocation;
	FRotator MyStartRotation;

	bool bShouldFollowBall = false;
	UBowlingBallComponent* myBall;
protected:
	float PawnStartXPoint;
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
	UBowlGameMasterComponent* GetGameMaster();
	UBowlGameModeComponent* GetBowlGameMode();
public:
	//Blueprint Callable
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void OnDragStart(FVector2D mousePos);
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void OnDragStop(FVector2D mousePos);
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void Debug_InstantStrike();
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void Debug_Fill18ScoreSlots();
	//Handlers
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void OnWinGame();
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void NewTurnIsReady(EBowlAction _action);
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void StartFollowingBall(FVector launchVelocity, UBowlingBallComponent* bowlingBall);
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void NudgeBallLeft(float famount);
	UFUNCTION(BlueprintCallable, Category = "BowlPlayer")
	void NudgeBallRight(float famount);
	//Blueprint Emplemented Events
	UFUNCTION(BlueprintImplementableEvent, Category = "BowlPlayer")
	void OnWinGameBPEvent();
#pragma endregion

};
