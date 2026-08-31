
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class ShimmerEdit : ModSystem
    {
        public override void Load()
        {
            base.Load();
            On_Item.CanShimmer += CanShimmer;
        }

        public override void Unload()
        {
            base.Unload();
            On_Item.CanShimmer -= CanShimmer;
        }

        private bool CanShimmer(On_Item.orig_CanShimmer orig, Item self)
        {
            //if (self.type == ItemID.RodofDiscord && !DownedBossTracker.IsDowned(DownedBossFlag.Ereshkigal))
            //    return false;
            return orig(self);
        }
    }
}
