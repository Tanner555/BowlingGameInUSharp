// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "BowlingBallComponent.generated.h"

class UAudioComponent;
class UBowlGameMasterComponent;
class UBowlGameModeComponent;
enum class EBowlAction : uint8;

UCLASS(Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALBOWLINGUSHARP_API UBowlingBallComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBowlingBallComponent();

#pragma region Fields
private:
	UBowlGameMasterComponent* bowlGameMaster;
	UBowlGameModeComponent* bowlGameMode;

	FHitResult myHit;
	FVector MyStartLocation;
	FRotator MyStartRotation;

	UStaticMeshComponent* MyMeshComponent;
	UAudioComponent* MyAudioSourceComponent;
#pragma endregion

#pragma region UProperties
public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlingBall)
	FVector LaunchVelocity;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlingBall)
	USoundBase* BallRollingSound;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = BowlingBall)
	bool bLaunchBallThroughBlueprints;
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
	UBowlGameMasterComponent* GetGameMaster();
	UBowlGameModeComponent* GetBowlGameMode();
protected:
	//Handlers
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void NewTurnIsReady(EBowlAction _action);
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void BowlTurnIsFinished();
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void LaunchBall(FVector _launchVelocity, UBowlingBallComponent* bowlingBall);
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void NudgeBallLeft(float famount);
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void NudgeBallRight(float famount);
	//Initialization
	//Used To Set MyMeshComponent and MyAudioSourceComponent Variables
	//Should be called from Blueprints
	UFUNCTION(BlueprintCallable, Category = "BowlingBall")
	void MyBeginPlayInitializer(UStaticMeshComponent* _mymeshcomponent, UAudioComponent* _myaudiosourcecomponent);
#pragma endregion

		
};
