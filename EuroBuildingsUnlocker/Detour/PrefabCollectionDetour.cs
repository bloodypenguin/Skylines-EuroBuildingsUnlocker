using System;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class PrefabCollectionDetour
    {
        private static bool _deployed;
        private static RedirectCallsState _stateInitPrefabImpl;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log("EuroBuildingsUnlocker - RedirectInitPrefab");
                }
                _stateInitPrefabImpl = RedirectionHelper.RedirectCalls
                   (
                       typeof(PrefabCollection<>).MakeGenericType(typeof(BuildingInfo))
                           .GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic),
                       typeof(PrefabCollectionDetour).GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic)
                   );
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
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log("EuroBuildingsUnlocker - RevertInitPrefab");
                }
                RedirectionHelper.RevertRedirect
                    (
                        typeof(PrefabCollection<>).MakeGenericType(typeof(BuildingInfo))
                            .GetMethod("InitializePrefabImpl", BindingFlags.Static | BindingFlags.NonPublic),
                        _stateInitPrefabImpl
                    );
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
            _deployed = false;
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
                    if ((EuroBuildingsUnlocker._nativeLevelName != "EuropePrefabs" && (collection == "Europe Beautification")) ||
                        (EuroBuildingsUnlocker._nativeLevelName == "EuropePrefabs" && (collection == "Tropical Beautification" || collection == "Sunny Beautification" || collection == "North Beautification")))
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

                var replaceNativeTrafficLights = OptionsHolder.Options.IsFlagSet(ModOption.OverrideNativeTrafficLights);
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
                                if (EuroBuildingsUnlocker._nativeLevelName == "EuropePrefabs" && replaceNativeTrafficLights)
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
                                    if (EuroBuildingsUnlocker._nativeLevelName != "EuropePrefabs" && !replaceNativeTrafficLights)
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
                                if (EuroBuildingsUnlocker._nativeLevelName != "EuropePrefabs" && replaceNativeTrafficLights)
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
                if (!OptionsHolder.Options.IsFlagSet(ModOption.LoadNativeGrowables))
                {
                    if ((EuroBuildingsUnlocker._nativeLevelName != "EuropePrefabs" && Util.IsPrefabInternational(collection)) ||
                         (EuroBuildingsUnlocker._nativeLevelName == "EuropePrefabs" && Util.IsPrefabEuropean(collection)))
                    {
                        if (EuroBuildingsUnlocker.debug)
                        {
                            Debug.Log(String.Format("EuroBuildingsUnlocker - Disabled native growable building '{0}",
                                prefab.name));
                        }
                        return;

                    }

                }
                if (!OptionsHolder.Options.IsFlagSet(ModOption.LoadNonNativeGrowables))
                {
                    if ((EuroBuildingsUnlocker._nativeLevelName != "EuropePrefabs" &&
                         ((!EuroBuildingsUnlocker._euroStyleEnabled && 
                         (collection == "Europe Residential High" || collection == "Europe Commercial High" ||
                          collection == "Europe Office")) ||
                          collection == "Europe Residential Low" || collection == "Europe Commercial Low" ||
                          collection == "Europe Industrial")) ||
                        (EuroBuildingsUnlocker._nativeLevelName == "EuropePrefabs" && Util.IsPrefabInternational(collection)
                         ))
                    {
                        if (EuroBuildingsUnlocker.debug)
                        {
                            Debug.Log(String.Format(
                                "EuroBuildingsUnlocker - Disabled non-native growable building '{0}", prefab.name));
                        }
                        return;

                    }
                }
                else
                {
                    if (EuroBuildingsUnlocker._euroStyleEnabled && (EuroBuildingsUnlocker._nativeLevelName != "EuropePrefabs" &&
                     (collection == "Europe Residential Low" || collection == "Europe Commercial Low" ||
                      collection == "Europe Industrial")))
                    {
                        if (EuroBuildingsUnlocker.debug)
                        {
                            Debug.Log(String.Format(
                                "EuroBuildingsUnlocker - Disabled Euro growable building '{0}' because European Style was detected", prefab.name));
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
                PrefabCollectionDetour.Revert();
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
                PrefabCollectionDetour.Deploy();
            }
        }
    }
}