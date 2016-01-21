using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(Application))]
    public class ApplicationDetour
    {
        private static readonly object Lock = new object();

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(ApplicationDetour));
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

        [RedirectMethod]
        public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
        {
            Monitor.Enter(Lock);
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - Loading level {levelName}");
            }
            try
            {
                if (EuroBuildingsUnlocker._nativeLevelName == null)
                {
                    EuroBuildingsUnlocker._nativeLevelName = GetNativeLevel();
                }
                var isNativeLevel = false;
                string levelToLoad;
                if (levelName != EuroBuildingsUnlocker._nativeLevelName || AsyncOperationDetour.nativelevelOperation != null)
                {
                    levelToLoad = levelName;
                }
                else
                {
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log($"EuroBuildingsUnlocker - Loading native level: '{levelName}'");
                    }
                    EuroBuildingsUnlocker._additionalLevelName = EuroBuildingsUnlocker._nativeLevelName == "EuropePrefabs" ? "TropicalPrefabs" : "EuropePrefabs";
                    levelToLoad = EuroBuildingsUnlocker._additionalLevelName;
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log($"EuroBuildingsUnlocker - It's time to load additional level '{levelToLoad}'");
                    }
                    isNativeLevel = true;
                }
                //TODO(earalov): use low level redirection instead
                Revert();
                var asyncOperation = Application.LoadLevelAdditiveAsync(levelToLoad);
                if (isNativeLevel)
                {
                    AsyncOperationDetour.nativelevelOperation = asyncOperation;
                }
                return asyncOperation;
            }
            finally
            {
                //TODO(earalov): use low level redirection instead
                Deploy();
                Monitor.Exit(Lock);
            }
        }

        private static string GetNativeLevel()
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
    }
}