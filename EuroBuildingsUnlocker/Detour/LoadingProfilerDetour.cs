using System;
using System.Reflection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    public class LoadingProfilerDetour
    {
        private static RedirectCallsState _stateBeginLoading;
        private static RedirectCallsState _stateEndLoading;
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                RedirectBeginLoading();
                RedirectEndLoading();
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
                RevertBeginLoading();
                RedirectEndLoading();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
            _deployed = false;
        }

        public static void RedirectEndLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectEndLoading");
            }
            _stateEndLoading =  RedirectionHelper.RedirectCalls
                (
                    typeof(LoadingProfiler).GetMethod("EndLoading", BindingFlags.Instance | BindingFlags.Public),
                    typeof(LoadingProfilerDetour).GetMethod("EndLoading", BindingFlags.Instance | BindingFlags.NonPublic)
                );
        }

        public static void RevertEndLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertEndLoading");
            }
            RedirectionHelper.RevertRedirect
                (
                    typeof(LoadingProfiler).GetMethod("EndLoading", BindingFlags.Instance | BindingFlags.Public),
                    _stateEndLoading
                );
        }
        public static void RedirectBeginLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectBeginLoading");
            }
            _stateBeginLoading = RedirectionHelper.RedirectCalls
                 (
                     typeof(LoadingProfiler).GetMethod("BeginLoading", BindingFlags.Instance | BindingFlags.Public),
                     typeof(LoadingProfilerDetour).GetMethod("BeginLoading", BindingFlags.Instance | BindingFlags.NonPublic)
                 );
        }

        public static void RevertBeginLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertBeginLoading");
            }
            RedirectionHelper.RevertRedirect
                (
                     typeof(LoadingProfiler).GetMethod("BeginLoading", BindingFlags.Instance | BindingFlags.Public),
                    _stateBeginLoading
                );
        }

        private void EndLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - EndLoading");
            }
        }

        private void BeginLoading(string levelName)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - BeginLoading: level '{levelName}'");
            }
            try
            {

                var env = Util.GetEnv();
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log($"EuroBuildingsUnlocker - Environment is '{env}'");
                }
                if (env == null)
                {
                    return;
                }
                if (EuroBuildingsUnlocker._nativeLevelName != null)
                {
                    return;
                }
                EuroBuildingsUnlocker._nativeLevelName = env + "Prefabs";
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }

    }
}