using System;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace EuroBuildingsUnlocker
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
                Debug.Log("EuroBuildingsUnlocker - Loading level");
            }
            Monitor.Enter(Lock);
            try
            {
                string levelToLoad;
                if (levelName != EuroBuildingsUnlocker._nativeLevelName)
                {
                    levelToLoad = levelName;
                }
                else
                {
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log(String.Format("EuroBuildingsUnlocker - Loading native level: '{0}'", levelName));
                    }
                    EuroBuildingsUnlocker._additionalLevelName = Util.GetEnv() == "Europe" ? "TropicalPrefabs" : "EuropePrefabs";
                    levelToLoad = EuroBuildingsUnlocker._additionalLevelName;
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log(String.Format("EuroBuildingsUnlocker - It's time to load additional level '{0}'",
                            levelToLoad));
                    }
                }
                ApplicationDetour.Revert();
                var result = Application.LoadLevelAdditiveAsync(levelToLoad);
                ApplicationDetour.Deploy();
                return result;
            }
            finally
            {
                Monitor.Exit(Lock);
            }
        }
    }
}