using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(BuildingCollection))]
    public class BuildingCollectionDetour : BuildingCollection
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(BuildingCollectionDetour));
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

        private bool IsInternationalPloppable => GameObjectName == Constants.FireDepartment || GameObjectName == Constants.PoliceDepartment || GameObjectName == Constants.Education || GameObjectName == Constants.HealthCare;
        private bool IsInternationalGrowable => GameObjectName == Constants.CommercialHigh || GameObjectName == Constants.ResidentialHigh || GameObjectName == Constants.Office || GameObjectName == Constants.ExtraBuildings;
        private bool IsEuropeanPloppable => GameObjectName == Constants.EuropeFireDepartment || GameObjectName == Constants.EuropePoliceDepartment || GameObjectName == Constants.EuropeEducation || GameObjectName == Constants.EuropeHealthCare || GameObjectName == Constants.EuropeBeautification || GameObjectName == Constants.EuropeMonument;
        private bool IsEuropeanGrowable => GameObjectName == Constants.EuropeCommercialHigh || GameObjectName == Constants.EuropeResidentialHigh || GameObjectName == Constants.EuropeIndustrial || GameObjectName == Constants.EuropeOffice;
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
                    if (!IsInternationalGrowable && !IsInternationalPloppable)
                    {
                        Destroy(this);
                        return;
                    }
                    if (IsInternationalGrowable && (OptionsHolder.Options & ModOption.LoadNonNativeGrowables) == 0)
                    {
                        Destroy(this);
                        return;
                    }
                    if (GameObjectName == Constants.Education || GameObjectName == Constants.HealthCare)
                    {
                        FixReplaces();
                    }
                }
                else
                {
                    if (IsInternationalGrowable && (OptionsHolder.Options & ModOption.LoadNativeGrowables) == 0)
                    {
                        Destroy(this);
                        return;
                    }
                }
            }
            else if (ParentName == Constants.EuropeCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName == Constants.EuropeLevel)
                {
                    if (IsEuropeanGrowable && (OptionsHolder.Options & ModOption.LoadNativeGrowables) == 0)
                    {
                        Destroy(this);
                        return;
                    }
                }
                else
                {
                    if (!IsEuropeanGrowable && !IsEuropeanPloppable)
                    {
                        Destroy(this);
                        return;
                    }
                    if (IsEuropeanGrowable && ((OptionsHolder.Options & ModOption.LoadNonNativeGrowables) == 0 || EuroBuildingsUnlocker._euroStyleEnabled))
                    {
                        Destroy(this);
                        return;
                    }
                    if (GameObjectName == Constants.EuropeEducation || GameObjectName == Constants.EuropeHealthCare)
                    {
                        FixReplaces();
                    }
                }
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
        }

        //TODO(earalov): provide extra international industrial and low density buildings for Euro biome
        //TODO(earalov): don't provide high density international buildings for European biome if non-native growables are disabled (or native in others)
        //TODO(earalov): filter out redundant European industries when loading non-euro biomes
        private void FixReplaces() //TODO(earalov): it's better to remove them from collections at all
        {
            if (m_prefabs == null)
            {
                return;
            }
            var newPrefabs= new List<BuildingInfo>();
            for (var i = 0; i < m_prefabs.Length; i++)
            {
                var prefabName = m_prefabs[i].name;
                if (prefabName != "Hadron Collider" && prefabName != "Medical Center" && prefabName != "Crematory" &&
                    prefabName != "Cemetery")
                {
                    newPrefabs.Add(m_prefabs[i]);
                }
            }
            m_prefabs = newPrefabs.ToArray();
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, BuildingInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}