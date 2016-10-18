using System.Collections.Generic;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(MilestoneCollection))]
    public class MilestoneCollectionDetour : MilestoneCollection
    {

        [RedirectMethod]
        private void Awake()
        {
            if (this.IsIgnored())
            {
                Destroy(this);
                return;
            }
            MilestoneCollection.InitializeMilestones(this.m_Milestones);
        }
    }
}