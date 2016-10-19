using System;
using System.Linq;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(LoadingManager))]
    public class LoadingManagerDetour : LoadingManager
    {
        public static Action<GameObject, DistrictStyle, bool> addChildrenToBuiltinStyleHook = null;


        [RedirectMethod]
        private void AddChildrenToBuiltinStyle(GameObject obj, DistrictStyle style, bool spawnNormally)
        {
            if ((UnityEngine.Object)obj == (UnityEngine.Object)null || style == null)
                return;
            //begin mod
            addChildrenToBuiltinStyleHook?.Invoke(obj, style, spawnNormally);
            //end mod
            foreach (BuildingCollection buildingCollection in obj.GetComponentsInChildren<BuildingCollection>(true))
            {
                BuildingInfo[] buildingInfoArray = buildingCollection.m_prefabs;
                for (int index = 0; index < buildingInfoArray.Length; ++index)
                {
                    style.Add(buildingInfoArray[index]);
                    buildingInfoArray[index].m_dontSpawnNormally = !spawnNormally;
                }
            }
        }
    }
}