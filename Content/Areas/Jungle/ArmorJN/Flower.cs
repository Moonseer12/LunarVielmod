using Stellamod.Content.Ammo;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ArmorJN
{
    public class FlowerLeafAura : ModProjectile
    {
        public Vector2[] ChainPos;
        public int FrameCounter;
        public int FrameTick;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            ChainPos = new Vector2[16];
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (owner == player)
                    continue;

                float distance = Vector2.Distance(owner.Center, player.Center);
                if (distance <= 64)
                {
                    player.AddBuff(ModContent.BuffType<FlowerPower>(), 60);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            MakeOval();
            SnapToPlayer();
        }

        private void MakeOval()
        {
            //Calculate Points On Oval
            Vector2 chainCenter = Projectile.Center;
            float ovalXRadius = 64;
            float ovalYRadius = 64;

            float ovalAngle = MathHelper.TwoPi + MathHelper.PiOver4 / 2;
            DrawHelper.DrawChainOval(chainCenter, ovalXRadius, ovalYRadius, ovalAngle, 0,
                ref ChainPos);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawFlowerChains(chainTexture, ChainPos, animationFrame, Projectile.alpha / 255f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void SnapToPlayer()
        {
            //Snap to NPC to follow
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<FlowerPlayer>().hasQuiver)
            {
                //Fade In
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.alpha = 255;
                Projectile.Center = owner.Center;
                Projectile.timeLeft = 3600;
            }
            else
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha <= 0)
                {
                    Projectile.Kill();
                }
            }
        }
    }

    public class FlowerPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 18;
        }
    }

    public class FlowerPlayer : ModPlayer
    {
        public bool hasQuiver;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasQuiver = false;
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly && hasQuiver)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordThrow"), position);
                type = ModContent.ProjectileType<FlowerArrow>();
                damage += 2;
                velocity *= 2f;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class FlowerHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 12; 
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.16f;
            player.GetDamage(DamageClass.Ranged) += 0.16f;
            player.hasAngelHalo = true;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FlowerRobe>() && legs.type == ModContent.ItemType<FlowerPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);
            player.lifeRegen += 1;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlowerLeafAura>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<FlowerLeafAura>(), 0, 0, player.whoAmI);
            }
            player.GetModPlayer<FlowerPlayer>().hasQuiver = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class FlowerRobe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 16;
        }
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 60;
            player.GetDamage(DamageClass.Melee) += 0.18f;
            player.GetDamage(DamageClass.Ranged) += 0.18f;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class FlowerPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 10;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.3f; 
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.flowerBoots = true;
        }
    }
}