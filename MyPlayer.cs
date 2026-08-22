using Stellamod.Items.Weapons.Melee;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod
{
    public class MyPlayer : ModPlayer
    {
        public int SwordCombo;
        public int SwordComboR;
        public int OnionDamage = 0;
        public bool Onion1 = false;
        public bool Onion2 = false;
        public bool Onion3 = false;
        public bool Onion4 = false;
        private float shakeDrama;
        public bool RadiantBomb = false;
        public int RadiantBombCooldown = 0;
        public int Bridget = 0;

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

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (RadiantBomb && RadiantBombCooldown <= 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSpawnEffect>(), 490, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<GoldsSlashProj>(), 400, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<GoldsSlashProj>(), 401, 1f, Player.whoAmI);
                }
                RadiantBombCooldown = 220;
            }
        }

        public override void ResetEffects()
        {
            RadiantBomb = false;
            if (SwordComboR <= 0)
            {
                SwordCombo = 0;
                SwordComboR = 0;
            }
            else
            {
                SwordComboR--;
            }
            Onion1 = false;
            Onion2 = false;
            Onion3 = false;
            Onion4 = false;
            OnionDamage = 0;
        }

        public override void OnEnterWorld()
        {
            Main.NewText(LangText.Misc("EnterWorld"));
        }

        public override void PostUpdate()
        {
            Player player = Main.LocalPlayer;
            if (!player.active)
                return;
            MyPlayer CVA = player.GetModPlayer<MyPlayer>();
            bool expertMode = Main.expertMode;
            #region//--------------------------------------------------------------------- Bridget lmaooo (1000 lines)
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<Bridget>())
                {
                    Bridget++;
                    if (Bridget > 1080)
                    {
                        int combatText = -1;
                        switch (Main.rand.Next(30))
                        {

                            case 0:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.1"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 1:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.2"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 2:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.3"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 4:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.4"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 5:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.5"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 6:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.6"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 7:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.7"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 8:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.8"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 9:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.9"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 10:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.10"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 11:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.11"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 12:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.12"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 13:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.13"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 14:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.14"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 15:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.15"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 16:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.16"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 17:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.17"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 18:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.18"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 19:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.19"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 20:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.20"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 21:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.21"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 22:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.22"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 23:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.23"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 24:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.24"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 25:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.25"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 26:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.26"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 27:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.27"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 28:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.28"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 29:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.29"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                        }
                        if (combatText != -1)
                        {
                            CombatText text = Main.combatText[combatText];
                            text.lifeTime = 360;
                        }
                    }
                    break;
                }
            }
            #endregion 
        }

        public override bool PreItemCheck()
        {
            if (SwordComboR > 0)
            {
                SwordComboR--;
                if (SwordComboR == 0)
                {
                    SwordCombo = 0;
                }
            }
            return true;
        }
    }
}