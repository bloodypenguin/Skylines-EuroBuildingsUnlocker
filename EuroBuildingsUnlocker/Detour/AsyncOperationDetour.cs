using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(AsyncOperation))]
    public class AsyncOperationDetour : AsyncOperation
    {
        public static AsyncOperation nativelevelOperation = null;
        public static AsyncOperation additionalLevelOperation = null;
        private static readonly object Lock = new object();

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(AsyncOperationDetour));
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
        public bool get_isDone()
        {
            Monitor.Enter(Lock);
            try
            {
                bool result;
                //TODO(earalov): use low level redirection instead
                Revert();
                if (this != nativelevelOperation)
                {
                    result = isDone;
                }
                else
                {
                    if (nativelevelOperation.isDone)
                    {
                        if (additionalLevelOperation == null)
                        {
                            if (EuroBuildingsUnlocker.debug)
                            {
                                Debug.Log($"EuroBuildingsUnlocker - It's time to load native level '{EuroBuildingsUnlocker._nativeLevelName}'");
                            }
                            additionalLevelOperation = Application.LoadLevelAdditiveAsync(EuroBuildingsUnlocker._nativeLevelName);
                            result = false;
                        }
                        else
                        {
                            result = additionalLevelOperation.isDone;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                return result;
            }
            finally
            {
                //TODO(earalov): use low level redirection instead
                Deploy();
                Monitor.Exit(Lock);
            }
        }
    }
}