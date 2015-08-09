using System;
using System.Reflection;
using System.Threading;
using ColossalFramework;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class Stubber
    {
        private static readonly object Lock = new object();
        private static string _nativeLevelName;
        private static string _additionalLevelName;
        private static RedirectCallsState _stateInitPrefabImpl;
        private static RedirectCallsState _stateLoadLevel;
        private static RedirectCallsState _stateBeginLoading;
        private static RedirectCallsState _stateEndLoading;

        public static void SetUp()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - SetUp");
            }

            _stateInitPrefabImpl = Util.RedirectInitPrefab();
            _stateLoadLevel = Util.RedirectLoadLevelAdditiveAsync();
            _stateBeginLoading = Util.RedirectBeginLoading();
            _stateEndLoading = Util.RedirectEndLoading();
            _nativeLevelName = null;
            _additionalLevelName = null;
        }

        public static void Reset()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - Reset");
            }
            Util.RevertLoadLevelAdditiveAsync(_stateLoadLevel);
            Util.RevertInitPrefab(_stateInitPrefabImpl);
            Util.RevertBeginLoading(_stateBeginLoading);
            Util.RevertEndLoading(_stateEndLoading);
            _nativeLevelName = null;
            _additionalLevelName = null;
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
                Debug.Log(String.Format("EuroBuildingsUnlocker - BeginLoading: level '{0}'", levelName));
            }
            try
            {
                var env = Util.GetEnv();
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log(String.Format("EuroBuildingsUnlocker - Environment is '{0}'", env));
                }
                if (env == null)
                {
                    return;
                }
                if (_nativeLevelName != null)
                {
                    return;
                }
                _nativeLevelName = env + "Prefabs";
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log(String.Format("EuroBuildingsUnlocker - It's time to load native level '{0}'",
                        _nativeLevelName));
                }
                Application.LoadLevelAdditive(_nativeLevelName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }


        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            if (field == null)
            {
                throw new Exception(String.Format("Type '{0}' doesn't have field '{1}", type, fieldName));
            }
            return field.GetValue(instance);
        }

        private static void InitializePrefabImpl(string collection, PrefabInfo prefab, string replace)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log(
                    String.Format(
                        "EuroBuildingsUnlocker - Initializing prefab: collection '{0}', prefab '{1}', replace '{2}'",
                        collection, prefab.name, replace));
            }
            try
            {
                if (!String.IsNullOrEmpty(replace))
                {
                    if ((_nativeLevelName != "EuropePrefabs" && (collection == "Europe Beautification")) ||
                        (_nativeLevelName == "EuropePrefabs" && (collection == "Tropical Beautification" || collection == "Sunny Beautification" || collection == "North Beautification")))
                    {
                        if (EuroBuildingsUnlocker.debug)
                        {
                            Debug.Log(String.Format(
                                "EuroBuildingsUnlocker - Prevented replacement of prefab '{0}' with prefab '{1}'",
                                replace, prefab.name));
                        }
                        replace = String.Empty;
                    }
                }

                var replaceNativeTrafficLights = EuroBuildingsUnlocker.Options.IsFlagSet(ModOptions.OverrideNativeTrafficLights);
                if (collection == "Europe Props")
                {
                    switch (prefab.name)
                    {
                        case "Traffic Light European 01":
                        case "Traffic Light European 01 Mirror":
                        case "Traffic Light European 02":
                        case "Traffic Light European 02 Mirror":
                        case "Traffic Light Pedestrian European":
                            {
                                if (_nativeLevelName == "EuropePrefabs" && replaceNativeTrafficLights)
                                {
                                    if (EuroBuildingsUnlocker.debug)
                                    {
                                        Debug.Log(String.Format(
                                            "EuroBuildingsUnlocker - Prevented european traffic light '{0}' from loading",
                                            prefab.name));
                                    }
                                    return;
                                }
                                else
                                {
                                    if (_nativeLevelName != "EuropePrefabs" && !replaceNativeTrafficLights)
                                    {
                                        if (EuroBuildingsUnlocker.debug)
                                        {
                                            Debug.Log(String.Format(
                                                "EuroBuildingsUnlocker - Prevented european traffic light '{0}' from overriding native one",
                                                prefab.name));
                                        }
                                        replace = "";
                                    }
                                }
                                break;

                            }
                    }
                }
                if (collection == "Road")
                {
                    switch (prefab.name)
                    {
                        case "Traffic Light 01":
                        case "Traffic Light 01 Mirror":
                        case "Traffic Light 02":
                        case "Traffic Light 02 Mirror":
                        case "Traffic Light Pedestrian":
                            {
                                if (_nativeLevelName != "EuropePrefabs" && replaceNativeTrafficLights)
                                {
                                    if (EuroBuildingsUnlocker.debug)
                                    {
                                        Debug.Log(String.Format(
                                            "EuroBuildingsUnlocker - Prevented non-european traffic light '{0}' from loading",
                                            prefab.name));
                                    }
                                    return;
                                }
                                break;
                            }
                    }
                }
                if (!EuroBuildingsUnlocker.Options.IsFlagSet(ModOptions.LoadNativeGrowables))
                {
                    if ((_nativeLevelName != "EuropePrefabs" && (collection == "Residential High" || collection == "Commercial High" ||
                        collection == "Residential Low" || collection == "Commercial Low" || collection == "Industrial" ||
                         collection == "Office")) ||
                         (_nativeLevelName == "EuropePrefabs" && (collection == "Europe Residential High" || collection == "Europe Commercial High" ||
                         collection == "Europe Residential Low" || collection == "Europe Commercial Low" || collection == "Europe Industrial" ||
                         collection == "Europe Office")))
                    {
                        if (EuroBuildingsUnlocker.debug)
                        {
                            Debug.Log(String.Format("EuroBuildingsUnlocker - Disabled native growable building '{0}",
                                prefab.name));
                        }
                        return;

                    }

                }
                if (!EuroBuildingsUnlocker.Options.IsFlagSet(ModOptions.LoadNonNativeGrowables))
                {
                    if ((_nativeLevelName != "EuropePrefabs" && (collection == "Europe Residential High" || collection == "Europe Commercial High" ||
                         collection == "Europe Residential Low" || collection == "Europe Commercial Low" || collection == "Europe Industrial" ||
                         collection == "Europe Office")) ||
                         (_nativeLevelName == "EuropePrefabs" && (collection == "Residential High" || collection == "Commercial High" ||
                        collection == "Residential Low" || collection == "Commercial Low" || collection == "Industrial" ||
                         collection == "Office")))
                    {
                        if (EuroBuildingsUnlocker.debug)
                        {
                            Debug.Log(String.Format(
                                "EuroBuildingsUnlocker - Disabled non-native growable building '{0}", prefab.name));
                        }
                        return;

                    }

                }
                if (!DuplicateExceptionPreventer.InitializePrefabImpl(collection, prefab, replace))
                {
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log("EuroBuildingsUnlocker - The prefab is a duplicate and will be ignored");
                    }
                    return;
                }
                Util.RevertInitPrefab(_stateInitPrefabImpl);
                var prefabType = prefab.GetType();
                var genericType = typeof(PrefabCollection<>).MakeGenericType(prefabType);
                var method = genericType
                    .GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic);
                method.Invoke(null, new object[] { collection, prefab, replace });

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                Util.RedirectInitPrefab();
            }
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
                if (levelName != _nativeLevelName)
                {
                    levelToLoad = levelName;
                }
                else
                {
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log(String.Format("EuroBuildingsUnlocker - Loading native level: '{0}'", levelName));
                    }
                    _additionalLevelName = Util.GetEnv() == "Europe" ? "TropicalPrefabs" : "EuropePrefabs";
                    levelToLoad = _additionalLevelName;
                    if (EuroBuildingsUnlocker.debug)
                    {
                        Debug.Log(String.Format("EuroBuildingsUnlocker - It's time to load additional level '{0}'",
                            levelToLoad));
                    }
                }
                Util.RevertLoadLevelAdditiveAsync(_stateLoadLevel);
                var result = Application.LoadLevelAdditiveAsync(levelToLoad);
                Util.RedirectLoadLevelAdditiveAsync();
                return result;
            }
            finally
            {
                Monitor.Exit(Lock);
            }
        }
    }
}