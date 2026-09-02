using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public class VeilGenTester : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.useTime = 1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 1;
    }

    public override bool? UseItem(Player player)
    {
        return true;
    }

}
