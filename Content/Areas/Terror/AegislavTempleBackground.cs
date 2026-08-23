using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.WallBackgroundSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror;

public partial class AegislavSurfaceBackground
{
    public class AegislavTempleBackground : MaskedWallBackground
    {
        private Asset<Texture2D> _moonspiralTowerMidTextureAsset;
        private Asset<Texture2D> _moonspiralTowerBackTextureAsset;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            _moonspiralTowerMidTextureAsset = ModContent.Request<Texture2D>(AssetRegistry.Textures.BackgroundPath2 + "BloodCathedral_Mid");
            _moonspiralTowerBackTextureAsset = ModContent.Request<Texture2D>(AssetRegistry.Textures.BackgroundPath2 + "BloodCathedral_Far");
        }

        public override void Unload()
        {
            base.Unload();
            _moonspiralTowerMidTextureAsset = null;
            _moonspiralTowerBackTextureAsset = null;
        }

        public override bool IsActive(Player player)
        {
            BiomePlayer biomePlayer = player.GetModPlayer<BiomePlayer>();
            return biomePlayer.ZoneAegislavSurface;
        }

        public override void SetupDrawLayers()
        {
            base.SetupDrawLayers();
            DrawScale = 1;

            DrawLayers[1].textureAsset = _moonspiralTowerMidTextureAsset;
            DrawLayers[1].parallax = new Vector2(0.135f);

            DrawLayers[0].textureAsset = _moonspiralTowerBackTextureAsset;
            DrawLayers[0].parallax = new Vector2(0.0075f);
        }
    }

}
