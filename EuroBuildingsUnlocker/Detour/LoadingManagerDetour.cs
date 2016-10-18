using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(LoadingManager))]
    public class LoadingManagerDetour : LoadingManager
    {

        [RedirectMethod]
        private void AddChildrenToBuiltinStyle(GameObject obj, DistrictStyle style, bool spawnNormally)
        {
            if ((UnityEngine.Object)obj == (UnityEngine.Object)null || style == null)
                return;
            //begin mod
            if (EuroBuildingsUnlocker._extraBuildings != null)
            {
                if (spawnNormally)
                {
                    foreach (var building in EuroBuildingsUnlocker._extraBuildings.m_prefabs
                        .Where(building => building.m_class.m_subService != ItemClass.SubService.ResidentialHigh &&
                        building.m_class.m_subService != ItemClass.SubService.CommercialHigh &&
                        building.m_class.m_service != ItemClass.Service.Office))
                    {
                        style.Add(building);
                        building.m_dontSpawnNormally = false;
                    }
                }
                else
                {
                    foreach (var building in EuroBuildingsUnlocker._extraBuildings.m_prefabs
                        .Where(building => building.m_class.m_subService == ItemClass.SubService.ResidentialHigh ||
                        building.m_class.m_subService == ItemClass.SubService.CommercialHigh ||
                        building.m_class.m_service == ItemClass.Service.Office))
                    {
                        style.Add(building);
                        building.m_dontSpawnNormally = true;
                    }
                }
            }
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