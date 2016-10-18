using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(BuildingCommonCollection))]
    public class BuildingCommonCollectionDetour : BuildingCommonCollection
    {

        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored())
            {
                return; //don't destroy this, because common will become null in Building manager
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, BuildingCommonCollection collection)
        {
            UnityEngine.Debug.Log($"{name}-{collection}");
            return null;
        }
    }
}