using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BlockAuto : GridAbstract
{
    [Header("Block Auto")]
    public BlockCtrl firstBlock;
    public BlockCtrl secondBlock;


    public virtual void ShowHint()
    {
        Debug.LogWarning("ShowHint");

        List<BlockCtrl> sameBlocks = new List<BlockCtrl>();
        foreach(BlockCtrl blockCtrl in this.ctrl.gridSystem.blocks)
        {
            sameBlocks.Clear();
            sameBlocks = this.GetSameBlocks(blockCtrl);
            foreach(BlockCtrl sameBlock in sameBlocks)
            {
                this.ctrl.pathfinding.DataReset();
                bool found = GridManagerCtrl.Instance.pathfinding.FindPath(blockCtrl, sameBlock);
                if (found)
                {
                    this.firstBlock = blockCtrl;
					this.secondBlock = sameBlock;
                    Transform chooseObj;
                    Vector3 firstPos = firstBlock.transform.position;
                    Vector3 secondPos = secondBlock.transform.position;
                    chooseObj=this.ctrl.blockSpawner.Spawn(BlockSpawner.CHOOSE, firstPos, Quaternion.identity);
					chooseObj.gameObject.SetActive(true);
					chooseObj=this.ctrl.blockSpawner.Spawn(BlockSpawner.CHOOSE, secondPos, Quaternion.identity);
					chooseObj.gameObject.SetActive(true);
					return;
                }
            }
        }

        Debug.Log("Not Found");
    }

    protected virtual List<BlockCtrl> GetSameBlocks(BlockCtrl checkBlock)
    {
        List<BlockCtrl> sameBlocks = new List<BlockCtrl>();
        foreach(BlockCtrl blockCtrl in this.ctrl.gridSystem.blocks)
        {
            if (blockCtrl == checkBlock) continue;
            if (blockCtrl.blockID == checkBlock.blockID) sameBlocks.Add(blockCtrl);
        }

        return sameBlocks;
    }

    public virtual void ShuffleBlocks()
    {
        BlockCtrl randomBlock;
        foreach (BlockCtrl blockCtrl in this.ctrl.gridSystem.blocks)
        {
            randomBlock = this.ctrl.gridSystem.GetRandomBlock();
            this.SwapBlocks(blockCtrl, randomBlock);
        }
        //this.SwapBlocks(this.ctrl.gridSystem.blocks[1], this.ctrl.gridSystem.blocks[8]);
        Debug.LogWarning("ShuffleBlocks");
    }

    //protected virtual void SwapBlocks(BlockCtrl blockCtrl, BlockCtrl randomBlock) {
    //    if (blockCtrl == randomBlock) return;
    //    if (!randomBlock.blockData.node.occupied) return;
    //    BlockCtrl temp = blockCtrl;

    //    blockCtrl.spriteRender.sprite = randomBlock.sprite;
    //    blockCtrl.sprite = randomBlock.sprite;
    //    blockCtrl.blockId = randomBlock.blockId;
    //    blockCtrl.blockData.node = randomblock.blockdata.node;
    //    blockCtrl.blockData.node.up = randomblock.blockdata.node.up;
    //    blockCtrl.blockData.node.down = randomblock.blockdata.node.down;
    //    blockCtrl.blockData.node.left = randomblock.blockdata.node.left;
    //    blockCtrl.blockData.node.right = randomblock.blockdata.node.right;
    //    //blockctrl.name = randomblock.name;

    //    randomblock.spriterender.sprite = temp.sprite;
    //    randomblock.sprite = temp.sprite;
    //    randomblock.blockid = temp.blockid;
    //    randomblock.blockdata.node = temp.blockdata.node;
    //    randomblock.neighbors = temp.neighbors;
    //    randomblock.blockdata.node.up = temp.blockdata.node.up;
    //    randomblock.blockdata.node.down = temp.blockdata.node.down;
    //    randomblock.blockdata.node.left = temp.blockdata.node.left;
    //    randomblock.blockdata.node.right = temp.blockdata.node.right;
    //    //randomblock.name = temp.name;
    //}
    protected virtual void SwapBlocks(BlockCtrl blockCtrl, BlockCtrl randomBlock)
    {
        if (blockCtrl == randomBlock) return;
        BlockCtrl temp = new BlockCtrl();
        temp.blockID = blockCtrl.blockID;
        temp.sprite = blockCtrl.sprite;

        blockCtrl.sprite = randomBlock.sprite;
        blockCtrl.blockID = randomBlock.blockID;
        blockCtrl.SetSprite(blockCtrl.sprite);

        randomBlock.sprite = temp.sprite;
        randomBlock.blockID = temp.blockID;
		randomBlock.SetSprite(randomBlock.sprite);
		Debug.LogWarning("SwapBlock");
	}
}
