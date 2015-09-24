using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public struct OptionsDTO
    {
        public bool loadNativeGrowables;
        public bool loadNonNativeGrowables;
        public bool overrideNativeTraficLights;
        public bool addCustomAssetsGameObject;
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
            OptionsDTO options = new OptionsDTO();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OptionsDTO));
                using (StreamReader streamReader = new StreamReader("CSL-EuroBuildingsUnlocker.xml"))
                {
                    options = (OptionsDTO)xmlSerializer.Deserialize(streamReader);
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
        }

        public static void SaveOptions()
        {
            if ((OptionsHolder.Options & ModOption.LoadNativeGrowables) == 0 &&
                (OptionsHolder.Options & ModOption.LoadNonNativeGrowables) == 0)
            {
                throw new Exception("European Buildings Unlocker  - at least one set of growables must be loaded!");
            }
            OptionsDTO options = new OptionsDTO();
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
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OptionsDTO));
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