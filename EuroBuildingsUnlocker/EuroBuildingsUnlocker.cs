﻿using ColossalFramework;
using ICities;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class EuroBuildingsUnlocker : IUserMod
    {
        private static bool _bootstrapped;
        public static OptionsManager.ModOptions Options = OptionsManager.ModOptions.None;
        static GameObject sm_optionsManager;

        public static void Bootstrap()
        {
            UnityEngine.Debug.Log("EuroBuildingsUnlocker - Bootstrap");
            if (_bootstrapped)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - Mod has been already bootstrapped");
                return;
            }
            NullifyEnvironmentVariable();
            DuplicateExceptionPreventer.Clear();
            Stubber.SetUp();
            _bootstrapped = true;
        }



        public static void Revert()
        {
            UnityEngine.Debug.Log("EuroBuildingsUnlocker - Revert");
            if (!_bootstrapped)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - Mod hasn't been bootstrapped");
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
                if (sm_optionsManager == null)
                {
                    sm_optionsManager = new GameObject("EuroBuildingsUnlockerOptionsManager");
                    sm_optionsManager.AddComponent<OptionsManager>();
                }
                return "EuropeanBuildingsUnlocker";
            }
        }

        public string Description
        {
            get { return "Unlocks European buildings (growables and ploppables) for all environments & vice versa"; }
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
            UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnCreated");
            EuroBuildingsUnlocker.Bootstrap();

        }

        public void OnReleased()
        {
            UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnReleased");
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
