// Fill out your copyright notice in the Description page of Project Settings.


#include "Public/BowlingPinComponentCPP.h"

// Sets default values for this component's properties
UBowlingPinComponentCPP::UBowlingPinComponentCPP()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}

#pragma region Overrides
// Called when the game starts
void UBowlingPinComponentCPP::BeginPlay()
{
	Super::BeginPlay();

	// ...

}


// Called every frame
void UBowlingPinComponentCPP::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}
#pragma endregion

#pragma region UFunctions
bool UBowlingPinComponentCPP::SE_CheckForPinHasFallen()
{
	return false;
}
#pragma endregion