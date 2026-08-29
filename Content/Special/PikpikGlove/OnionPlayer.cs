using Terraria.ModLoader;

namespace Stellamod.Content.Special.PikpikGlove
{
    public class OnionPlayer : ModPlayer
    {
        public int OnionDamage = 0;
        public bool Onion1 = false;
        public bool Onion2 = false;
        public bool Onion3 = false;
        public bool Onion4 = false;

        public override void ResetEffects()
        {
            Onion1 = false;
            Onion2 = false;
            Onion3 = false;
            Onion4 = false;
            OnionDamage = 0;
        }
    }
}