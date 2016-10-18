using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(PropCollection))]
    public class PropCollectionDetour : PropCollection
    {

        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored()
                && (!(!Levels.IsNativeLevelEuropean() && gameObject?.name == Constants.EuropeBeautification)) //to load tennis court
                && (!(!Levels.IsNativeLevelWinter() && gameObject?.name == Constants.WinterBeautification)) //to load curling
                )
            {
                Destroy(this);
                return;
            }
            if ((!Levels.IsNativeLevelWinter() && gameObject?.name == Constants.WinterPreorderPack)) //to prevent wrong preorder pack props versions from loading
            {
                Destroy(this);
                return;
            }
            if ((Levels.IsNativeLevelWinter() && gameObject?.name == Constants.PreorderPack))
            {
                m_prefabs = m_prefabs.Where(p => p.name == "Basketball Court Decal").ToArray();
                m_replacedNames = null;
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePrefabs(this.gameObject.name, this.m_prefabs, this.m_replacedNames));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePrefabs(string name, PropInfo[] prefabs, string[] replaces)
        {
            UnityEngine.Debug.Log($"{name}-{prefabs}-{(replaces == null ? "Null" : "Nonnull")}");
            return null;
        }
    }
}