using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using UnrealEngine.Slate;
using UnrealEngine;
using UnrealEngine.GameplayTasks;
using UnrealEngine.SlateCore;
using UnrealEngine.NavigationSystem;
using System.Collections;

namespace HelloUSharp
{
    [UClass, Blueprintable, BlueprintType]
    public class BowlingFloorCaptureTriggerComponent : UActorComponent
    {
        #region IgnoreProperties
        [UPropertyIngore]
        public AActor MyOwner
        {
            get
            {
                if (_owner == null)
                    _owner = GetOwner();

                return _owner;
            }
        }
        private AActor _owner = null;

        [UPropertyIngore]
        protected BowlGameMasterComponent gamemaster => BowlGameMasterComponent.GetInstance(MyOwner);
        [UPropertyIngore]
        protected BowlGameModeComponent gamemode => BowlGameModeComponent.GetInstance(MyOwner);
        #endregion

        #region Fields
        private float waitPeriod = 3.0f;
        #endregion

        #region Overrides
        public override void BeginPlay()
        {
            gamemaster.BowlTurnIsFinished += CancelTriggerCoroutines;
        }

        public override void EndPlay(EEndPlayReason endPlayReason)
        {
            if (gamemaster != null)
            {
                gamemaster.BowlTurnIsFinished -= CancelTriggerCoroutines;
            }
        }
        #endregion

        #region Handlers
        void CancelTriggerCoroutines()
        {
            StopAllCoroutines();
        }
        #endregion

        [UFunction, BlueprintCallable]
        public void OnBeginOverlapWrapper(UPrimitiveComponent OverlappedComp, AActor OtherActor, UPrimitiveComponent OtherComp, int OtherBodyIndex, bool bFromSweep, FHitResult SweepResult)
        {
            if (OtherActor != null)
            {
                if (OtherActor.ActorHasTag(gamemode.BallTag) &&
                    gamemaster.bBowlTurnIsOver == false)
                {
                    StartCoroutine(this, WaitForPinsToFall());
                }
                else if (OtherActor.ActorHasTag(gamemode.PinTag))
                {
                    OtherActor.DestroyActor();
                }
            }
        }

        IEnumerator WaitForPinsToFall()
        {
            yield return new WaitForSeconds(waitPeriod);
            CallTurnIsFinishedAfterWaiting();
        }

        void CallTurnIsFinishedAfterWaiting()
        {
            if (gamemaster.bBowlTurnIsOver == false)
            {
                gamemaster.CallBowlTurnIsFinished();
            }
        }
    }
}
