using ColossalFramework;

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

        public static bool IsPrefabEuropean(string collection)
        {
            return (collection == "Europe Residential High" || collection == "Europe Commercial High" ||
                         collection == "Europe Residential Low" || collection == "Europe Commercial Low" || collection == "Europe Industrial" ||
                         collection == "Europe Office");
        }


        public static bool IsPrefabInternational(string collection)
        {
            return (collection == "Residential High" || collection == "Commercial High" ||
                    collection == "Residential Low" || collection == "Commercial Low" ||
                    collection == "Industrial" ||
                    collection == "Office" || collection == "Extra Buildings");
        }
    }
}