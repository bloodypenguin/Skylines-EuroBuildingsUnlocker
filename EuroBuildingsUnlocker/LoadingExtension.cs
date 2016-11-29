using System;
using ColossalFramework.Packaging;
using EuroBuildingsUnlocker.Detour;
using EuroBuildingsUnlocker.Redirection;
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
            Levels.ResetNativeLevel();
            SceneManagerDetour.Deploy();
            AsyncOperationDetour.Deploy();
            Redirector<BuildingCollectionDetour>.Deploy();
            Redirector<PropCollectionDetour>.Deploy();
            Redirector<NetCollectionDetour>.Deploy();
            Redirector<CitizenCollectionDetour>.Deploy();
            Redirector<VehicleCollectionDetour>.Deploy();
            Redirector<EventCollectionDetour>.Deploy();
            Redirector<TransportCollectionDetour>.Deploy();
            Redirector<EffectCollectionDetour>.Deploy();
            Redirector<MilestoneCollectionDetour>.Deploy();
            Redirector<ItemClassCollectionDetour>.Deploy();
            Redirector<TreeCollectionDetour>.Deploy();
            Redirector<DistrictPolicyCollectionDetour>.Deploy();
            Redirector<BuildingCommonCollectionDetour>.Deploy();
            Redirector<LoadingManagerDetour>.Deploy();
            LoadingManagerDetour.addChildrenToBuiltinStyleHook = null;
            _bootstrapped = true;
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Levels.ResetNativeLevel();
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
            Levels.ResetNativeLevel();
            SceneManagerDetour.Revert();
            AsyncOperationDetour.Revert();
            Redirector<BuildingCollectionDetour>.Revert();
            Redirector<PropCollectionDetour>.Revert();
            Redirector<NetCollectionDetour>.Revert();
            Redirector<CitizenCollectionDetour>.Revert();
            Redirector<VehicleCollectionDetour>.Revert();
            Redirector<EventCollectionDetour>.Revert();
            Redirector<TransportCollectionDetour>.Revert();
            Redirector<EffectCollectionDetour>.Revert();
            Redirector<MilestoneCollectionDetour>.Revert();
            Redirector<ItemClassCollectionDetour>.Revert();
            Redirector<TreeCollectionDetour>.Revert();
            Redirector<DistrictPolicyCollectionDetour>.Revert();
            Redirector<BuildingCommonCollectionDetour>.Revert();
            Redirector<LoadingManagerDetour>.Revert();
            LoadingManagerDetour.addChildrenToBuiltinStyleHook = null;
            _bootstrapped = false;
        }
    }
}