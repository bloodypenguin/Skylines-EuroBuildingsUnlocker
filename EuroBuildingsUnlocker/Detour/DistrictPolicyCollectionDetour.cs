using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(DistrictPolicyCollection))]
    public class DistrictPolicyCollectionDetour : DistrictPolicyCollection
    {

        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored())
            {
                return;  //don't destroy this, because manager will unload policies in such case!
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePolicies(this.gameObject.name, this));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePolicies(string name, DistrictPolicyCollection collection)
        {
            UnityEngine.Debug.Log($"{name}-{collection}");
            return null;
        }
    }
}