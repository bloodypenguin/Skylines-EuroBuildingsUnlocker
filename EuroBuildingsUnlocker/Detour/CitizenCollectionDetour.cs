using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    //TODO(earalov): make sure no overlaps between biomes
    [TargetType(typeof(CitizenCollection))]
    public class CitizenCollectionDetour : CitizenCollection
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(CitizenCollectionDetour));
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

        private string GameObjectName => gameObject?.name;
        private string ParentName => gameObject?.transform?.parent?.gameObject?.name;

        [RedirectMethod]
        private void Awake()
        {
            if (ParentName == Constants.TropicalCollections || ParentName == Constants.SunnyCollections ||
                ParentName == Constants.NorthCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName == Constants.EuropeLevel)
                {
                    if (GameObjectName != Constants.TropicalBeautification || GameObjectName != Constants.SunnyBeautification|| GameObjectName != Constants.NorthBeautification)
                    {
                        Destroy(this);
                        return;
                    }
                }
            }
            else if (ParentName == Constants.EuropeCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName != Constants.EuropeLevel) {
                    if (GameObjectName != Constants.EuropeBeautification)
                    {
                        Destroy(this);
                        return;
                    }
                }
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, CitizenInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}