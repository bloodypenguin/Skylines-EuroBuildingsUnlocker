using System;
using System.Collections.Generic;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(LoadingProfiler))]
    public class LoadingProfilerDetour
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(LoadingProfilerDetour));
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
        private void EndLoading()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log("EuroBuildingsUnlocker - EndLoading");
            }
        }

        [RedirectMethod]
        private void BeginLoading(string levelName)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - BeginLoading: level '{levelName}'");
            }
            try
            {

                var env = Util.GetEnv();
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log($"EuroBuildingsUnlocker - Environment is '{env}'");
                }
                if (env == null)
                {
                    return;
                }
                if (EuroBuildingsUnlocker._nativeLevelName != null)
                {
                    return;
                }
                EuroBuildingsUnlocker._nativeLevelName = env + "Prefabs";
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}