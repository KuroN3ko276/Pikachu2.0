//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class AStar : AbstractPathfinding
//{
//	[Header("A* Search")]
//	public GridManagerCtrl ctrl;
//	public List<Node> openSet = new List<Node>();
//	public HashSet<Node> closedSet = new HashSet<Node>();
//	public List<Node> finalPath = new List<Node>();
//	public List<NodeStep> cameFromNodes = new List<NodeStep>();
//	public LineRenderer lineRenderer;

//	protected override void LoadComponents()
//	{
//		base.LoadComponents();
//		LoadCtrl();
//		LoadLineRenderer();
//	}

//	protected virtual void LoadCtrl()
//	{
//		if (ctrl != null) return;
//		ctrl = transform.parent.GetComponent<GridManagerCtrl>();
//		Debug.LogWarning(transform.name + " LoadCtrl", gameObject);
//	}

//	protected virtual void LoadLineRenderer()
//	{
//		if (lineRenderer != null) return;
//		lineRenderer = transform.GetComponent<LineRenderer>();
//		Debug.LogWarning(transform.name + " LoadLineRenderer", gameObject);
//	}

//	public override LineRenderer GetLineRenderer()
//	{
//		return lineRenderer;
//	}

//	public override void DataReset()
//	{
//		openSet.Clear();
//		finalPath.Clear();
//		cameFromNodes.Clear();
//		closedSet.Clear();
//	}

//	public override bool FindPath(BlockCtrl startBlock, BlockCtrl targetBlock)
//	{
//		Node startNode = startBlock.blockData.node;
//		Node targetNode = targetBlock.blockData.node;

//		openSet.Add(startNode);
//		cameFromNodes.Add(new NodeStep(startNode, startNode));

//		NodeStep nodeStep;
//		List<NodeStep> steps;
//		while (openSet.Count > 0)
//		{
//			Node currentNode = openSet.OrderBy(n => n.fCost).First();

//			if (currentNode == targetNode)
//			{
//				finalPath = BuildFinalPath(startNode, targetNode);
//				break;
//			}

//			openSet.Remove(currentNode);
//			closedSet.Add(currentNode);

//			foreach (Node neighbor in currentNode.Neighbors())
//			{
//				if (closedSet.Contains(neighbor) || !this.IsValidPosition(neighbor, targetNode))
//					continue;

//				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbor);
//				if (newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor))
//				{
//					neighbor.gCost = newMovementCostToNeighbour;
//					neighbor.hCost = GetDistance(neighbor, targetNode);
//					foreach(NodeStep step in cameFromNodes)
//					{
//						if(step.toNode == neighbor) step.toNode = currentNode;
//					}

//					if (!openSet.Contains(neighbor))
//					{
//						openSet.Add(neighbor);
//						nodeStep = new NodeStep(neighbor, currentNode);
//						cameFromNodes.Add(nodeStep);
//						steps = BuildNodeStepPath(neighbor, startNode);
//						nodeStep.stepsString = GetStringFromSteps(steps);
//						nodeStep.directionString = GetDirectionsFromSteps(steps);
//						nodeStep.changeDirectionCount = CountDirectionFrom2Nodes(neighbor, startNode);
//						if (nodeStep.changeDirectionCount <= 3)
//							continue;
//					}
//				}
//			}
//		}

//		ShowPath();

//		return IsPathFound();
//	}

//	private bool IsValidPosition(Node node, Node startNode)
//	{
//		if (node == startNode) return true;

//		return !node.occupied;
//	}

//	protected virtual bool IsPathFound()
//	{
//		return finalPath.Count > 0;
//	}

//	protected virtual List<Node> BuildFinalPath(Node startNode, Node targetNode)
//	{
//		List<Node> path = new List<Node>();
//		Node toNode = targetNode;

//		while (toNode != startNode)
//		{
//			path.Add(toNode);
//			toNode = GetFromNode(toNode);
//		}

//		path.Add(startNode);
//		path.Reverse();

//		return path;
//	}

//	protected virtual Node GetFromNode(Node toNode)
//	{
//		return cameFromNodes.Find(item => item.toNode == toNode).fromNode;
//	}

//	protected virtual List<NodeStep> BuildNodeStepPath(Node currentNode, Node startNode)
//	{
//		List<NodeStep> steps = new List<NodeStep>();

//		Node checkNode = currentNode;
//		for (int i = 0; i < cameFromNodes.Count; i++)
//		{
//			NodeStep step = cameFromNodes.Find(item => item.toNode == checkNode);
//			steps.Add(step);
//			checkNode = step.fromNode;
//			if (step.fromNode == startNode) break;
//		}

//		return steps;
//	}

//	protected virtual void ShowPath()
//	{
//		lineRenderer.enabled = true;
//		List<Vector3> listPoint = new List<Vector3>();
//		foreach (Node node in finalPath)
//		{
//			listPoint.Add(node.nodeObj.transform.position);
//		}
//		lineRenderer.positionCount = listPoint.Count;
//		lineRenderer.SetPositions(listPoint.ToArray());
//	}

//	protected virtual int GetDistance(Node nodeA, Node nodeB)
//	{
//		int distX = Mathf.Abs(nodeA.x - nodeB.x);
//		int distY = Mathf.Abs(nodeA.y - nodeB.y);

//		if (distX > distY)
//			return 14 * distY + 10 * (distX - distY);
//		else
//			return 14 * distX + 10 * (distY - distX);
//	}

//	protected virtual string GetStringFromSteps(List<NodeStep> steps)
//	{
//		string stepsString = "";
//		foreach (NodeStep nodeStep in steps)
//		{
//			stepsString += nodeStep.toNode.Name() + "=>";
//		}
//		return stepsString;
//	}

//	protected virtual string GetDirectionsFromSteps(List<NodeStep> steps)
//	{
//		string stepsString = "";
//		foreach (NodeStep nodeStep in steps)
//		{
//			stepsString += nodeStep.direction + "=>";
//		}
//		return stepsString;
//	}

//	protected virtual int CountDirectionFrom2Nodes(Node currentNode, Node startNode)
//	{
//		int count = 0;
//		List<NodeStep> steps = BuildNodeStepPath(currentNode, startNode);
//		count = CountDirectionFromSteps(steps);
//		return count;
//	}

//	protected virtual int CountDirectionFromSteps(List<NodeStep> steps)
//	{
//		NodeDirections lastDirection = NodeDirections.noDirection;
//		NodeDirections currentDirection;
//		int turnCount = 0;
//		foreach (NodeStep nodeStep in steps)
//		{
//			currentDirection = nodeStep.direction;
//			if (currentDirection != lastDirection)
//			{
//				lastDirection = currentDirection;
//				turnCount++;
//			}
//		}

//		return turnCount;
//	}
//}
