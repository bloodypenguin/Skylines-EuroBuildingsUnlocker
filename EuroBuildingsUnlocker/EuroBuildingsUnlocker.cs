using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class EuroBuildingsUnlocker : IUserMod
    {
        private static bool _bootstrapped;
        public static ModOptions Options = ModOptions.None;
       // static GameObject sm_optionsManager;
        public static bool debug = false;

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
            NullifyEnvironmentVariable();
            DuplicateExceptionPreventer.Clear();
            Stubber.SetUp();
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
            NullifyEnvironmentVariable();
            Stubber.Reset();
            DuplicateExceptionPreventer.Clear();
            _bootstrapped = false;
        }

        public string Name
        {
            get
            {
                return "EuropeanBuildingsUnlocker";
            }
        }

        public string Description
        {
            get { return "Unlocks European buildings (growables and ploppables) for all environments & vice versa"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsLoader.LoadOptions();
            UIHelperBase group = helper.AddGroup("European Buildings Unlocker Options");
            _nativeCheckBox = (UICheckBox)group.AddCheckbox("Load native vanilla growables", (Options & ModOptions.LoadNativeGrowables)!=0,
                (b) =>
                {
                    if (!b && !_nonNativeCheckBox.isChecked)
                    {
                        _nativeCheckBox.isChecked = true;
                        return;
                    }
                    if (b)
                    {
                        Options |= ModOptions.LoadNativeGrowables;
                    }
                    else
                    {
                        Options &= ~ModOptions.LoadNativeGrowables;
                    }
                    OptionsLoader.SaveOptions();
                });
            _nonNativeCheckBox = (UICheckBox)group.AddCheckbox("Load non-native vanilla growables", (Options & ModOptions.LoadNonNativeGrowables) != 0,
                (b) =>
                {
                    if (!b && !_nativeCheckBox.isChecked)
                    {
                        _nonNativeCheckBox.isChecked = true;
                        return;
                    }
                    if (b)
                    {
                        Options |= ModOptions.LoadNonNativeGrowables;
                    }
                    else
                    {
                        Options &= ~ModOptions.LoadNonNativeGrowables;
                    }
                    OptionsLoader.SaveOptions();
                });
            group.AddCheckbox("Override native traffic lights", (Options & ModOptions.OverrideNativeTrafficLights) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.OverrideNativeTrafficLights;
                    }
                    else
                    {
                        Options &= ~ModOptions.OverrideNativeTrafficLights;
                    }
                    OptionsLoader.SaveOptions();
                });
            group.AddCheckbox("Add 'Custom Assets Collection' GameObject (may affect loading time)", (Options & ModOptions.AddCustomAssetsGameObject) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.AddCustomAssetsGameObject;
                    }
                    else
                    {
                        Options &= ~ModOptions.AddCustomAssetsGameObject;
                    }
                    OptionsLoader.SaveOptions();
                });
        }




        private static void NullifyEnvironmentVariable()
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            if (simulationManager != null)
            {
                var mMetaData = simulationManager.m_metaData;
                if (mMetaData != null)
                {
                    mMetaData.m_environment = null;
                }
            }
        }
    }

    public class ModLoad : ILoadingExtension
    {

        public void OnCreated(ILoading loading)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnCreated");
            }
            EuroBuildingsUnlocker.Bootstrap();

        }

        public void OnReleased()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnReleased");
            }
            EuroBuildingsUnlocker.Revert();
        }

        public void OnLevelLoaded(LoadMode mode)
        {
        }

        public void OnLevelUnloading()
        {

        }
    }

}
