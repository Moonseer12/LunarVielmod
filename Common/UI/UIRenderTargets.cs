using Stellamod.Core.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Common.UI;

[Autoload(Side = ModSide.Client)]
public class UIRenderTargets : ModSystem
{
    public RenderTargetProvider uiTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
}
