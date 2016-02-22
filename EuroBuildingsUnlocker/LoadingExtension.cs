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
            Levels.CheckIfWinterUnlockerEnabled();
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
            EuroBuildingsUnlocker._nativeLevelName = null;
            ApplicationDetour.Deploy();
            AsyncOperationDetour.Deploy();
            BuildingCollectionDetour.Deploy();
            PropCollectionDetour.Deploy();
            NetCollectionDetour.Deploy();
            CitizenCollectionDetour.Deploy();
            VehicleCollectionDetour.Deploy();
            EventCollectionDetour.Deploy();
            TransportCollectionDetour.Deploy();
            EffectCollectionDetour.Deploy();
            MilestoneCollectionDetour.Deploy();
            ItemClassCollectionDetour.Deploy();
            TreeCollectionDetour.Deploy();
            DistrictPolicyCollectionDetour.Deploy();
            BuildingCollectionDetour.Deploy();
            LoadingManagerDetour.Deploy();
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
            EuroBuildingsUnlocker._nativeLevelName = null;
            EuroBuildingsUnlocker._extraBuildings = null;
            ApplicationDetour.Revert();
            AsyncOperationDetour.Revert();
            BuildingCollectionDetour.Revert();
            PropCollectionDetour.Revert();
            NetCollectionDetour.Revert();
            CitizenCollectionDetour.Revert();
            VehicleCollectionDetour.Revert();
            EventCollectionDetour.Revert();
            TransportCollectionDetour.Revert();
            EffectCollectionDetour.Revert();
            MilestoneCollectionDetour.Revert();
            ItemClassCollectionDetour.Revert();
            TreeCollectionDetour.Revert();
            DistrictPolicyCollectionDetour.Revert();
            BuildingCollectionDetour.Revert();
            LoadingManagerDetour.Revert();
            _bootstrapped = false;
        }
    }
}