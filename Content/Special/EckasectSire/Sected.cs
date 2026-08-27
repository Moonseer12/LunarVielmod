using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.EckasectSire
{
    public class Sected : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}