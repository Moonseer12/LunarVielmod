using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class ShakePlayer : ModPlayer
    {
        private float shakeDrama;

        public void ShakeAtPosition(Vector2 position, float distance, float strength)
        {
            LunarVeilClientConfig config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.ShakeToggle)
                return;
            shakeDrama = strength * (1f - Player.Center.Distance(position) / distance) * 0.5f;
        }

        public override void ModifyScreenPosition()
        {
            if (shakeDrama > 0.5f)
            {
                shakeDrama *= 0.92f;
                Vector2 shake = new(Main.rand.NextFloat(shakeDrama), Main.rand.NextFloat(shakeDrama));
                Main.screenPosition += shake;
            }
        }
    }
}