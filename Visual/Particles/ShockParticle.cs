using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class ShockParticle : Particle<ShockParticle>
    {
        public int FrameWidth = 143;
        public int FrameHeight = 143;
        private float _timer;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);

        }
        public override void Update()
        {
            _timer++;
            Scale *= 1.2f;
            color = Color.Lerp(Color.White, Color.Black, fadeIn / 60f);
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
