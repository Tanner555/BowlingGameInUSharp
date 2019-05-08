// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Public/BowlGlobalValues.h"
#include "BowlGameMasterComponent.generated.h"

class UBowlingBallComponent;
class UBowlingPinComponent;
class UBowlGameModeComponent;

class BowlGlobalValues;

#pragma region Delegates
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FGeneralEventHandler);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FVectorAndBallRefHandler, FVector, launchVelocity, UBowlingBallComponent*, bowlingBall);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOneFloatArgHandler, float, famount);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOneIntArgHandler, int32, iamount);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOneBoolArgHandler, bool, _isTrue);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnePinArgHandler, UBowlingPinComponent*, _pin);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FBowlActionArgHandler, EBowlAction, _action);

#pragma endregion


UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UBowlGameMasterComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBowlGameMasterComponent();

#pragma region Fields
private:
	UBowlGameModeComponent* bowlGameMode;
#pragma endregion

#pragma region Events
public:
	UPROPERTY(BlueprintAssignable)
	FBowlActionArgHandler BowlNewTurnIsReady;
	UPROPERTY(BlueprintAssignable)
	FGeneralEventHandler BowlTurnIsFinished;
	UPROPERTY(BlueprintAssignable)
	FVectorAndBallRefHandler OnBallLaunch;
	UPROPERTY(BlueprintAssignable)
	FOneFloatArgHandler OnNudgeBallLeft;
	UPROPERTY(BlueprintAssignable)
	FOneFloatArgHandler OnNudgeBallRight;
	UPROPERTY(BlueprintAssignable)
	FOnePinArgHandler OnPinHasFallen;
	UPROPERTY(BlueprintAssignable)
	FOnePinArgHandler OnPinHasGottenBackUp;
	UPROPERTY(BlueprintAssignable)
	FOneIntArgHandler OnUpdatePinCount;
	UPROPERTY(BlueprintAssignable)
	FBowlActionArgHandler OnSendBowlActionResults;
	UPROPERTY(BlueprintAssignable)
	FGeneralEventHandler OnWinGame;
	//Debug
	UPROPERTY(BlueprintAssignable)
	FGeneralEventHandler Debug_OnSimulateStrike;
	UPROPERTY(BlueprintAssignable)
	FGeneralEventHandler Debug_Fill18ScoreSlots;
#pragma endregion

#pragma region EventCalls
public:
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnBallLaunch(FVector launchVelocity, UBowlingBallComponent* bowlingBall);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallBowlNewTurnIsReady(EBowlAction _action);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallBowlTurnIsFinished();
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnNudgeBallLeft(float famount);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnNudgeBallRight(float famount);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnPinHasFallen(UBowlingPinComponent* _pin);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnPinHasGottenBackUp(UBowlingPinComponent* _pin);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallUpdatePinCount(int32 _count);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnSendBowlActionResults(EBowlAction _action);
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallOnWinGame();
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallDebug_OnSimulateStrike();
	UFUNCTION(BlueprintCallable, Category = "Bowl Events")
		void CallDebug_Fill18ScoreSlots();
#pragma endregion

#pragma region UFunctions
public:
	//Getters
	UBowlGameModeComponent* GetBowlGameMode();
#pragma endregion

#pragma region UProperties
public:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GameMasterProperties")
		bool bBowlTurnIsOver;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GameMasterProperties")
		bool bCanLaunchBall;
#pragma endregion

#pragma region Overrides
protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	void EndPlay(const EEndPlayReason::Type EndPlayReason) override;
#pragma endregion

};
