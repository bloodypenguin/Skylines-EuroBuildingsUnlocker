using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ColossalFramework;
using ColossalFramework.Packaging;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(BuildingCollection))]
    public class BuildingCollectionDetour : BuildingCollection
    {

        private bool IsIntUniquePloppable => GameObjectName == Constants.FireDepartment || GameObjectName == Constants.PoliceDepartment ||
                                                 GameObjectName == Constants.Education || GameObjectName == Constants.HealthCare;
        private bool IsEuroUniquePloppable => GameObjectName == Constants.EuropeFireDepartment || GameObjectName == Constants.EuropePoliceDepartment || GameObjectName == Constants.EuropeEducation || GameObjectName == Constants.EuropeHealthCare || GameObjectName == Constants.EuropeBeautification || GameObjectName == Constants.EuropeMonument;
        private string GameObjectName => gameObject?.name;

        [RedirectMethod]
        private void Awake()
        {
            var replacedNameOriginal = (string[])m_replacedNames.Clone();
            var prefabsOriginal = (BuildingInfo[])m_prefabs.Clone();

            if (this.IsCollectionInternational() && !this.IsCollectionWinter())
            {
                if (Levels.IsNativeLevelEuropean())
                {
                    if (!IsIntUniquePloppable && GameObjectName != Constants.ExtraBuildings)
                    {
                        Destroy(this);
                        return;
                    }
                    if (GameObjectName == Constants.Education || GameObjectName == Constants.HealthCare)
                    {
                        FixReplaces();
                    }
                    if (GameObjectName == Constants.ExtraBuildings)
                    {
                        ManageExtraBuildings();
                    }
                }
            }
            else if (this.IsCollectionWinter())
            {
                if (Levels.IsNativeLevelWinter())
                {
                    if (GameObjectName == Constants.WinterBeautification)
                    {
                        m_replacedNames = null;
                    }
                }
                else
                {
                    if (GameObjectName != Constants.WinterBeautification && GameObjectName != Constants.WinterMonument &&
                            GameObjectName != Constants.WinterIndustrialFarming && GameObjectName != Constants.WinterGarbage)
                    {
                        Destroy(this);
                        return;
                    }
                    if (GameObjectName == Constants.WinterIndustrialFarming)
                    {
                        m_prefabs = m_prefabs.Where(prefab => prefab.name.EndsWith("_Greenhouse")).ToArray();
                        m_replacedNames = null;
                    }
                    if (GameObjectName == Constants.WinterGarbage)
                    {
                        m_prefabs = m_prefabs.Where(prefab => prefab.name == "Snowdump").ToArray();
                        m_replacedNames = null;
                    }
                    if (GameObjectName == Constants.WinterBeautification)
                    {
                        m_replacedNames = null;
                    }
                }
            }
            else if (this.IsCollectionEuropean())
            {
                if (!Levels.IsNativeLevelEuropean())
                {
                    if (!IsEuroUniquePloppable)
                    {
                        Destroy(this);
                        return;
                    }
                    if (GameObjectName == Constants.EuropeEducation || GameObjectName == Constants.EuropeHealthCare)
                    {
                        FixReplaces();
                    }
                }
            }


            ResolveAfterDarkConflicts();
            ResolvePreorderPackConflicts();
            ResolveSignupPackConflicts();

            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
            m_replacedNames = replacedNameOriginal;
            m_prefabs = prefabsOriginal;

        }

        private void ManageExtraBuildings()
        {
            var vanillaStyle = PackageManager.FindAssetByName("System." + DistrictStyle.kEuropeanStyleName);
            if (vanillaStyle != null && vanillaStyle.isEnabled)
            {
                LoadingManagerDetour.addChildrenToBuiltinStyleHook = (go, style, spawnNormally) =>
                {
                    foreach (var building in this.m_prefabs)
                    {
                        var isHdGrowable = Util.IsHdGrowable(building);
                        if (spawnNormally || !isHdGrowable)
                        {
                            continue;
                        }
                        style.Add(building);
                        building.m_dontSpawnNormally = true;
                    }
                    LoadingManagerDetour.addChildrenToBuiltinStyleHook = null;
                };
            }
            else
            {
                LoadingManagerDetour.addChildrenToBuiltinStyleHook = null;
                m_prefabs = m_prefabs.Where(p => !Util.IsHdGrowable(p)).ToArray();
            }
        }


        private void ResolvePreorderPackConflicts()
        {
            if ((Levels.IsNativeLevelWinter() && gameObject?.name == Constants.PreorderPack))
            {
                m_prefabs = m_prefabs.Where(p => p.name == "Basketball Court" || p.name == "bouncer_castle").ToArray();
                m_replacedNames = null;
            }
        }

        private void ResolveAfterDarkConflicts()
        {
            if (this.IsCollectionSummerExpansion())
            {
                if (Levels.IsNativeLevelWinter())
                {
                    m_prefabs = m_prefabs.Where(prefab => prefab.name == "2x2_Jet_ski_rental"
                                                          || prefab.name == "2x8_FishingPier" ||
                                                          prefab.name == "Skatepark" ||
                                                          prefab.name == "Beachvolley Court" ||
                                                          prefab.name == "DrivingRange").ToArray();
                    m_replacedNames = null;
                }
                else
                {
                    for (int i = 0; i < m_replacedNames.Length; i++)
                    {
                        if (m_replacedNames[i] == "2x2_winter_fishing_pier")
                        {
                            m_replacedNames[i] = null;
                        }
                    }
                }
            }
            else if (this.IsCollectionWinterExpansion())
            {
                if (Levels.IsNativeLevelWinter())
                {
                    for (int i = 0; i < m_replacedNames.Length; i++)
                    {
                        if (m_replacedNames[i] == "2x2_Jet_ski_rental")
                        {
                            m_replacedNames[i] = null;
                        }
                    }
                }
                else
                {
                    m_prefabs = m_prefabs.Where(prefab => prefab.name == "2x2_winter_fishing_pier"
                                                          || prefab.name == "Snowmobile Track" ||
                                                          prefab.name == "Ice_Fishing_Pond" || prefab.name == "Ice Hockey Rink")
                        .ToArray();
                    m_replacedNames = null;
                }
            }
        }



        private void ResolveSignupPackConflicts()
        {
            if (Util.IsSnowfallInstalled() && Levels.IsWinterUnlockerEnabled)
            {
                if (GameObjectName == "Signup Pack" || GameObjectName == "Winter Signup Pack")
                {
                    m_replacedNames = null;
                }
            }
        }

        private void FixReplaces()
        {
            if (m_prefabs == null)
            {
                return;
            }
            var newPrefabs = new List<BuildingInfo>();
            for (var i = 0; i < m_prefabs.Length; i++)
            {
                var prefabName = m_prefabs[i].name;
                if (prefabName != "Hadron Collider" && prefabName != "Medical Center" && prefabName != "Crematory" &&
                    prefabName != "Cemetery")
                {
                    newPrefabs.Add(m_prefabs[i]);
                }
            }
            m_prefabs = newPrefabs.ToArray();
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, BuildingInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}