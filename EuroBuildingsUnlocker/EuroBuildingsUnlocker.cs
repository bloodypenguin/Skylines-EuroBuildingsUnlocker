using ICities;

namespace EuroBuildingsUnlocker
{
    public class EuroBuildingsUnlocker : IUserMod
    {
#if DEBUG
        public static bool debug = true;
#else
        public static bool debug = false;
#endif
        public static string _nativeLevelName;
        public static BuildingCollection _extraBuildings;

        public string Name => "EuropeanBuildingsUnlocker";

        public string Description => "Unlocks European buildings (growables and ploppables) for all environments & vice versa";
    }
}
