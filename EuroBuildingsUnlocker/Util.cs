using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public sealed class Util
    {
        public static RedirectCallsState RedirectLoadLevelAdditiveAsync()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectLoadLevelAdditiveAsync");
            }
            return RedirectionHelper.RedirectCalls
               (
                   typeof(Application).GetMethod("LoadLevelAdditiveAsync", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null),
                   typeof(Stubber).GetMethod("LoadLevelAdditiveAsync", BindingFlags.Static | BindingFlags.Public)
               );
        }

        public static void RevertLoadLevelAdditiveAsync(RedirectCallsState stateLoadLevel)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertLoadLevelAdditiveAsync");
            }
            RedirectionHelper.RevertRedirect
                (
                    typeof(Application).GetMethod("LoadLevelAdditiveAsync", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null),
                    stateLoadLevel
                );
        }


        public static RedirectCallsState RedirectInitPrefab()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectInitPrefab");
            }
            return RedirectionHelper.RedirectCalls
               (
                   typeof(PrefabCollection<>).MakeGenericType(typeof(BuildingInfo))
                       .GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic),
                   typeof(Stubber).GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic)
               );
        }

        public static void RevertInitPrefab(RedirectCallsState stateInitPrefabImpl)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertInitPrefab");
            }
            RedirectionHelper.RevertRedirect
                (
                    typeof(PrefabCollection<>).MakeGenericType(typeof(BuildingInfo))
                        .GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic),
                    stateInitPrefabImpl
                );
        }

        public static RedirectCallsState RedirectEndLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectEndLoading");
            }
            return RedirectionHelper.RedirectCalls
                (
                    typeof(LoadingProfiler).GetMethod("EndLoading", BindingFlags.Instance | BindingFlags.Public),
                    typeof(Stubber).GetMethod("EndLoading", BindingFlags.Instance | BindingFlags.NonPublic)
                );
        }

        public static void RevertEndLoading(RedirectCallsState stateEndLoading)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertEndLoading");
            }
            RedirectionHelper.RevertRedirect
                (
                    typeof(LoadingProfiler).GetMethod("EndLoading", BindingFlags.Instance | BindingFlags.Public),
                    stateEndLoading
                );
        }
        public static RedirectCallsState RedirectBeginLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RedirectBeginLoading");
            }
            return RedirectionHelper.RedirectCalls
                 (
                     typeof(LoadingProfiler).GetMethod("BeginLoading", BindingFlags.Instance | BindingFlags.Public),
                     typeof(Stubber).GetMethod("BeginLoading", BindingFlags.Instance | BindingFlags.NonPublic)
                 );
        }

        public static void RevertBeginLoading(RedirectCallsState stateBeginLoading)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - RevertBeginLoading");
            }
            RedirectionHelper.RevertRedirect
                (
                     typeof(LoadingProfiler).GetMethod("BeginLoading", BindingFlags.Instance | BindingFlags.Public),
                    stateBeginLoading
                );
        }

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
    }
}