// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Public/BowlGlobalValuesCPP.h"
#include "Components/ActorComponent.h"
#include "BowlGameModeComponentCPP.generated.h"

class ULevelSequence;
class UBowlGameMasterComponentCPP;
class APlayerCameraManager;
class UBowlingBallComponentCPP;
class UMyBowlPlayerComponentCPP;
class AStaticMeshActor;

UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UBowlGameModeComponentCPP : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBowlGameModeComponentCPP();

#pragma region Fields
private:
	UBowlGameMasterComponentCPP* bowlGameMaster;
	//static UBowlGameModeComponent* thisInstance;

public:
	FName BallTag = FName("Ball");
	FName PinTag = FName("Pin");
	FName PinManagerTag = FName("PinManager");
	FName BowlingFloorTag = FName("BowlingFloor");

protected:
	APlayerCameraManager* myCameraManager = nullptr;
	UBowlingBallComponentCPP* myBall = nullptr;
	UMyBowlPlayerComponentCPP* myBowler = nullptr;

private:
	AStaticMeshActor* BowlFloorMeshActor = nullptr;
	FVector2D dragStart, dragEnd;
	float startTime, endTime;
	float boundsYLeftEdge, boundsYRightEdge;
	float boundsYPaddingCheck = 10.0f;

public:
	int32 lastSettledCount = 10;

#pragma endregion

#pragma region BowlTurnFields
public:
	int32 Frame01_BowlA;
	int32 Frame01_BowlB;
	int32 Frame01_Results;
	int32 Frame02_BowlA;
	int32 Frame02_BowlB;
	int32 Frame02_Results;
	int32 Frame03_BowlA;
	int32 Frame03_BowlB;
	int32 Frame03_Results;
	int32 Frame04_BowlA;
	int32 Frame04_BowlB;
	int32 Frame04_Results;
	int32 Frame05_BowlA;
	int32 Frame05_BowlB;
	int32 Frame05_Results;
	int32 Frame06_BowlA;
	int32 Frame06_BowlB;
	int32 Frame06_Results;
	int32 Frame07_BowlA;
	int32 Frame07_BowlB;
	int32 Frame07_Results;
	int32 Frame08_BowlA;
	int32 Frame08_BowlB;
	int32 Frame08_Results;
	int32 Frame09_BowlA;
	int32 Frame09_BowlB;
	int32 Frame09_Results;
	int32 Frame10_BowlA;
	int32 Frame10_BowlB;
	int32 Frame10_BowlC;
	int32 Frame10_Results;
#pragma endregion

#pragma region UProperties
protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = BowlGameMode)
	int32 StandingPinCount;
public:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = BowlGameMode)
	int32 BowlTurnCount;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlGameMode)
	float MinimalForwardLaunchVelocity;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlGameMode)
	float ForwardMultipleVelocityFactor;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlGameMode)
	ULevelSequence* CleanUpSweepLevelSequence;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlGameMode)
	ULevelSequence* ClearSweepLevelSequence;

	UPROPERTY(BlueprintReadOnly, Category=BowlingPinStats)
	bool bHitFirstPin;

#pragma endregion

#pragma region Overrides
public:
	void InitializeComponent() override;
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	void EndPlay(const EEndPlayReason::Type EndPlayReason) override;
#pragma endregion

#pragma region UFunctions
public:
	//Component Getters
	UBowlGameMasterComponentCPP* GetGameMaster();
	//Other Getters
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	int32 GetStandingPinCount();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	TArray<int32> GetAllBowlFrameResults();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	TArray<int32> GetAllBowlFrameTurns();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	int32 GetPinFallCount();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	TArray<int32> GetBowlTurnListFromCount();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	int32 GetBowlTurnCount();
	//Setters
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void SetStandingPinCount(int32 pinCount);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void SetResultsFromFrameTurns();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void SetCurrentBowlTurnValue(int32 _value);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void SetHitFirstPin(bool _enable);
	//PublicUFunctionCalls
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void NudgeBallLeft();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void NudgeBallRight();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void EndBowlingTurn();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void GetBowlFrameProperties(EBowlFrameCPP bowlframe, int32& bowlAProperty, int32& bowlBProperty, int32& bowlCProperty, int32& bowlResultProperty);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void CallNewTurnIsReadyAfterWaiting(EBowlActionCPP _action);
	//Handlers
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void OnSendBowlActionResults(EBowlActionCPP _action);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void OnTurnIsFinished();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void UpdatePinCount(int32 _pinCount);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void ResetPinCount(EBowlActionCPP _action);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void Debug_Fill18ScoreSlots();

	//DraggingAndBallLaunch
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void OnStartDrag(FVector2D mousePos);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void OnStopDrag(FVector2D mousePos);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	void StartLaunchingTheBall(FVector launchVelocity);
	//Blueprint Emplemented Events
	UFUNCTION(BlueprintImplementableEvent, Category = "BowlGameMode")
	void UpdatePinCountBPEvent(int32 pinCount);
	UFUNCTION(BlueprintImplementableEvent, Category = "BowlGameMode")
	void UpdateBowlTurnFramesBPEvent();
	//Temporary Wrapper Because Implementing This in BP will be much easier than C++
	UFUNCTION(BlueprintImplementableEvent, Category = "BowlGameMode")
	void SendBowlActionResultsAndWaitBPEvent(EBowlActionCPP _action);
	//Bowling
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	EBowlActionCPP Bowl();
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	EBowlActionCPP BowlHelper(int32 _pinFall);
	UFUNCTION(BlueprintCallable, Category = "BowlGameMode")
	EBowlActionCPP NextAction(TArray<int32> rolls);
#pragma endregion

};
