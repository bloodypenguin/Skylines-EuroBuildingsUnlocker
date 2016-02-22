using ColossalFramework;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public static class Levels
    {
        public static bool IsWinterUnlockerEnabled { get; private set; }

        public static void CheckIfWinterUnlockerEnabled()
        {
            IsWinterUnlockerEnabled = Util.IsModActive("Winter Buildings Unlocker");
        }

        public static string GetFirstNonNativeLevel()
        {
            if (Util.IsSnowfallInstalled() && IsWinterUnlockerEnabled)
            {
                return IsNativeLevelWinter() ? Constants.EuropePrefabs : Constants.WinterPrefabs;
            }
            return IsNativeLevelEuropean() ? Constants.TropicalPrefabs : Constants.EuropePrefabs;
        }

        public static string GetSecondNonNativeLevel()
        {
            if (IsNativeLevelWinter())
            {
                return null;
            }
            if (Util.IsSnowfallInstalled() && IsWinterUnlockerEnabled)
            {
                return IsNativeLevelEuropean() ? Constants.TropicalPrefabs : Constants.EuropePrefabs;
            }
            return null;
        }

        public static string GetNativeLevel()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var mMetaData = simulationManager?.m_metaData;
            var env = mMetaData?.m_environment;
            if (env == null)
            {
                return null;
            }
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - Environment is '{env}'");
            }
            return $"{env}Prefabs";
        }

        public static bool IsIgnored(this Component component)
        {
            if (component.IsCollectionInternational() && !component.IsCollectionWinter())
            {
                if (IsNativeLevelEuropean() || IsNativeLevelWinter())
                {
                    return true;
                }
            }
            else if (component.IsCollectionEuropean())
            {
                if (!IsNativeLevelEuropean())
                {
                    return true;
                }
            } else if (component.IsCollectionWinter())
            {
                if (!IsNativeLevelWinter())
                {
                    return true;
                }
            } else if (component.IsCollectionWinterExpansion())
            {
                if (!IsNativeLevelWinter())
                {
                    return true;
                }
            }
            else if (component.IsCollectionSummerExpansion())
            {
                if (IsNativeLevelWinter())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNativeLevelWinter()
        {
            return EuroBuildingsUnlocker._nativeLevelName == Constants.WinterPrefabs;
        }

        public static bool IsNativeLevelEuropean()
        {
            return EuroBuildingsUnlocker._nativeLevelName == Constants.EuropePrefabs;
        }
    }
}