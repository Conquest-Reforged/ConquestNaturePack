using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace conquestnaturepack.src.Blocks.Behaviors
{
    public class ModCorrectedHorizontalOrientableBlockBehavior : BlockBehaviorHorizontalOrientable
    {
        public ModCorrectedHorizontalOrientableBlockBehavior(Block block) : base(block)
        {
        }

        //We override GetDrops to not use the default slab behaviour, and instead use whatever our block drops.
        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return null;
        }

    }
}
