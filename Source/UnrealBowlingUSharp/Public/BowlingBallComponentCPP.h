// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "BowlingBallComponentCPP.generated.h"

class UAudioComponent;
class UBowlGameMasterComponentCPP;
class UBowlGameModeComponentCPP;
enum class EBowlActionCPP : uint8;

UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UBowlingBallComponentCPP : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBowlingBallComponentCPP();

#pragma region Fields
private:
	UBowlGameMasterComponentCPP* bowlGameMaster;
	UBowlGameModeComponentCPP* bowlGameMode;

	FHitResult myHit;
	FVector MyStartLocation;
	FRotator MyStartRotation;

	UStaticMeshComponent* MyMeshComponent;
	UAudioComponent* MyAudioSourceComponent;
#pragma endregion

#pragma region UProperties
public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlingBall)
	USoundBase* BallRollingSound;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlingBall)
    USoundBase* BallNudgeSound;
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
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	UBowlGameMasterComponentCPP* GetGameMaster();
	UBowlGameModeComponentCPP* GetBowlGameMode();
protected:
	//Handlers
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void NewTurnIsReady(EBowlActionCPP _action);
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void BowlTurnIsFinished();
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void LaunchBallCPP(FVector _launchVelocity, UBowlingBallComponentCPP* bowlingBall);
	UFUNCTION(BlueprintImplementableEvent, Category = "BowlingBall")
	void LaunchBallBPImplementation(FVector _launchVelocity, UBowlingBallComponentCPP* bowlingBall);
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void NudgeBallLeft(float famount);
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void NudgeBallRight(float famount);
	//Initialization
	//Used To Set MyMeshComponent and MyAudioSourceComponent Variables
	//Should be called from Blueprints
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void MyBeginPlayInitializer(UStaticMeshComponent* _mymeshcomponent, UAudioComponent* _myaudiosourcecomponent);
	//Other
public:
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void StopRollingSound();
#pragma endregion

		
};
