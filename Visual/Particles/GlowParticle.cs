using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class GlowParticle : Particle<GlowParticle>
    {
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public int MaxFrameCount = 1;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.3f, 0.6f);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.97f;
            color *= 0.99f;

            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            var textureAsset = GetTexture();
            spriteBatch.Draw(textureAsset.Value, DrawPosition, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
