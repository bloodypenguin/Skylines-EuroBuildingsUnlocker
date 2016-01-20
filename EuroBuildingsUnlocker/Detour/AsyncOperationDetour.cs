using System;
using System.Threading;
using UnityEngine;

namespace EuroBuildingsUnlocker.Detour
{
    public class AsyncOperationDetour : AsyncOperation
    {
        private static RedirectCallsState _state;
        private static bool _deployed;
        public static AsyncOperation nativelevelOperation = null;
        public static AsyncOperation additionalLevelOperation = null;
        private static readonly object Lock = new object();
        
        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                _state = RedirectionHelper.RedirectCalls(typeof(AsyncOperation).GetMethod("get_isDone"),
                    typeof(AsyncOperationDetour).GetMethod("get_isDone"));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            _deployed = true;
        }


        public static void Revert()
        {
            if (!_deployed)
            {
                return;
            }
            try
            {
                RedirectionHelper.RevertRedirect(typeof(AsyncOperation).GetMethod("get_isDone"), _state);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
            _deployed = false;
        }


        public bool get_isDone()
        {
            Monitor.Enter(Lock);
            try
            {
                bool result;
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
                Deploy();
                Monitor.Exit(Lock);
            }
        }
    }
}