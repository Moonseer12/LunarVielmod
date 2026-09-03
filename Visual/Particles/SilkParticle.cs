using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class SilkParticle : Particle<SilkParticle>
    {
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public override void OnSpawn()
        {
            Scale = Main.rand.NextFloat(0.2f, 0.6f);
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Velocity.Y += 0.025f;
            Velocity = Velocity.RotatedByRandom(0.09f);
            Rotation += 0.01f;
            //   Scale *= 0.997f;c
            color = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(fadeIn / 180f));

            fadeIn++;
            if (fadeIn > 180)
                active = false;

        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            var textureAsset = GetTexture();
            spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
