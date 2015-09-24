using ICities;

namespace EuroBuildingsUnlocker
{
    public class LoadingExtension : ILoadingExtension
    {

        public void OnCreated(ILoading loading)
        {
            if (EuroBuildingsUnlocker.debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnCreated");
            }
            EuroBuildingsUnlocker.Bootstrap();

        }

        public void OnReleased()
        {
            if (EuroBuildingsUnlocker.debug)
            {
                UnityEngine.Debug.Log("EuroBuildingsUnlocker - OnReleased");
            }
            EuroBuildingsUnlocker.Revert();
        }

        public void OnLevelLoaded(LoadMode mode)
        {
        }

        public void OnLevelUnloading()
        {
        }
    }
}