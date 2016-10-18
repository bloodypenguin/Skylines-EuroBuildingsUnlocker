using System.Collections.Generic;
using System.Reflection;
using EuroBuildingsUnlocker.Redirection;

namespace EuroBuildingsUnlocker.Detour
{
    [TargetType(typeof(EffectCollection))]
    public class EffectCollectionDetour : EffectCollection
    {
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