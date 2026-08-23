using Stellamod.Content.Areas.RoyalCapital;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace
{
    public class WonderousPlayer : ModPlayer
    {
        public override void Load()
        {
            base.Load();
            On_Player.CanSeeShimmerEffects += RemoveShimmer;
        }
        public override void Unload()
        {
            base.Unload();
            On_Player.CanSeeShimmerEffects -= RemoveShimmer;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.LocalPlayer == null)
                return;
            if (!Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneWonder)
                return;
            if (Main.rand.NextBool(5))
            {
                float xRand = Main.rand.NextFloat(-1000, 1000);
                float yRand = Main.rand.NextFloat(-1000, 1000);
                LegacyParticle.NewParticle<StarParticle>(Main.LocalPlayer.Center + new Vector2(xRand, yRand), Vector2.Zero);
            }
        }
        private bool RemoveShimmer(On_Player.orig_CanSeeShimmerEffects orig, Player self)
        {
            if (Main.LocalPlayer == null)
                return false;
            if (Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneWonder)
                return false;
            return orig(self);
        }
    }
    public class WonderousDarkspcaeTileGlow : GlobalTile
    {
        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            base.ModifyLight(i, j, type, ref r, ref g, ref b);
            var biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
            if (!biomePlayer.ZoneWonder)
                return;
            Tile tile = Main.tile[i, j];
            if (WorldGen.TileIsExposedToAir(i, j) && tile.LiquidAmount > 0)
            {
                r = 0.25f;
                g = 0.71f;
                b = 0.8f;
            }
        }
    }
    public class WonderousDarkspaceBiome : BaseUrdveilBiome, IBackLightModifier
    {
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/AlcadziaBackgroundStyle");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/WondrousDarkspace");
        public override void SpecialVisuals(Player player, bool isActive)
        {
            string name = "LunarVeil:DarkspaceSky";
            if (!SkyManager.Instance[name].IsActive() && isActive)
                SkyManager.Instance.Activate(name, player.Center);
            if (SkyManager.Instance[name].IsActive() && !isActive)
                SkyManager.Instance.Deactivate(name);
        }
        public override bool IsBiomeActive(Player player)
        {
            StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
            return BiomeTileCounts.InDarkspace && !player.ZoneOverworldHeight && !player.ZoneSkyHeight && player.position.ToTileCoordinates().Y > stellaWorld.DarkspaceStart;
        }
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneWonder = true;
            if (Main.netMode == NetmodeID.Server)
                return;
            ModContent.GetInstance<LunarLightingRenderer>().AddBackLight(this);
        }
        public override void OnLeave(Player player)
        {
            player.GetModPlayer<BiomePlayer>().ZoneWonder = false;
            if (Main.netMode == NetmodeID.Server)
                return;
            ModContent.GetInstance<LunarLightingRenderer>().RemoveBackLight(this);
        }
        public void ModifyBackLight(ref Color backLightColor)
        {
            backLightColor = Color.Lerp(backLightColor, Color.White, 0.8f);
        }
    }
}