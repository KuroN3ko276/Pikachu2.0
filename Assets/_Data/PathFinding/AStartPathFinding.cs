using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStar : AbstractPathfinding
{
	[Header("A* Search")]
	public GridManagerCtrl ctrl;
	public List<Node> openSet = new List<Node>();
	public HashSet<Node> closedSet = new HashSet<Node>();
	public List<Node> finalPath = new List<Node>();
	//public List<NodeStep> cameFromNodes = new List<NodeStep>();
	public LineRenderer lineRenderer;

	protected override void LoadComponents()
	{
		base.LoadComponents();
		LoadCtrl();
		LoadLineRenderer();
	}

	protected virtual void LoadCtrl()
	{
		if (ctrl != null) return;
		ctrl = transform.parent.GetComponent<GridManagerCtrl>();
		Debug.LogWarning(transform.name + " LoadCtrl", gameObject);
	}

	protected virtual void LoadLineRenderer()
	{
		if (lineRenderer != null) return;
		lineRenderer = transform.GetComponent<LineRenderer>();
		Debug.LogWarning(transform.name + " LoadLineRenderer", gameObject);
	}

	public override LineRenderer GetLineRenderer()
	{
		return lineRenderer;
	}

	public override void DataReset()
	{
		openSet.Clear();
		finalPath.Clear();
		//cameFromNodes.Clear();
		closedSet.Clear();
	}

	public override bool FindPath(BlockCtrl startBlock, BlockCtrl targetBlock)
	{
		if (startBlock == null || targetBlock == null)
		{
			Debug.LogWarning("startBlock or targetBlock is null");
		}
		Node startNode = startBlock.blockData.node;
		Node targetNode = targetBlock.blockData.node;
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node currentNode = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
				{
					currentNode = openSet[i];
				}
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if (currentNode == targetNode)
			{
				this.finalPath = this.BuildFinalPath(startNode, targetNode);
				break;
			}

			foreach (Node neighbor in currentNode.Neighbors())
			{
				if (neighbor == null) continue;
				if (this.closedSet.Contains(neighbor)) continue;
				if (!this.IsValidPosition(neighbor, targetNode)) continue;

				int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

				// Additional cost for changing direction
				if (currentNode.parent != null && GetDirectionChangeCost(currentNode.parent, currentNode, neighbor) > 0)
				{
					newCostToNeighbor += 10; // Additional cost for changing direction, you can adjust this value
				}

				if (!openSet.Contains(neighbor) || newCostToNeighbor < neighbor.gCost)
				{
					neighbor.gCost = newCostToNeighbor;
					neighbor.hCost = GetDistance(neighbor, targetNode);
					neighbor.parent = currentNode;

					if (!openSet.Contains(neighbor))
					{
						openSet.Add(neighbor);
					}
				}
			}
		}
		this.ShowPath();
		return this.IsPathFound(); // No path found

	}

	private bool IsValidPosition(Node node, Node startNode)
	{
		if (node == startNode) return true;

		return !node.occupied;
	}

	static int GetDirectionChangeCost(Node fromNode, Node toNode, Node neighborNode)
	{
		// If any of the nodes is null, return 0 (no change in direction)
		if (fromNode == null || toNode == null || neighborNode == null)
		{
			return 0;
		}

		// Calculate the direction vectors from fromNode to toNode and from toNode to neighborNode
		int fromX = toNode.x - fromNode.x;
		int fromY = toNode.y - fromNode.y;
		int toX = neighborNode.x - toNode.x;
		int toY = neighborNode.y - toNode.y;

		// Calculate the dot product of the two direction vectors
		int dotProduct = (fromX * toX) + (fromY * toY);

		// If the dot product is negative, there is a change in direction
		// Return a positive cost to indicate the change in direction
		if (dotProduct < 0)
		{
			return 1; // You can adjust this cost based on your preference
		}

		// No change in direction
		return 0;
	}

	protected virtual int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.x - nodeB.x);
		int distY = Mathf.Abs(nodeA.y - nodeB.y);

		if (distX > distY)
			return 14 * distY + 10 * (distX - distY);
		else
			return 14 * distX + 10 * (distY - distX);
	}
	//kiểm tra xem có đi được từ Node có bị chiếm không
	protected virtual bool IsPathFound()
	{
		return finalPath.Count > 0;
	}

	protected virtual List<Node> BuildFinalPath(Node startNode, Node targetNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Add(startNode);

		path.Reverse();
		return path;
	}

	//Hiển thị đường đi ra màn hình
	protected virtual void ShowPath()
	{
		lineRenderer.enabled = true;
		List<Vector3> listPoint = new List<Vector3>();
		foreach (Node node in finalPath)
		{
			listPoint.Add(node.nodeObj.transform.position);
		}
		lineRenderer.positionCount = listPoint.Count;
		lineRenderer.SetPositions(listPoint.ToArray());
	}

	protected virtual int CountDirectionFromSteps(List<NodeStep> steps)
	{
		NodeDirections lastDirection = NodeDirections.noDirection;
		NodeDirections currentDirection;
		int turnCount = 0;
		foreach (NodeStep nodeStep in steps)
		{
			currentDirection = nodeStep.direction;
			if (currentDirection != lastDirection)
			{
				lastDirection = currentDirection;
				turnCount++;
			}
		}

		return turnCount;
	}
}
