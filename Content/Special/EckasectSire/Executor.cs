using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.EckasectSire
{
    public class Executor : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            EckasectPlayer EckasectPlayer = player.GetModPlayer<EckasectPlayer>();
            EckasectPlayer.Executor = true;


        }
    }
}