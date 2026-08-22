using System;

namespace Stellamod.Tiles
{
    public class TileTagAttribute : Attribute
    {
        public TileTags[] Tags = { };

        public TileTagAttribute(params TileTags[] tags)
        {
            Tags = tags;
        }
    }

    public enum TileTags
    {
        Indestructible,
        IndestructibleNoGround,
        VineSway,
        ChandelierSway
    }
}