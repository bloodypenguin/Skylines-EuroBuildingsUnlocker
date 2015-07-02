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

    public class OptionsLoader
    {
        public  static void LoadOptions()
        {
            EuroBuildingsUnlocker.Options = OptionsManager.ModOptions.None;
            Options options = new Options();
            options.loadNativeGrowables = true;
            options.loadNonNativeGrowables = true;
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
                return;
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
                return;
            }
            if (!(options.loadNativeGrowables || options.loadNonNativeGrowables))
            {
                throw new Exception("European Buildings Unlocker  - at least one set of vanilla growables must be loaded!");
            }
            if (options.loadNativeGrowables)
                EuroBuildingsUnlocker.Options |= OptionsManager.ModOptions.LoadNativeGrowables;

            if (options.loadNonNativeGrowables)
                EuroBuildingsUnlocker.Options |= OptionsManager.ModOptions.LoadNonNativeGrowables;

            if (options.overrideNativeTraficLights)
                EuroBuildingsUnlocker.Options |= OptionsManager.ModOptions.OverrideNativeTrafficLights;

            if (options.addCustomAssetsGameObject)
                EuroBuildingsUnlocker.Options |= OptionsManager.ModOptions.AddCustomAssetsGameObject;
        }
    }

}