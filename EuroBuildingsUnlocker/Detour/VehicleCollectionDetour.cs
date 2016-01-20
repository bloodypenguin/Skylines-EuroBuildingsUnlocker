using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(VehicleCollection))]
    public class VehicleCollectionDetour : VehicleCollection
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(VehicleCollectionDetour));
        }
        public static void Revert()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertRedirect(redirect.Key, redirect.Value);
            }
            _redirects = null;
        }

        private string ParentName => gameObject?.transform?.parent?.gameObject?.name;

        [RedirectMethod]
        private void Awake()
        {
            if (ParentName == Constants.TropicalCollections || ParentName == Constants.SunnyCollections ||
                ParentName == Constants.NorthCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName == Constants.EuropeLevel)
                {
                    Destroy(this);
                    return;
                }
            }
            else if (ParentName == Constants.EuropeCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName != Constants.EuropeLevel)
                {
                    Destroy(this);
                    return;
                }
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, VehicleInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}