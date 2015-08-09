using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public struct Options
    {
        public bool loadNativeGrowables;
        public bool loadNonNativeGrowables;
        public bool overrideNativeTraficLights;
        public bool addCustomAssetsGameObject;
    }

    [Flags]
    public enum ModOptions : long
    {
        None = 0,
        LoadNativeGrowables = 1,
        LoadNonNativeGrowables = 2,
        OverrideNativeTrafficLights = 4,
        AddCustomAssetsGameObject = 8
    }

    public class OptionsLoader
    {
        public static void LoadOptions()
        {
            EuroBuildingsUnlocker.Options = ModOptions.None;
            Options options = new Options();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamReader streamReader = new StreamReader("CSL-EuroBuildingsUnlocker.xml"))
                {
                    options = (Options)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (FileNotFoundException)
            {
                // No options file yet
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNativeGrowables;
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNonNativeGrowables;
                SaveOptions();
                return;
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
                return;
            }
            if (options.loadNativeGrowables || options.loadNonNativeGrowables)
            {
                if (options.loadNativeGrowables)
                    EuroBuildingsUnlocker.Options |= ModOptions.LoadNativeGrowables;

                if (options.loadNonNativeGrowables)
                    EuroBuildingsUnlocker.Options |= ModOptions.LoadNonNativeGrowables;
            }
            else {
                Debug.LogWarning("European Buildings Unlocker  - at least one set of vanilla growables must be loaded. Resetting defaults.");
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNativeGrowables;
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNonNativeGrowables;
            }
            if (options.overrideNativeTraficLights)
                EuroBuildingsUnlocker.Options |= ModOptions.OverrideNativeTrafficLights;

            if (options.addCustomAssetsGameObject)
                EuroBuildingsUnlocker.Options |= ModOptions.AddCustomAssetsGameObject;
        }

        public static void SaveOptions()
        {
            if ((EuroBuildingsUnlocker.Options & ModOptions.LoadNativeGrowables) == 0 &&
                (EuroBuildingsUnlocker.Options & ModOptions.LoadNonNativeGrowables) == 0)
            {
                throw new Exception("European Buildings Unlocker  - at least one set of growables must be loaded!");
            }
            Options options = new Options();
            if ((EuroBuildingsUnlocker.Options & ModOptions.LoadNativeGrowables)!=0)
            {
                options.loadNativeGrowables = true;
            }
            if ((EuroBuildingsUnlocker.Options & ModOptions.LoadNonNativeGrowables) != 0)
            {
                options.loadNonNativeGrowables = true;
            }
            if ((EuroBuildingsUnlocker.Options & ModOptions.OverrideNativeTrafficLights) != 0)
            {
                options.overrideNativeTraficLights = true;
            }
            if ((EuroBuildingsUnlocker.Options & ModOptions.AddCustomAssetsGameObject) != 0)
            {
                options.addCustomAssetsGameObject = true;
            }

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamWriter streamWriter = new StreamWriter("CSL-EuroBuildingsUnlocker.xml"))
                {
                    xmlSerializer.Serialize(streamWriter, options);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " saving options: " + e.Message + "\n" + e.StackTrace);
            }
        }

    }

}