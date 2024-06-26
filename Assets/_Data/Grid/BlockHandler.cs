using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockHandler : GridAbstract
{
    [Header("Block Handler")]
    public BlockCtrl firstBlock;
    public BlockCtrl lastBlock;
    public GameObject gameFinishObject;

	public virtual void SetNode(BlockCtrl blockCtrl)
    {
        Debug.Log("SetNode: " + blockCtrl.name);
        if (this.IsBlockRemoved(blockCtrl)) return;
        Vector3 pos;
        Transform chooseObj;
        if (this.firstBlock == null)
        {
            this.ctrl.pathfinding.DataReset();
            this.firstBlock = blockCtrl;
            pos = blockCtrl.transform.position;
            chooseObj = this.ctrl.blockSpawner.Spawn(BlockSpawner.CHOOSE, pos, Quaternion.identity);
            chooseObj.gameObject.SetActive(true);
            return;
        }

        this.lastBlock = blockCtrl;
        pos = blockCtrl.transform.position;
        chooseObj = this.ctrl.blockSpawner.Spawn(BlockSpawner.CHOOSE, pos, Quaternion.identity);
        chooseObj.gameObject.SetActive(true);

        if (this.firstBlock != this.lastBlock && this.firstBlock.blockID == this.lastBlock.blockID)
        {
            bool isPathFound = this.ctrl.pathfinding.FindPath(this.firstBlock, this.lastBlock);
            if (isPathFound)
            {
                this.ctrl.pathfinding.ShowPath();
                this.FreeBlocks();
            }
        }
		Invoke(nameof(this.ClearScreen), 0.2f);
		this.firstBlock = null;
        this.lastBlock = null;
        if (!this.ctrl.blockAuto.checkNextMove())
        {
            this.ctrl.blockAuto.ShuffleBlocks();
        }
        if (this.ctrl.gridSystem.blocks.Count == 0) 
        {
            this.ctrl.timer.StopTimer();
            gameFinishObject.SetActive(true);
		}
    }

    public virtual void ClearScreen()
	{
		List<string> names = new List<string>();
		names.Add(BlockSpawner.LINKER);
		names.Add(BlockSpawner.SCAN);
		names.Add(BlockSpawner.SCAN_STEP);
		names.Add(BlockSpawner.CHOOSE);
        this.ctrl.pathfinding.GetLineRenderer().enabled=false;
        
		foreach (Transform clone in BlockSpawner.Instance.Holder)
		{
			if (names.Contains(clone.name)) 
                BlockSpawner.Instance.DeSpawn(clone);
                //clone.gameObject.SetActive(false);
		}


	}

	protected virtual bool IsBlockRemoved(BlockCtrl blockCtrl)
    {
        Node node = blockCtrl.blockData.node;
        return !node.occupied && node.blockPlaced;
    }

    protected virtual void FreeBlocks()
    {
        this.ctrl.gridSystem.NodeFree(this.firstBlock.blockData.node);
        this.ctrl.gridSystem.NodeFree(this.lastBlock.blockData.node);
    }
}
