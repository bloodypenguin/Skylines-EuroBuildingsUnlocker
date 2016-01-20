using System.Collections.Generic;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(ItemClassCollection))]
    public class ItemClassCollectionDetour : ItemClassCollection
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(ItemClassCollectionDetour));
        }
        public static void Revert()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertRedirect(redirect.Key, redirect.Value);
            }
            _redirects = null;
        }

        private string ParentName => gameObject?.transform?.parent?.gameObject?.name;

        [RedirectMethod]
        private void Awake()
        {
            if (ParentName == Constants.TropicalCollections || ParentName == Constants.SunnyCollections ||
                ParentName == Constants.NorthCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName == Constants.EuropeLevel)
                {
                    Destroy(this);
                    return;
                }
            }
            else if (ParentName == Constants.EuropeCollections)
            {
                if (EuroBuildingsUnlocker._nativeLevelName != Constants.EuropeLevel)
                {
                    Destroy(this);
                    return;
                }
            }
            ItemClassCollection.InitializeClasses(this.m_classes);
        }
    }
}