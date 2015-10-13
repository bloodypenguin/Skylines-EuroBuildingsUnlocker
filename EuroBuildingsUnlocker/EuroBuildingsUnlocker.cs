using System;
using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class EuroBuildingsUnlocker : IUserMod
    {
        private static bool _bootstrapped;
        public static bool debug;
        public static string _nativeLevelName;
        public static string _additionalLevelName;
        public static bool _euroStyleEnabled;


        private static UICheckBox _nativeCheckBox;
        private static UICheckBox _nonNativeCheckBox;





        public static void Bootstrap()
        {
            if (debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - Bootstrap");
            }
            if (_bootstrapped)
            {
                if (debug)
                {
                    UnityEngine.Debug.Log("EuroBuildingsUnlocker - Mod has been already bootstrapped");
                }
                return;
            }
            Util.NullifyEnvironmentVariable();
            DuplicateExceptionPreventer.Clear();
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - SetUp");
            }
            ApplicationDetour.Deploy();
            PrefabCollectionDetour.Deploy();
            LoadingProfilerDetour.Deploy();
            _nativeLevelName = null;
            _additionalLevelName = null;
            try
            {
                var europeanStyles = PackageManager.FindAssetByName("System." + DistrictStyle.kEuropeanStyleName);
                _euroStyleEnabled = (europeanStyles != null && europeanStyles.isEnabled);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _bootstrapped = true;

        }



        public static void Revert()
        {
            if (debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - Revert");
            }
            if (!_bootstrapped)
            {
                if (debug)
                {
                    UnityEngine.Debug.Log("EuroBuildingsUnlocker - Mod hasn't been bootstrapped");
                }
                return;
            }
            Util.NullifyEnvironmentVariable();
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - Reset");
            }
            ApplicationDetour.Revert();
            PrefabCollectionDetour.Revert();
            LoadingProfilerDetour.Revert();
            _nativeLevelName = null;
            _additionalLevelName = null;
            DuplicateExceptionPreventer.Clear();
            _bootstrapped = false;
        }

        public string Name
        {
            get { return "EuropeanBuildingsUnlocker"; }
        }

        public string Description
        {
            get { return "Unlocks European buildings (growables and ploppables) for all environments & vice versa"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsLoader.LoadOptions();
            var group = helper.AddGroup("European Buildings Unlocker Options");
            AddCheckbox("Load native vanilla growables", ModOption.LoadNativeGrowables, group);
            AddCheckbox("Load non-native vanilla growables", ModOption.LoadNonNativeGrowables, group);
            AddCheckbox("Override native traffic lights", ModOption.OverrideNativeTrafficLights, group);
        }


        private static void AddCheckbox(string text, ModOption flag, UIHelperBase group)
        {
            group.AddCheckbox(text, OptionsHolder.Options.IsFlagSet(flag),
                b =>
                {
                    if (b)
                    {
                        OptionsHolder.Options |= flag;
                    }
                    else
                    {
                        OptionsHolder.Options &= ~flag;
                    }
                    OptionsLoader.SaveOptions();
                });
        }
    }
}
