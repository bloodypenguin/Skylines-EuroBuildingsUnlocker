using System.Collections.Generic;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(EffectCollection))]
    public class EffectCollectionDetour : EffectCollection
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(EffectCollectionDetour));
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

        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored())
            {
                Destroy(this);
                return;
            }
            EffectCollection.InitializeEffects(this.m_effects);
        }
    }
}