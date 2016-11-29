using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using EuroBuildingsUnlocker.Redirection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(SceneManager))]
    public class SceneManagerDetour
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
            _redirects = RedirectionUtil.RedirectType(typeof(SceneManagerDetour));
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
        public static AsyncOperation LoadSceneAsync(string levelName, LoadSceneMode mode)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                Debug.Log($"EuroBuildingsUnlocker - SceneManagerDetour - level '{levelName}' loading was demanded. Mode: {mode}" +
                          $"\nStack trace:\n{Environment.StackTrace}");
            }
            Monitor.Enter(Lock);
            try
            {
                RevertTemp();
                Levels.DetectNativeLevel();
                var isNativeLevel = false;
                if (levelName == Levels.GetNativeLevel() && AsyncOperationDetour.nativelevelOperation == null)
                {
                    levelName = Levels.GetFirstNonNativeLevel();
                    isNativeLevel = true;
                }
                if (EuroBuildingsUnlocker.debug)
                {
                    Debug.Log($"EuroBuildingsUnlocker - SceneManagerDetour - Loading level '{levelName}'");
                }
                var asyncOperation = SceneManager.LoadSceneAsync(levelName, mode);
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
                        AsyncOperationDetour.additionalLevels.Enqueue("PreorderPackPrefabs");
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
                AsyncOperationDetour.additionalLevels.Enqueue(Levels.GetNativeLevel());
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