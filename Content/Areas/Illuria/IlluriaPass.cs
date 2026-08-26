using Terraria.IO;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Illuria;

public class IlluriaPass : GenPass
{
    public IlluriaPass() : base("Illuria", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        /*Rectangle rectangle = StructureLoader.ReadRectangle("Structures/Illuria");
        progress.Message = "Niivi protecting the cities above.";
        Point Loc = new(GenVars.snowOriginRight - 150, (int)Main.worldSurface - 350);
        rectangle.Location = Loc;
        Structurizer.ProtectStructure(Loc, "Structures/Illuria");*/
    }
}