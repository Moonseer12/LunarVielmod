using Stellamod.Core.Rendering.RTs;
using Terraria.ModLoader;

namespace Stellamod.Common.UI;

[Autoload(Side = ModSide.Client)]
public class UIRenderTargets : ModSystem
{
    public LazyRenderTargetProvider uiTarget = new(RenderTargetParameters.DefaultScreenTargetCreationFunc);
}
