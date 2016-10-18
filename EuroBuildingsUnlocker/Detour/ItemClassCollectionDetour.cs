using System.Collections.Generic;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(ItemClassCollection))]
    public class ItemClassCollectionDetour : ItemClassCollection
    {
        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored())
            {
                Destroy(this);
                return;
            }
            ItemClassCollection.InitializeClasses(this.m_classes);
        }
    }
}