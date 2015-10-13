using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    //don't rename this class! it causes errors!
    public struct Options
    {
        public bool loadNativeGrowables;
        public bool loadNonNativeGrowables;
        public bool overrideNativeTraficLights;
        public bool addCustomAssetsGameObject; //deprecated
        public bool debugOutput;
    }

    [Flags]
    public enum ModOption : long
    {
        None = 0,
        LoadNativeGrowables = 1,
        LoadNonNativeGrowables = 2,
        OverrideNativeTrafficLights = 4
    }

    public static class OptionsHolder
    {
        public static ModOption Options = ModOption.None;
    }

    public class OptionsLoader
    {
        public static void LoadOptions()
        {
            OptionsHolder.Options = ModOption.None;
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
                OptionsHolder.Options |= ModOption.LoadNativeGrowables;
                OptionsHolder.Options |= ModOption.LoadNonNativeGrowables;
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
                    OptionsHolder.Options |= ModOption.LoadNativeGrowables;

                if (options.loadNonNativeGrowables)
                    OptionsHolder.Options |= ModOption.LoadNonNativeGrowables;
            }
            else {
                Debug.LogWarning("European Buildings Unlocker  - at least one set of vanilla growables must be loaded. Resetting defaults.");
                OptionsHolder.Options |= ModOption.LoadNativeGrowables;
                OptionsHolder.Options |= ModOption.LoadNonNativeGrowables;
            }
            if (options.overrideNativeTraficLights)
                OptionsHolder.Options |= ModOption.OverrideNativeTrafficLights;
            EuroBuildingsUnlocker.debug = options.debugOutput;
        }

        public static void SaveOptions()
        {
            if ((OptionsHolder.Options & ModOption.LoadNativeGrowables) == 0 &&
                (OptionsHolder.Options & ModOption.LoadNonNativeGrowables) == 0)
            {
                throw new Exception("European Buildings Unlocker  - at least one set of growables must be loaded!");
            }
            Options options = new Options();
            if ((OptionsHolder.Options & ModOption.LoadNativeGrowables)!=0)
            {
                options.loadNativeGrowables = true;
            }
            if ((OptionsHolder.Options & ModOption.LoadNonNativeGrowables) != 0)
            {
                options.loadNonNativeGrowables = true;
            }
            if ((OptionsHolder.Options & ModOption.OverrideNativeTrafficLights) != 0)
            {
                options.overrideNativeTraficLights = true;
            }
            options.debugOutput = EuroBuildingsUnlocker.debug;
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