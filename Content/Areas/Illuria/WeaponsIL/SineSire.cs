using Stellamod.Common;
using Stellamod.Common.OrbSystem;
using Stellamod.Dusts;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class SineSireDebuff : ModBuff
    {
        public static readonly int TagDamage = 12;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class SineSireDebuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool markedByWhip;

        public override void ResetEffects(NPC npc)
        {
            markedByWhip = false;
        }

        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (markedByWhip && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                projectile.damage += SineSireDebuff.TagDamage;
            }
        }
    }

    public class SineSire : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SineSireDebuff.TagDamage);
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd", Helpers.LangText.Common("Orb"))
            {
                OverrideColor = ColorFunctions.OrbWeaponType
            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 8;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.RaiseLamp;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<SineSireProj>();
            Item.shootSpeed = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<SineSireProj>() &&
                    Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[0]++;
                    break;
                }
            }

            return false;
        }
    }

    public class SineSireProj : OrbProjectile
    {
        public enum ActionState
        {
            Orbit,
            Swing_1,
            Swing_2,
            Swing_3
        }

        public const float Swing_Time = 40 * Swing_Speed_Multiplier;
        public const float Swing_Time_2 = 60 * Swing_Speed_Multiplier;
        public const float Final_Swing_Distance = 252;
        public const float Combo_Time = 8;
        public const int Swing_Speed_Multiplier = 8;

        public override float MaxThrowDistance => 462;

        ref float ComboCounter => ref Projectile.ai[0];
        public ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        ref float Timer => ref Projectile.ai[2];
        float SwingTime;
        float EasedProgress;
        Vector2 SwingStart;
        Vector2 SwingTarget;
        Vector2 SwingVelocity;
        int ComboCounter2;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SwingStart);
            writer.WriteVector2(SwingTarget);
            writer.WriteVector2(SwingVelocity);
            writer.Write(SwingTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingStart = reader.ReadVector2();
            SwingTarget = reader.ReadVector2();
            SwingVelocity = reader.ReadVector2();
            SwingTime = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 256;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = Swing_Speed_Multiplier;
        }

        public override bool? CanDamage()
        {
            //Only deal damage while swinging
            return State != ActionState.Orbit;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Orbit:
                    Orbit();
                    break;
                case ActionState.Swing_1:
                    Swing1();
                    break;
                case ActionState.Swing_2:
                    Swing2();
                    break;
                case ActionState.Swing_3:
                    Swing3();
                    break;
            }
        }


        private void Orbit()
        {
            //Orbiting
            float orbitSpeed = 5;
            float orbitDistance = 48;
            float orbitProgress = VectorHelper.Osc(0, 1, orbitSpeed);
            float easedProgress = Easing.InOutCubic(orbitProgress);

            //Hovering
            float hoverSpeed = 5;
            float hoverDistance = 24;
            float yOffset = VectorHelper.Osc(-hoverDistance, hoverDistance, hoverSpeed);

            Vector2 offset = new Vector2(orbitDistance, yOffset);
            Vector2 startVector = Owner.Center - offset;
            Vector2 endVector = Owner.Center + offset;

            //Lerp
            Vector2 targetCenter = Vector2.Lerp(startVector, endVector, easedProgress);
            Projectile.rotation = (targetCenter - Projectile.Center).ToRotation();
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetCenter, 0.12f / Swing_Speed_Multiplier);
            if (ComboCounter >= 1)
            {
                //Throw Sounds
                SoundStyle soundStyle = SoundID.Item21;
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Pitch = -0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                OrbHelper.PlaySummonSound(Projectile.position);
                Reset();
                SwingVelocity = Owner.DirectionTo(SwingTarget);
                SwingStart = Owner.Center;
                if (Main.myPlayer == Projectile.owner)
                {
                    SwingTarget = GetSwingTarget();
                    Projectile.netUpdate = true;
                }

                SwingTime = Swing_Time;
                State = ActionState.Swing_1;
                ComboCounter = 0;
                Timer = 0;
            }
        }

        private void Reset()
        {
            EasedProgress = 0;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        private void SwingDusts()
        {

        }

        private void Swing1()
        {
            SwingDusts();
            Timer++;

            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutCirc(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);
            lerpPosition.Y += MathF.Sin(EasedProgress * 8) * 256;

            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
                    //Throw Sounds
                    SoundStyle soundStyle = SoundID.Item21;
                    soundStyle.PitchVariance = 0.15f;
                    soundStyle.Pitch = -0.5f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                    OrbHelper.PlaySummonSound(Projectile.position);
                    Reset();
                    SwingVelocity = Owner.DirectionTo(SwingTarget);
                    float distance = 180;
                    SwingStart = Owner.Center + SwingVelocity.RotatedByRandom(MathHelper.TwoPi) * distance;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SwingTarget = GetSwingTarget();
                        Projectile.netUpdate = true;
                    }

                    SwingTime = Swing_Time;
                    ComboCounter2++;
                    if (ComboCounter2 >= 7)
                    {
                        ComboCounter2 = 0;
                        SwingTime = Swing_Time_2;
                        State = ActionState.Swing_3;
                    }
                    else
                    {
                        SwingTime = Swing_Time;
                        State = ActionState.Swing_2;
                    }
                    ComboCounter = 0;
                    Timer = 0;
                }
                else if (Timer > SwingTime + Combo_Time)
                {
                    ComboCounter = 0;
                    Timer = 0;
                    State = ActionState.Orbit;
                }
            }
        }

        private void Swing2()
        {
            SwingDusts();
            Timer++;

            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutCirc(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);
            lerpPosition.Y += MathF.Sin(EasedProgress * 16) * 256;

            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
                    //Throw Sounds
                    SoundStyle soundStyle = SoundID.Item21;
                    soundStyle.PitchVariance = 0.15f;
                    soundStyle.Pitch = -0.25f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                    OrbHelper.PlaySummonSound(Projectile.position);
                    Reset();
                    SwingVelocity = Owner.DirectionTo(SwingTarget);
                    SwingStart = Projectile.Center;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SwingTarget = GetSwingTarget();
                        Projectile.netUpdate = true;
                    }

                    SwingTime = Swing_Time_2;
                    ComboCounter2++;
                    if (ComboCounter2 >= 7)
                    {
                        ComboCounter2 = 0;
                        SwingTime = Swing_Time_2;
                        State = ActionState.Swing_3;
                    }
                    else
                    {
                        SwingTime = Swing_Time;
                        State = ActionState.Swing_1;
                    }
                    ComboCounter = 0;
                    Timer = 0;
                }
                else if (Timer > SwingTime + Combo_Time)
                {
                    ComboCounter = 0;
                    Timer = 0;
                    State = ActionState.Orbit;
                }
            }
        }

        private void Swing3()
        {
            SwingDusts();
            Timer++;
            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutCirc(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);
            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
            if (Timer > SwingTime)
            {
                ComboCounter = 0;
                Timer = 0;
                State = ActionState.Orbit;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<SineSireDebuff>(), 240);
            SoundStyle soundStyle;
            switch (State)
            {
                case ActionState.Swing_1:
                case ActionState.Swing_2:
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDust(target.position, Projectile.width, Projectile.height, ModContent.DustType<GunFlash>(), Scale: 0.8f);
                        Dust.NewDustPerfect(target.position, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkBlue, 1f).noGravity = true;
                    }

                    soundStyle = SoundID.SplashWeak;
                    soundStyle.PitchVariance = 0.15f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                    break;

                case ActionState.Swing_3:
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkBlue, 1f).noGravity = true;
                    }

                    for (int i = 0; i < 14; i++)
                    {

                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkBlue, 1f).noGravity = true;
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<WataBoom2>(),
                       Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                    target.SimpleStrikeNPC(Projectile.damage, hit.HitDirection);
                    soundStyle = SoundID.SplashWeak;
                    soundStyle.PitchVariance = 0.15f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.LightGoldenrodYellow.ToVector3(),
                Color.DarkGoldenrod.ToVector3(),
                new Vector3(3, 3, 3), 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();


            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.DarkBlue, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Turquoise * 0.8f, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
    
    public class WataBoom2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
    }
}