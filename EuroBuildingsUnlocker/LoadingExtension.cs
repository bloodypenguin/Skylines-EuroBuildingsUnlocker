using System;
using ColossalFramework.Packaging;
using EuroBuildingsUnlocker.Detour;
using ICities;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static bool _bootstrapped;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (EuroBuildingsUnlocker.debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnCreated");
            }
            if (_bootstrapped)
            {
                if (EuroBuildingsUnlocker.debug)
                {
                    UnityEngine.Debug.Log("EuroBuildingsUnlocker - Mod has been already bootstrapped");
                }
                return;
            }
            Util.NullifyEnvironmentVariable();
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - SetUp");
            }
            ApplicationDetour.Deploy();
            AsyncOperationDetour.Deploy();
            EuroBuildingsUnlocker._nativeLevelName = null;
            EuroBuildingsUnlocker._additionalLevelName = null;
            BuildingCollectionDetour.Deploy();
            PropCollectionDetour.Deploy();
            NetCollectionDetour.Deploy();
            CitizenCollectionDetour.Deploy();
            VehicleCollectionDetour.Deploy();
            TransportCollectionDetour.Deploy();
            EffectCollectionDetour.Deploy();
            MilestoneCollectionDetour.Deploy();
            ItemClassCollectionDetour.Deploy();
            TreeCollectionDetour.Deploy();
            DistrictPolicyCollectionDetour.Deploy();
            BuildingCollectionDetour.Deploy();
            try
            {
                var europeanStyles = PackageManager.FindAssetByName("System." + DistrictStyle.kEuropeanStyleName);
                EuroBuildingsUnlocker._euroStyleEnabled = (europeanStyles != null && europeanStyles.isEnabled);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _bootstrapped = true;
        }
        
        public override void OnReleased()
        {
            base.OnReleased();
            if (EuroBuildingsUnlocker.debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnReleased");
            }
            if (!_bootstrapped)
            {
                if (EuroBuildingsUnlocker.debug)
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
            EuroBuildingsUnlocker._nativeLevelName = null;
            EuroBuildingsUnlocker._additionalLevelName = null;
            BuildingCollectionDetour.Revert();
            PropCollectionDetour.Revert();
            NetCollectionDetour.Revert();
            CitizenCollectionDetour.Revert();
            VehicleCollectionDetour.Revert();
            TransportCollectionDetour.Revert();
            EffectCollectionDetour.Revert();
            MilestoneCollectionDetour.Revert();
            ItemClassCollectionDetour.Revert();
            TreeCollectionDetour.Revert();
            DistrictPolicyCollectionDetour.Revert();
            BuildingCollectionDetour.Revert();
            _bootstrapped = false;
        }
    }
}