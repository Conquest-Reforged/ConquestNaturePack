using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace conquestnaturepack.src.Blocks
{
    public class BlockLayeredModCorrected : BlockLayeredSlowDig
    {

        //Using a custom version of GetNextLayer allows us to ensure that we return null when our block is finished.
        private new Block GetNextLayer(IWorldAccessor world)
        {
            int num = CountLayers();
            if (num < 7)
            {
                return world.BlockAccessor.GetBlock(CodeWithVariant(layerGroupCode, (num + 1).ToString() ?? ""));
            }
            else if (num < 8)
            {
                return world.BlockAccessor.GetBlock(fullBlockCode);
            }
            return null;
        }


        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            if (!world.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                byPlayer.InventoryManager.ActiveHotbarSlot.MarkDirty();
                failureCode = "claimed";
                return false;
            }


            BlockLayeredModCorrected blockLayeredSlowDig = world.BlockAccessor.GetBlock(blockSel.Position.AddCopy(blockSel.Face.Opposite)) as BlockLayeredModCorrected;

            //If we are hovering over a side of a block instead of the block itself, the game glitches.
            //We use this to cause a placement failure and not actually place anything.
            if (world.BlockAccessor.GetBlock(blockSel.Position) is BlockLayeredModCorrected)
            {
                failureCode = "layer-add-to-block";
                return false;
            }

            //Make sure we're not at the maximum number of layers.
            if (blockLayeredSlowDig != null && blockLayeredSlowDig.CountLayers() < 8)
            {


                //Use variant group defined by attribute. If attribute is ommited, then any variant will be allowed.
                string variantAttribute = blockLayeredSlowDig.Attributes["variantControlCode"].AsString("null");
                if (variantAttribute == "null" || blockLayeredSlowDig.Variant[variantAttribute] == Variant[variantAttribute])
                {
                    Block nextLayer = blockLayeredSlowDig.GetNextLayer(world);
                    if (nextLayer == null)
                    {
                        //If this layer is finished, use our default placement logic.
                        AttemptToPlaceNewBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
                        return true;
                    }

                    world.BlockAccessor.SetBlock(nextLayer.BlockId, blockSel.Position.AddCopy(blockSel.Face.Opposite));
                    return true;
                }
            }

            AttemptToPlaceNewBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
            return true;
        }

        //This is actually the default logic for TryPlaceBlock.
        //If we need to place a new layer instead of adding to one, we call this.
        private bool AttemptToPlaceNewBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            bool flag = true;
            bool flag2 = false;
            BlockBehavior[] blockBehaviors = BlockBehaviors;
            foreach (BlockBehavior obj in blockBehaviors)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                bool flag3 = obj.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref handling, ref failureCode);
                if (handling != 0)
                {
                    flag = flag && flag3;
                    flag2 = true;
                }

                if (handling == EnumHandling.PreventSubsequent)
                {
                    return flag;
                }
            }

            if (flag2)
            {
                return flag;
            }

            if (CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return DoPlaceBlock(world, byPlayer, blockSel, itemstack);
            }

            return false;
        }
    }
}
