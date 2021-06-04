using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchUtilities : MonoBehaviour
{
	# region Variables
	
	# endregion
	
	# region Properties
	
	# endregion
	
	# region Unity Methods
	// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	#endregion

	#region Search Methods
	public static List<Vector2> runGetNeighbors(Vector2 location, Grid grid)
	{
		List<Vector2> gridLocations = new List<Vector2>();

		Debug.Log("Current: " + grid.World[(int)location.x, (int)location.y].WorldLocation + " -- Data: " + grid.World[(int)location.x, (int)location.y].Data);
		//Debug.Log("Manager: " + manager);
		//Debug.Log("World: " + manager.World);

		// Up
		Vector2 up = location;
		up.y += 1;

		if (up.y < grid.Cols && grid.World[(int)up.x, (int)up.y].Data == 0)
		{
			//Debug.Log("Up: " + up);
			//Debug.Log("Manager: " + manager);
			gridLocations.Add(up);
		}
		if (up.y < grid.Cols && grid.World[(int)up.x, (int)up.y].Data == 1)
		{
			//Debug.Log("OBSTACLE ABOVE");
		}

		// Right
		Vector2 right = location;
		right.x += 1;

		if (right.x < grid.Rows && grid.World[(int)right.x, (int)right.y].Data == 0)
		{
			//Debug.Log("Right: " + right);
			//Debug.Log("Manager: " + manager);
			gridLocations.Add(right);
		}
		if (right.x < grid.Rows && grid.World[(int)right.x, (int)right.y].Data == 1)
		{
			//Debug.Log("OBSTACLE TO THE RIGHT");
		}

		// Down
		Vector2 down = location;
		down.y -= 1;
		//Debug.Log("*** " + manager.World[(int)down.x, (int)down.y].Data);

		if (down.y > -1 && grid.World[(int)down.x, (int)down.y].Data == 0)
		{
			//Debug.Log("Down: " + down);
			//Debug.Log("Manager: " + manager);
			gridLocations.Add(down);
		}
		if (down.y > -1 && grid.World[(int)down.x, (int)down.y].Data == 1)
		{
			//Debug.Log("OBSTACLE BELOW");
		}
		// Left
		Vector2 left = location;
		left.x -= 1;

		if (left.x > -1 && grid.World[(int)left.x, (int)left.y].Data == 0)
		{
			//Debug.Log("Left: " + left);
			//Debug.Log("Manager: " + manager);
			gridLocations.Add(left);
		}
		if (left.x > -1 && grid.World[(int)left.x, (int)left.y].Data == 1)
		{
			//Debug.Log("OBSTACLE TO THE LEFT");
		}

		return gridLocations;
	}

	public static LinkedList<Node> runExpandNode(Node node, ref LinkedList<Node> frontier, ref List<Node> visited, Grid grid)
	{
		// Gets the neighbors for the current location
		List<Vector2> neighbors = SearchUtilities.runGetNeighbors(node.GridLocation, grid);

		//Debug.Log("Current: [" + node.GridLocation.x + ", " + node.GridLocation.y + "]");
		//Debug.Log(neighbors.Count);

		// Traverse through the neighbors and create node objects for them to be stored for later use
		foreach (Vector2 n in neighbors)
		{
			//Debug.Log("Neighbor: [" + n.x + ", " + n.y + "]");

			if (!CheckList(frontier, n) && !CheckList(visited, n))
			{
				//Debug.Log(n + " → " + grid.World[(int)n.x, (int)n.y].WorldLocation);
				Node nd = new Node(0, grid.World[(int)n.x, (int)n.y].WorldLocation, n, 0f, 0f);
				nd.Parent = node;
				frontier.AddLast(nd);
			}
		}

		return frontier;
	}

	// type: True = A* and False = Greedy
	public static PriorityQueue<Node> runExpandNode(Node node, ref PriorityQueue<Node> frontier, ref List<Node> frontierCopy, ref List<Node> visited, Grid grid, Node goal, bool type)
	{
		// Gets the neighbors for the current location
		List<Vector2> neighbors = SearchUtilities.runGetNeighbors(node.GridLocation, grid);

		//Debug.Log("Current: [" + node.GridLocation.x + ", " + node.GridLocation.y + "]");
		//Debug.Log(neighbors.Count);

		// Traverse through the neighbors and create node objects for them to be stored for later use
		foreach (Vector2 n in neighbors)
		{
			//Debug.Log("Neighbor: [" + n.x + ", " + n.y + "]");

			
			if (type) // Is A*
			{
				if (!CheckList(frontierCopy, n) && !CheckList(visited, n))
				{
					// Step Cost = X + Y → (int)(n.x + n.y)
					Node nd = new Node(0, grid.World[(int)n.x, (int)n.y].WorldLocation, n, Random.Range(0f, 1f) * 10f,
						CalculateManhattanDistance(grid.World[(int)n.x, (int)n.y].WorldLocation, goal.WorldLocation));

					nd.Parent = node;
					frontier.Enqueue(nd);
					frontierCopy.Add(nd);
				}

			}
			else // Is Greedy
			{
				if (!CheckList(frontierCopy, n))
                {
					Node nd = new Node(0, grid.World[(int)n.x, (int)n.y].WorldLocation, n, 0f,
						CalculateManhattanDistance(grid.World[(int)n.x, (int)n.y].WorldLocation, goal.WorldLocation));

					nd.Parent = node;
					frontier.Enqueue(nd);
					frontierCopy.Add(nd);
				}
			}
		}

		return frontier;
	}

	public static List<Node> runSetPath(Node current, List<Node> path)
	{
		
		// Keeps iterating until the parent is null (at the start)
		while (current.Parent != null)
		{
			path.Add(current);
			current = current.Parent;
		}

		//path.Reverse();

		string str = "";

		for (int i = 0; i < path.Count; i++)
		{
			if (i != path.Count - 1)
			{
				str += "[" + path[i].GridLocation.x + ", " + path[i].GridLocation.y + "] → ";
			}
			else
			{
				str += "[" + path[i].GridLocation.x + ", " + path[i].GridLocation.y + "]";
			}
		}

		Debug.Log("Path size & Path: " + path.Count + str);

		str = "";

		for (int i = 0; i < path.Count; i++)
		{
			if (i != path.Count - 1)
			{
				str += "[" + path[i].WorldLocation.x + ", " + path[i].WorldLocation.y + ", " + path[i].WorldLocation.z + "] → ";
			}
			else
			{
				str += "[" + path[i].WorldLocation.x + ", " + path[i].WorldLocation.y + ", " + path[i].WorldLocation.z + "]";
			}
		}

		Debug.Log("Path size & Path: " + path.Count + str);

		return path;
	}

	public static bool CheckList(LinkedList<Node> nodeList, Vector2 location)
	{
		bool result = false;

		foreach (Node node in nodeList)
		{
			if (node.GridLocation == location)
			{
				result = true;
				return result;
			}
		}

		return result;
	}

	public static bool CheckList(List<Node> nodeList, Vector2 location)
	{
		bool result = false;

		foreach (Node node in nodeList)
		{
			if (node.GridLocation == location)
			{
				result = true;
				return result;
			}
		}

		return result;
	}

	public static float CalculateManhattanDistance(Vector3 current, Vector3 goal)
	{
		float x = Mathf.Abs(goal.x - current.x);
		float z = Mathf.Abs(goal.z - current.z);

		return (x + z);
	}
	#endregion
}
