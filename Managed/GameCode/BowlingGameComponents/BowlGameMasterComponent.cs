using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using UnrealEngine.Slate;
using UnrealEngine;
using UnrealEngine.GameplayTasks;
using UnrealEngine.SlateCore;
using UnrealEngine.NavigationSystem;

namespace HelloUSharp
{
    [UClass, Blueprintable, BlueprintType]
    public class BowlGameMasterComponent : UActorComponent
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
        #endregion

        #region Fields
        protected static BowlGameMasterComponent ThisInstance = null;
        #endregion

        #region Getters
        public static BowlGameMasterComponent GetInstance(UObject worldContextObject)
        {
            if (ThisInstance == null)
            {
                ThisInstance = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<BowlGameMasterComponent>();
            }
            return ThisInstance;
        }
        #endregion

        #region Delegates
        public delegate void GeneralEventHandler();
        #endregion

        #region Overrides
        protected override void ReceiveBeginPlay_Implementation()
        {
            
        }

        protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        {
            //Set ThisInstance To Null, Otherwise Value Doesn't Get Destroyed and Will Crash Engine.
            ThisInstance = null;
        }
        #endregion
        
        #region Events
        public event GeneralEventHandler BowlTurnIsFinished;
        #endregion

        #region EventCalls
        public void CallBowlTurnIsFinished()
        {
            if (BowlTurnIsFinished != null) BowlTurnIsFinished();
        }
        #endregion

        #region UnusedCode
        //public class GeneralDelegateHandler : FMulticastDelegate<GeneralDelegateHandler.Signature>
        //{
        //    public delegate void Signature();
        //}

        //[UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        //public GeneralDelegateHandler BowlTurnIsFinished { get; set; }
        #endregion
    }
}
