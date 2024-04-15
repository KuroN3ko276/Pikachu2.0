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
	public List<NodeStep> cameFromNodes = new List<NodeStep>();
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
		cameFromNodes.Clear();
		closedSet.Clear();
	}

	public override bool FindPath(BlockCtrl startBlock, BlockCtrl targetBlock)
	{
		Node startNode = startBlock.blockData.node;
		Node targetNode = targetBlock.blockData.node;
		openSet.Add(startNode);
		this.cameFromNodes.Add(new NodeStep(startNode, startNode));
		NodeStep nodeStep;
		List<NodeStep> steps;

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
					nodeStep = new NodeStep(neighbor, currentNode);
					this.cameFromNodes.Add(nodeStep);
					steps = this.BuildNodeStepPath(neighbor, startNode);
					nodeStep.stepsString = this.GetStringFromSteps(steps);
					nodeStep.directionString = this.GetDirectionsFromSteps(steps);
					nodeStep.changeDirectionCount = this.CountDirectionFrom2Nodes(neighbor, startNode);
					if (nodeStep.changeDirectionCount > 3) continue;

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


	protected virtual string GetStringFromSteps(List<NodeStep> steps)
	{
		string stepsString = "";
		foreach (NodeStep nodeStep in steps)
		{
			stepsString += nodeStep.toNode.Name() + "=>";
		}
		return stepsString;
	}

	protected virtual string GetDirectionsFromSteps(List<NodeStep> steps)
	{
		string stepsString = "";
		foreach (NodeStep nodeStep in steps)
		{
			stepsString += nodeStep.direction + "=>";
		}
		return stepsString;
	}

	protected virtual int CountDirectionFrom2Nodes(Node currentNode, Node startNode)
	{
		int count = 0;
		List<NodeStep> steps = this.BuildNodeStepPath(currentNode, startNode);
		count = this.CountDirectionFromSteps(steps);
		return count;
	}

	protected virtual List<NodeStep> BuildNodeStepPath(Node currentNode, Node startNode)
	{
		List<NodeStep> steps = new List<NodeStep>();

		Node checkNode = currentNode;
		for (int i = 0; i < this.cameFromNodes.Count; i++)
		{
			NodeStep step = this.GetNodeStepByToNode(checkNode);
			steps.Add(step);
			checkNode = step.fromNode;
			if (step.fromNode == startNode) break;
		}

		//this.ShowScanStep(currentNode);
		return steps;
	}
	protected virtual Node GetFromNode(Node toNode)
	{
		return this.GetNodeStepByToNode(toNode).fromNode;
	}

	protected virtual NodeStep GetNodeStepByToNode(Node toNode)
	{
		return this.cameFromNodes.Find(item => item.toNode == toNode);
	}
}
