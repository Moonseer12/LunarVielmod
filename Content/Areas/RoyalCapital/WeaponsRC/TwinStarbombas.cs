using Stellamod.Common.MagicCauldron;
using Stellamod.Common.OrbSystem;
using Stellamod.Content.CommonMaterials;
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

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class TwinStarbombasDebuff : ModBuff
    {
        public static readonly int TagDamage = 7;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }


    public class TwinStarbombasDebuffNPC : GlobalNPC
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
                projectile.damage += TwinStarbombasDebuff.TagDamage;
            }
        }
    }

    public class TwinStarbombas : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TwinStarbombasDebuff.TagDamage);
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
            Item.width = 32;
            Item.height = 48;
            Item.damage = 44;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 8;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Pink;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<TwinStarbombasProj1>();
            Item.shootSpeed = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int found = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<TwinStarbombasProj1>()
                    && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[0]++;
                    found++;
                }
                if (Main.projectile[i].type == ModContent.ProjectileType<TwinStarbombasProj2>()
                    && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[0]++;
                    found++;
                }
                if (found >= 2)
                {
                    break;
                }
            }

            return false;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankOrb>();
        }
    }
    public class TwinStarbombasProj1 : OrbProjectile
    {
        public enum ActionState
        {
            Orbit,
            Swing_1,
            Swing_2,
            Swing_3
        }

        public const float Swing_Revolutions = 1.33f;
        public const float Swing_Time = 34 * Swing_Speed_Multiplier;
        public const float Swing_Time_2 = 53 * Swing_Speed_Multiplier;
        public const float Final_Swing_Distance = 252;
        public const float Combo_Time = 8;
        public const int Swing_Speed_Multiplier = 16;

        public override float MaxThrowDistance => 512;

        ref float ComboCounter => ref Projectile.ai[0];
        public ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        ref float Timer => ref Projectile.ai[2];
        float SwingTime;
        float EasedProgress;
        float OrbitRotation;
        float OrbitSwingDistance;
        Vector2 SwingStart;
        Vector2 SwingTarget;
        Vector2 SwingVelocity;
        int DustTimer;

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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 132;
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

        private void Reset()
        {
            ComboCounter = 0;
            Timer = 0;
            EasedProgress = 0;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
            OrbitSwingDistance = Vector2.Distance(Owner.Center, Main.MouseWorld);
        }

        private void Orbit()
        {
            //Orbit around the player
            float orbitDistance = 256;
            OrbitRotation += 0.003f;
            Vector2 targetOrbitPos = MovementHelper.OrbitAround(Owner.Center, Vector2.UnitY, orbitDistance, OrbitRotation);

            //Lerp
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetOrbitPos, 0.12f / Swing_Speed_Multiplier);
            if (ComboCounter >= 1)
            {
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
            }
        }

        private void SwingDusts()
        {
            DustTimer++;
            if (DustTimer >= 4 * Swing_Speed_Multiplier)
            {
                DustTimer = 0;
                //Get a random color
                Color randColor = Main.rand.NextColor(Color.White, Main.DiscoColor, Color.Black);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GunFlash>(), newColor: randColor, Scale: 0.8f);
            }
        }

        private void Swing1()
        {
            SwingDusts();
            Timer++;

            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutExpo(progress);
            float orbitRotation = MathHelper.Lerp(-MathHelper.TwoPi * Swing_Revolutions, 0, EasedProgress);
            float orbitDistance = MathHelper.Lerp(0, OrbitSwingDistance, EasedProgress);

            Vector2 start = SwingStart;
            Vector2 end = MovementHelper.OrbitAround(Owner.Center, SwingVelocity.RotatedBy(MathHelper.PiOver2), orbitDistance, orbitRotation);
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

            Projectile.Center = Vector2.Lerp(Projectile.Center, end, EasedProgress);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
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
                    State = ActionState.Swing_2;
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
            EasedProgress = Easing.SpikeInOutExpo(progress);
            float orbitRotation = MathHelper.Lerp(MathHelper.TwoPi * Swing_Revolutions, 0, EasedProgress);
            float orbitDistance = MathHelper.Lerp(0, OrbitSwingDistance, EasedProgress);
            Vector2 end = MovementHelper.OrbitAround(Owner.Center, SwingVelocity.RotatedBy(MathHelper.PiOver2), orbitDistance, -orbitRotation);

            Projectile.Center = Vector2.Lerp(Projectile.Center, end, EasedProgress);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
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
                    State = ActionState.Swing_3;
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
            EasedProgress = Easing.SpikeInOutExpo(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;

            //This should be cool

            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);
            lerpPosition += MathF.Sin(EasedProgress * 32) * SwingVelocity.RotatedBy(MathHelper.PiOver2) * 8;

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
            target.AddBuff(ModContent.BuffType<TwinStarbombasDebuff>(), 240);
            switch (State)
            {
                case ActionState.Swing_1:
                case ActionState.Swing_2:
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                            ModContent.DustType<GunFlash>(), newColor: Color.Black, Scale: 0.8f);
                    }

                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya1") { PitchVariance = 0.15f }, Projectile.position);
                            break;
                        case 1:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya2") { PitchVariance = 0.15f }, Projectile.position);
                            break;
                        case 2:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3") { PitchVariance = 0.15f }, Projectile.position);
                            break;
                    }
                    break;

                case ActionState.Swing_3:
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                            ModContent.DustType<GunFlash>(), newColor: Main.DiscoColor, Scale: 0.8f);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        //Get a random velocity
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 8);

                        //Get a random color
                        Color randColor = Main.rand.NextColor(Color.White, Main.DiscoColor, Color.Black);
                        float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<StarsBoom2>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);

                    target.SimpleStrikeNPC(Projectile.damage, hit.HitDirection);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower3") { PitchVariance = 0.15f }, Projectile.position);
                    break;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);
            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Main.DiscoColor, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }

    public class TwinStarbombasProj2 : OrbProjectile
    {
        public enum ActionState
        {
            Orbit,
            Swing_1,
            Swing_2,
            Swing_3
        }

        public const float Swing_Revolutions = 1.33f;
        public const float Swing_Time = 34 * Swing_Speed_Multiplier;
        public const float Swing_Time_2 = 53 * Swing_Speed_Multiplier;
        public const float Final_Swing_Distance = 252;
        public const float Combo_Time = 8;
        public const int Swing_Speed_Multiplier = 16;

        public override float MaxThrowDistance => 512;

        ref float ComboCounter => ref Projectile.ai[0];
        public ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        ref float Timer => ref Projectile.ai[2];
        float SwingTime;
        float EasedProgress;
        float OrbitRotation;
        float OrbitSwingDistance;
        Vector2 SwingStart;
        Vector2 SwingTarget;
        Vector2 SwingVelocity;
        int DustTimer;

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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 132;
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

        private void Reset()
        {
            ComboCounter = 0;
            Timer = 0;
            EasedProgress = 0;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
            OrbitSwingDistance = Vector2.Distance(Owner.Center, Main.MouseWorld);
        }

        private void Orbit()
        {
            //Orbit around the player
            float orbitDistance = 128;
            OrbitRotation -= 0.003f;
            Vector2 targetOrbitPos = MovementHelper.OrbitAround(Owner.Center, Vector2.UnitY, orbitDistance, OrbitRotation);

            //Lerp
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetOrbitPos, 0.12f / Swing_Speed_Multiplier);
            if (ComboCounter >= 1)
            {
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
            }
        }

        private void SwingDusts()
        {
            DustTimer++;
            if (DustTimer >= 4 * Swing_Speed_Multiplier)
            {
                DustTimer = 0;
                //Get a random color
                Color randColor = Main.rand.NextColor(Color.White, Main.DiscoColor, Color.Black);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GunFlash>(), newColor: randColor, Scale: 0.8f);
            }
        }

        private void Swing1()
        {
            SwingDusts();
            Timer++;

            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutExpo(progress);
            float orbitRotation = MathHelper.Lerp(-MathHelper.TwoPi * Swing_Revolutions, 0, EasedProgress);
            float orbitDistance = MathHelper.Lerp(0, OrbitSwingDistance, EasedProgress);

            Vector2 end = MovementHelper.OrbitAround(Owner.Center, SwingVelocity.RotatedBy(-MathHelper.PiOver2), orbitDistance, orbitRotation);

            Projectile.Center = Vector2.Lerp(Projectile.Center, end, EasedProgress);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
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
                    State = ActionState.Swing_2;
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
            EasedProgress = Easing.SpikeInOutExpo(progress);
            float orbitRotation = MathHelper.Lerp(MathHelper.TwoPi * Swing_Revolutions, 0, EasedProgress);
            float orbitDistance = MathHelper.Lerp(0, OrbitSwingDistance, EasedProgress);

            Vector2 start = SwingStart;
            Vector2 end = MovementHelper.OrbitAround(Owner.Center, SwingVelocity.RotatedBy(-MathHelper.PiOver2), orbitDistance, -orbitRotation);
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

            Projectile.Center = Vector2.Lerp(Projectile.Center, end, EasedProgress);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
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
                    State = ActionState.Swing_3;
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
            EasedProgress = Easing.SpikeInOutExpo(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;

            //This should be cool

            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);
            lerpPosition += MathF.Sin(EasedProgress * 32) * (SwingVelocity.RotatedBy(MathHelper.PiOver2)) * 256;

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
            target.AddBuff(ModContent.BuffType<TwinStarbombasDebuff>(), 240);
            switch (State)
            {
                case ActionState.Swing_1:
                case ActionState.Swing_2:
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                            ModContent.DustType<GunFlash>(), newColor: Color.Black, Scale: 0.8f);
                    }

                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya1") { PitchVariance = 0.15f }, Projectile.position);
                            break;
                        case 1:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya2") { PitchVariance = 0.15f }, Projectile.position);
                            break;
                        case 2:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3") { PitchVariance = 0.15f }, Projectile.position);
                            break;
                    }
                    break;

                case ActionState.Swing_3:
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                            ModContent.DustType<GunFlash>(), newColor: Main.DiscoColor, Scale: 0.8f);
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<StarsBoom2>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                    target.SimpleStrikeNPC(Projectile.damage, hit.HitDirection);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower3") { PitchVariance = 0.15f }, Projectile.position);
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Main.DiscoColor, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }

    public class StarsBoom2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 25;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = 107;
            Projectile.height = 103;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.scale = 1f;

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
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 25)
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