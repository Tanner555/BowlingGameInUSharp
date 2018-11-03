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
    [UClassIgnore]
    public class MyActorComponentSingleton<T> : UActorComponent where T : UActorComponent
    {
        #region IgnoreProperties
        //[UPropertyIngore]
        
        #endregion

        #region Fields
        protected static T ThisInstance = null;
        protected static bool bCanSetInstance;
        #endregion

        #region Overrides
        //[UFunctionIgnore]
        //public override void Initialize(FObjectInitializer initializer)
        //{
        //    //Set ThisInstance To Null, Otherwise Value Doesn't Get Destroyed and Will Crash Engine.
        //    ThisInstance = null;
        //    bCanSetInstance = true;
        //}

        //[UFunctionIgnore]
        //protected override void ReceiveEndPlay_Implementation(EEndPlayReason EndPlayReason)
        //{
        //    bCanSetInstance = false;
        //    ThisInstance = null;
        //}
        #endregion

        #region Getters
        [UFunctionIgnore]
        public static T GetInstance(UObject worldContextObject)
        {
            if (ThisInstance == null)
            {
                ThisInstance = UGameplayStatics.GetGameMode(worldContextObject).GetComponentByClass<T>();
            }
            return ThisInstance;
        }
        #endregion
    }

}
