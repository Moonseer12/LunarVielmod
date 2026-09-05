using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Content.Particles
{
    public class SkullParticle : Particle<SkullParticle>
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = 1;
        }

        public override void Update()
        {
            Velocity *= 0.98f;
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
