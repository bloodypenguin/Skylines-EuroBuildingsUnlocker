using System;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    public class ApplicationDetour
    {
        private static readonly object Lock = new object();
        private static RedirectCallsState _stateLoadLevel;
        private static bool _deployed;


        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                RedirectLoadLevelAdditiveAsync();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            _deployed = true;
        }


        public static void Revert()
        {
            if (!_deployed)
            {
                return;
            }
            try
            {
                RevertLoadLevelAdditiveAsync();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
            _deployed = false;
        }

        public static void RedirectLoadLevelAdditiveAsync()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectLoadLevelAdditiveAsync");
            }
            _stateLoadLevel =  RedirectionHelper.RedirectCalls
               (
                   typeof(Application).GetMethod("LoadLevelAdditiveAsync", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null),
                   typeof(ApplicationDetour).GetMethod("LoadLevelAdditiveAsync", BindingFlags.Static | BindingFlags.Public)
               );
        }

        public static void RevertLoadLevelAdditiveAsync()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertLoadLevelAdditiveAsync");
            }
            RedirectionHelper.RevertRedirect
                (
                    typeof(Application).GetMethod("LoadLevelAdditiveAsync", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null),
                    _stateLoadLevel
                );
        }

        public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - Loading level {levelName}");
            }
            Monitor.Enter(Lock);
            try
            {
                bool isNativeLevel = false;
                string levelToLoad;
                if (levelName != EuroBuildingsUnlocker._nativeLevelName || AsyncOperationDetour.nativelevelOperation !=null)
                {
                    levelToLoad = levelName;
                }
                else
                {
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log($"EuroBuildingsUnlocker - Loading native level: '{levelName}'");
                    }
                    EuroBuildingsUnlocker._additionalLevelName = Util.GetEnv() == "Europe" ? "TropicalPrefabs" : "EuropePrefabs";
                    levelToLoad = EuroBuildingsUnlocker._additionalLevelName;
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log($"EuroBuildingsUnlocker - It's time to load additional level '{levelToLoad}'");
                    }
                    isNativeLevel = true;
                }
                Revert();
                var asyncOperation = Application.LoadLevelAdditiveAsync(levelToLoad);
                if (isNativeLevel)
                {
                    AsyncOperationDetour.nativelevelOperation = asyncOperation;
                }
                Deploy();
                return asyncOperation;
            }
            finally
            {
                Monitor.Exit(Lock);
            }
        }
    }
}