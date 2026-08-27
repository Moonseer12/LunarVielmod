using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class SerpentSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Color GlowWhiteColor;
        public bool GlowWhite;
        public float GlowTimer;
        public Rectangle? Frame;
        public Vector2 Size => Texture.Size();
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale = 1f;
        public bool Eaten;
        public int FrameCounter;
        public int FrameTick;
        public SerpentSegment(Projectile projectile)
        {
            Position = projectile.position;
            Rotation = 0;
            Velocity = Vector2.Zero;
            Eaten = false;
        }
    }
}