using System;
using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using EuroBuildingsUnlocker.Detour;
using ICities;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class EuroBuildingsUnlocker : IUserMod
    {
#if DEBUG
        public static bool debug = true;
#else
        public static bool debug = false;
#endif
        public static string _nativeLevelName;
        public static string _additionalLevelName;
        public static bool _euroStyleEnabled;

        private static UICheckBox _nativeCheckBox;
        private static UICheckBox _nonNativeCheckBox;

        public string Name => "EuropeanBuildingsUnlocker";

        public string Description => "Unlocks European buildings (growables and ploppables) for all environments & vice versa";

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsLoader.LoadOptions();
            var group = helper.AddGroup("European Buildings Unlocker Options");
            _nativeCheckBox = AddCheckbox("Load native vanilla growables", ModOption.LoadNativeGrowables, group, (b) =>
            {
                if (b || _nonNativeCheckBox.isChecked)
                {
                    return true;
                }
                _nativeCheckBox.isChecked = true;
                return false;
            });
            _nonNativeCheckBox = AddCheckbox("Load non-native vanilla growables", ModOption.LoadNonNativeGrowables, group, (b) =>
            {
                if (b || _nativeCheckBox.isChecked)
                {
                    return true;
                }
                _nonNativeCheckBox.isChecked = true;
                return false;
            });
            AddCheckbox("Override native traffic lights", ModOption.OverrideNativeTrafficLights, group);
        }


        private static UICheckBox AddCheckbox(string text, ModOption flag, UIHelperBase group, Func<bool, bool> additonalCondition = null)
        {
            return (UICheckBox)group.AddCheckbox(text, OptionsHolder.Options.IsFlagSet(flag),
                b =>
                {
                    if (additonalCondition != null && !additonalCondition.Invoke(b))
                    {
                        return;
                    }
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
