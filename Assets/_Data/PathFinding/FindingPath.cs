using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FindingPath : AbstractPathfinding
{
    [Header("Finding Path")]
    public GridManagerCtrl ctrl;
    public LineRenderer lineRenderer;
    public List<Node> finalPath = new List<Node>();

	protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCtrl();
        this.LoadLineRenderer();
    }

    protected virtual void LoadCtrl()
    {
        if (this.ctrl != null) return;
        this.ctrl = transform.parent.GetComponent<GridManagerCtrl>();
        Debug.LogWarning(transform.name + " LoadCtrl", gameObject);
    }

    protected virtual void LoadLineRenderer()
    {
        if(this.lineRenderer != null) return;
        this.lineRenderer = transform.GetComponent<LineRenderer>();
		Debug.LogWarning(transform.name + " LoadLineRenderer", gameObject);
	}
	public override LineRenderer GetLineRenderer()
	{
		return lineRenderer;
	}

	public override void DataReset()
    {
        finalPath.Clear();
    }

    public override bool FindPath(BlockCtrl startBlock, BlockCtrl targetBlock)
    {
        List<Node> startList = new List<Node>();
        List<Node> targetList = new List<Node>();
        //Debug.Log("FindPath");
        Node startNode = startBlock.blockData.node;
        Node targetNode = targetBlock.blockData.node;
        
        if(this.isRoad(startNode,targetNode))
        {
            finalPath.Add(startNode);
            finalPath.Add(targetNode);
            return this.IsPathFound();
        }

        startList = this.findHorizontalAndVertical(startNode);
        targetList = this.findHorizontalAndVertical(targetNode);
        
        foreach(Node node in startList)
        {
            //Nếu 1 node có trong cả targetList và startList thì có đường đi giữa 2 block
            if(targetList.Contains(node))
            {
                finalPath.Add(startNode);
                finalPath.Add(node);
                finalPath.Add(targetNode);
                return this.IsPathFound();
            }

            foreach(Node node2 in targetList)
            {
                if( (node.x == node2.x || node.y == node2.y) && this.isRoad(node,node2) )
                {
                    this.finalPath.Add(startNode);
                    this.finalPath.Add(node);
					this.finalPath.Add(node2);
					this.finalPath.Add(targetNode);
                    return this.IsPathFound();
                }
            }
        }
        
        return this.IsPathFound();
    }

    protected List<Node> findHorizontalAndVertical(Node startNode)
    {
        List<Node> nodes = new List<Node>();
        foreach(Node node in this.ctrl.gridSystem.nodes)
        {
            if( ( node.x == startNode.x || node.y == startNode.y ) && this.isRoad(node,startNode) )
            {
                nodes.Add(node);
            }
        }

        return nodes;
    }

    protected bool isRoad(Node startNode,Node targetNode)
    {
        if(startNode == null || targetNode == null) return false;
        if(startNode == targetNode) return true;
        Node currentNode = new Node();
        currentNode = startNode;
        if(startNode.x == targetNode.x)
        {
			if (startNode.y < targetNode.y)
			{
				while (currentNode != targetNode)
				{
					if (!this.IsValidPosition(startNode, currentNode.up)) return false;
					currentNode = currentNode.up;
				}
			}
			else
			{
				while (currentNode != targetNode)
				{
					if (!this.IsValidPosition(startNode, currentNode.down)) return false;
					currentNode = currentNode.down;
				}
			}
		}
        else if(startNode.y == targetNode.y)
        {
            
			if (startNode.x < targetNode.x)
			{
				while (currentNode != targetNode)
				{
					if (!this.IsValidPosition(startNode, currentNode.right)) return false;
					currentNode = currentNode.right;
				}
			}
			else
			{
				while (currentNode != targetNode)
				{
					if (!this.IsValidPosition(startNode, currentNode.left)) return false;
					currentNode = currentNode.left;
				}
			}
		}
        return true;
    }

	private bool IsValidPosition(Node startNode, Node targetNode)
	{
		if (startNode == targetNode) return true;

		//if( targetNode.occupied) return false;

        return !targetNode.occupied;

	}

	protected virtual bool IsPathFound()
    {
        int nodeCount = this.finalPath.Count;
        return nodeCount > 0;
    }

   
    public override void ShowPath()
    {
        Debug.Log("DrawRoad");
        lineRenderer.enabled = true;
        List<Vector3> listPoint = new List<Vector3>(); 
        Vector3 pos;
        foreach (Node node in this.finalPath)
        {
            pos = node.nodeObj.transform.position;
            listPoint.Add(pos);
            //Transform linker = this.ctrl.blockSpawner.Spawn(BlockSpawner.LINKER, pos, Quaternion.identity);
            //linker.gameObject.SetActive(true);
        }
		lineRenderer.positionCount = listPoint.Count;
		lineRenderer.SetPositions(listPoint.ToArray<Vector3>());
	}

}
