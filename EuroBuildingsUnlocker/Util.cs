using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.Steamworks;
using ICities;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public static class Util
    {
        public static void NullifyEnvironmentVariable()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var mMetaData = simulationManager?.m_metaData;
            if (mMetaData != null)
            {
                mMetaData.m_environment = null;
            }
        }

        public static bool IsCollectionEuropean(this Component component)
        {
            return ParentName(component) == Constants.EuropeCollections;
        }

        public static bool IsCollectionInternational(this Component component)
        {
            return ParentName(component) == Constants.TropicalCollections ||
                   ParentName(component) == Constants.SunnyCollections ||
                   ParentName(component) == Constants.NorthCollections ||
                   IsCollectionWinter(component);
        }

        public static bool IsCollectionWinter(this Component component)
        {
            return ParentName(component) == Constants.WinterCollections;
        }

        public static bool IsCollectionWinterExpansion(this Component component)
        {
            return component?.gameObject?.name == "Winter Expansion 1";
        }

        public static bool IsCollectionSummerExpansion(this Component component)
        {
            return component?.gameObject?.name == "Expansion 1";
        }

        public static bool IsPreorderPackInstalled()
        {
            return Steam.IsDlcInstalled(340160U);
        }

        public static bool IsSnowfallInstalled()
        {
            return Steam.IsDlcInstalled(420610U);
        }

        public static bool IsAfterDarkInstalled()
        {
            return Steam.IsDlcInstalled(369150U);
        }

        private static string ParentName(Component component)
        {
           return component?.gameObject?.transform?.parent?.gameObject?.name;
        }

        public static bool IsModActive(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>() into instances
                    where instances.Any()
                    select instances[0].Name into name
                    where name == modName
                    select name).Any();
        }
    }
}