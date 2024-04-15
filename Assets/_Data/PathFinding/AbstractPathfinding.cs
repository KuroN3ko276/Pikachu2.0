using System;
using UnityEngine;

public abstract class AbstractPathfinding : SaiMonoBehaviour
{
    public abstract bool FindPath(BlockCtrl startBlock, BlockCtrl targetBlock);

    public abstract void DataReset();

    public abstract LineRenderer GetLineRenderer();

    public abstract void ShowPath();
}
