using Stellamod.Content.Dusts;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.WeaponsWS;

    public class WaterGun : ModItem
    {
        private int _comboCounter;

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 132;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<WaterGunNodeProj>();
            Item.shootSpeed = 6;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, -2);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool hasConnector = false;
            int connectorType = ModContent.ProjectileType<WaterGunConnectorProj>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == connectorType)
                {
                    hasConnector = true;
                    break;
                }
            }
            _comboCounter++;
            if (_comboCounter % 9 == 0)
            {
                SoundStyle soundStyle = SoundRegistry.BubbleIn;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, position);
            }

            float rot = velocity.ToRotation();
            float distance = 16;
            Vector2 offset = new Vector2(3.2f, -0.1f * player.direction).RotatedBy(rot);
            Dust.NewDustPerfect(position + (offset * distance) + new Vector2(0, 6), ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightSkyBlue, 1);

            if (!hasConnector)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, connectorType, damage, knockback, player.whoAmI);
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class WaterGunNodeProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private float Index
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer < 30)
            {
                Projectile.velocity.Y += 0.1f;
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.1f;
            return false;
        }
    }

public class WaterGunConnectorProj : ModProjectile
{
    private Vector2[] TrailPoints = new Vector2[1];
    private List<Projectile> Projectiles = new List<Projectile>();
    private List<Vector2> Connector = new List<Vector2>();
    private int Smooth;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = int.MaxValue;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 9;
    }

    public override void AI()
    {
        AI_Channel();
        AI_FillPoints();
    }

    private void AI_Channel()
    {
        //Channeling
        bool isReal = false;
        Player player = Main.player[Projectile.owner];
        Projectile.Center = player.Center;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<WaterGunNodeProj>() && proj.owner == Projectile.owner)
            {
                isReal = true;
            }
        }

        Timer++;
        if (Timer % 6 == 0)
        {
            Smooth = Main.rand.Next(65, 155);
            for (int i = 0; i < TrailPoints.Length; i++)
            {
                Vector2 trailPoint = TrailPoints[i];
                if (Main.rand.NextBool(300))
                {
                    Dust.NewDustPerfect(trailPoint, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Aqua, 2f).noGravity = true;
                }
            }
        }
        if (!isReal)
        {
            Projectile.Kill();
        }
    }

    private void AI_FillPoints()
    {
        //Get the points to connect
        Connector.Clear();
        Projectiles.Clear();
        int nodeType = ModContent.ProjectileType<WaterGunNodeProj>();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.owner != Projectile.owner)
                continue;
            if (proj.type != nodeType)
                continue;
            Projectiles.Add(proj);
        }

        Projectiles.Sort((x, y) => y.timeLeft.CompareTo(x.timeLeft));
        for (int i = 1; i < Projectiles.Count; i++)
        {
            for (float j = 0; j < 8f; j++)
            {
                Connector.Add(Vector2.Lerp(Projectiles[i - 1].Center, Projectiles[i].Center, j / 8f));
            }

        }


        TrailPoints = Connector.ToArray();
        for (int i = 1; i < TrailPoints.Length - 1; i++)
        {
            float p = i / (float)TrailPoints.Length - 1;
            ref Vector2 pos = ref TrailPoints[i];
            ref Vector2 nextPos = ref TrailPoints[i + 1];
            Vector2 vec = nextPos - pos;
            vec = vec.RotatedBy(MathHelper.ToRadians(90));
            vec *= p;

            pos += vec * MathF.Sin(Main.GlobalTimeWrappedHourly * -12 + p * 24);
            pos += vec * MathF.Sin((Main.GlobalTimeWrappedHourly + 4) * -12 + p * 12);

        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //This damages everything in the trail
        Vector2[] positions = TrailPoints;
        float collisionPoint = 0;
        for (int i = 1; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            Vector2 previousPosition = positions[i - 1];
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                return true;
        }
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}