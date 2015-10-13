using System;
using System.Collections.Generic;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    class DuplicateExceptionPreventer
    {
        private static Dictionary<Type, Dictionary<string, PrefabData>> m_prefabDict = new Dictionary<Type, Dictionary<string, PrefabData>>();
        private struct PrefabData
        {
            public bool Replaced;
        }

        private static GameObject customAssetsGameObject;
        private static BuildingCollection buildingCollection;
        private static PropCollection propCollection;
        private static TreeCollection treeCollection;


        public static bool InitializePrefabImpl(string collection, PrefabInfo prefab, string replace)
        {
            var name = prefab.gameObject.name;
            PrefabData prefabData;
            var prefabType = prefab.GetType();
            if (!m_prefabDict.ContainsKey(prefabType))
            {
                m_prefabDict.Add(prefabType, new Dictionary<string, PrefabData>());
            }
            if (name != replace && m_prefabDict[prefabType].TryGetValue(name, out prefabData))
            {
                if (!prefabData.Replaced)
                {
                    return false;
                }
            }
            else
            {
                prefabData.Replaced = false;
                if (name != replace && !m_prefabDict[prefabType].ContainsKey(name))
                {
                    m_prefabDict[prefabType].Add(name, prefabData);
                }
                if (string.IsNullOrEmpty(replace))
                {
                    return true;
                }
                prefabData.Replaced = true;
                if (replace.IndexOf(',') != -1)
                {
                    var str = replace;
                    var chArray = new char[1];
                    const int index = 0;
                    const int num = 44;
                    chArray[index] = (char)num;
                    foreach (var key in str.Split(chArray))
                    {
                        m_prefabDict[prefabType][key] = prefabData;
                    }
                }
                else
                {
                    m_prefabDict[prefabType][replace] = prefabData;
                }
            }
            return true;
        }

        public static bool Clear()
        {
            if (customAssetsGameObject != null)
            {
                GameObject.DestroyObject(customAssetsGameObject);
                customAssetsGameObject = null;
                buildingCollection = null;
                propCollection = null;
                treeCollection = null;
            }
            m_prefabDict = new Dictionary<Type, Dictionary<string, PrefabData>>();
            return false;
        }
    }
}