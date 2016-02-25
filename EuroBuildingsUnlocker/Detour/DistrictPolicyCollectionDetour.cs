using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(DistrictPolicyCollection))]
    public class DistrictPolicyCollectionDetour : DistrictPolicyCollection
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(DistrictPolicyCollectionDetour));
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
                return;  //don't destroy this, because manager will unload policies in such case!
            }
            Singleton<LoadingManager>.instance.QueueLoadingAction(InitializePolicies(this.gameObject.name, this));
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerator InitializePolicies(string name, DistrictPolicyCollection collection)
        {
            UnityEngine.Debug.Log($"{name}-{collection}");
            return null;
        }
    }
}