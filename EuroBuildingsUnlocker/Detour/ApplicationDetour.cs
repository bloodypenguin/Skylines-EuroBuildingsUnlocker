using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(Application))]
    public class ApplicationDetour
    {
        private static readonly object Lock = new object();
        private static RedirectCallsState _tempState;
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(ApplicationDetour));
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

        private static void RevertTemp()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                _tempState = RedirectionHelper.RevertJumpTo(redirect.Key.MethodHandle.GetFunctionPointer(), redirect.Value);
                break;
            }
        }

        private static void DeployBack()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertJumpTo(redirect.Key.MethodHandle.GetFunctionPointer(), _tempState);
                break;
            }
        }

        [RedirectMethod]
        public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
        {
            Monitor.Enter(Lock);
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - Loading level {levelName}");
            }
            try
            {
                RevertTemp();
                if (EuroBuildingsUnlocker._nativeLevelName == null)
                {
                    EuroBuildingsUnlocker._nativeLevelName = Levels.GetNativeLevel();
                }
                var isNativeLevel = false;
                if (levelName == EuroBuildingsUnlocker._nativeLevelName && AsyncOperationDetour.nativelevelOperation == null)
                {
                    levelName = Levels.GetFirstNonNativeLevel();
                    isNativeLevel = true;
                }
                var asyncOperation = Application.LoadLevelAdditiveAsync(levelName);
                if (!isNativeLevel)
                {
                    return asyncOperation;
                }
                AsyncOperationDetour.nativelevelOperation = asyncOperation;
                var secondNonNativeLevel = Levels.GetSecondNonNativeLevel();
                if (secondNonNativeLevel != null)
                {
                    AsyncOperationDetour.additionalLevels.Enqueue(secondNonNativeLevel);
                }
                if (Levels.IsNativeLevelWinter())
                {
                    if (Levels.IsWinterUnlockerEnabled)
                    {
                        if (Util.IsAfterDarkInstalled())
                        {
                            AsyncOperationDetour.additionalLevels.Enqueue("Expansion1Prefabs");
                        }
                        if (Util.IsPreorderPackInstalled())
                        {
                            AsyncOperationDetour.additionalLevels.Enqueue("PreorderPackPrefabs");
                        }
                        AsyncOperationDetour.additionalLevels.Enqueue("SignupPackPrefabs");
                    }
                }
                else {
                    if (Util.IsSnowfallInstalled() && Levels.IsWinterUnlockerEnabled)
                    {
                        if (Util.IsAfterDarkInstalled())
                        {
                            AsyncOperationDetour.additionalLevels.Enqueue("WinterExpansion1Prefabs");
                        }
                        AsyncOperationDetour.additionalLevels.Enqueue("WinterSignupPackPrefabs");
                    }
                }
                AsyncOperationDetour.additionalLevels.Enqueue(EuroBuildingsUnlocker._nativeLevelName);
                return asyncOperation;
            }
            finally
            {
                DeployBack();
                Monitor.Exit(Lock);
            }
        }
    }
}