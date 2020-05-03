// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "BowlGameInstanceCPP.generated.h"

/**
 * 
 */
UCLASS()
class UNREALBOWLINGUSHARP_API UBowlGameInstanceCPP : public UGameInstance
{
	GENERATED_BODY()

public:
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category="MyScriptingPlugins")
	bool bUseUSharpCode;
};
