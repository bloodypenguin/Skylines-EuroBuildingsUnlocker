using ColossalFramework;

namespace EuroBuildingsUnlocker
{
    public static class Util
    {
        public static string GetEnv()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            if (simulationManager == null)
            {
                return null;
            }
            var mMetaData = simulationManager.m_metaData;
            return mMetaData == null ? null : mMetaData.m_environment;
        }

        public static void NullifyEnvironmentVariable()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            if (simulationManager != null)
            {
                var mMetaData = simulationManager.m_metaData;
                if (mMetaData != null)
                {
                    mMetaData.m_environment = null;
                }
            }
        }
    }
}