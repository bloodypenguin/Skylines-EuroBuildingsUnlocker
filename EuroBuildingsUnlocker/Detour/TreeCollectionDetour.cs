using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    //TODO(earalov): merge tree collections
    [TargetType(typeof(TreeCollection))]
    public class TreeCollectionDetour : TreeCollection
    {

        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored())
            {
                Destroy(this);
                return;
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, TreeInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}