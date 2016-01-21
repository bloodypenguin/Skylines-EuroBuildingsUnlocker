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
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(BuildingCommonCollectionDetour));
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
                    return; //don't destroy this, because common will become null in Building manager
                }
            }
            else if (ParentName == Constants.EuropeCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName != Constants.EuropeLevel)
                {
                    return; //don't destroy this, because common will become null in Building manager
                }
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePolicies(this.gameObject.name, this));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePolicies(string name, BuildingCommonCollection collection)
        {
            UnityEngine.Debug.Log($"{name}-{collection}");
            return null;
        }
    }
}