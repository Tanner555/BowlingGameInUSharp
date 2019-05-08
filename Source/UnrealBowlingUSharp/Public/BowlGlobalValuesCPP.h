// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#pragma region GlobalEnums
UENUM(BlueprintType)
enum class EBowlFrameCPP : uint8 {
	Frame01 = 0,
	Frame02 = 1,
	Frame03 = 2,
	Frame04 = 3,
	Frame05 = 4,
	Frame06 = 5,
	Frame07 = 6,
	Frame08 = 7,
	Frame09 = 8,
	Frame10 = 9
};

UENUM(BlueprintType)
enum class EBowlActionCPP : uint8 {
	Tidy = 0,
	Reset = 1,
	EndTurn = 2,
	EndGame = 3,
	Undefined = 4
};
#pragma endregion

/**
 * 
 */
class UNREALBOWLINGUSHARP_API BowlGlobalValuesCPP
{
public:
	BowlGlobalValuesCPP();
	~BowlGlobalValuesCPP();
};
