using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchAlgorithms : MonoBehaviour
{
	#region Variables
	[SerializeField] private bool isSearching = false;
	[SerializeField] private bool isBFS = false;
	[SerializeField] private bool isDFS = false;
	[SerializeField] private bool isGreedy = false;
	[SerializeField] private bool isAStar = false;

	[SerializeField] private Vector2 startPosition;
	[SerializeField] private Vector2 goalPosition;

	[SerializeField] private Grid grid;
	[SerializeField] private AgentController agent;

	private List<Node> visited;
	private List<Node> path;
	Node start;
	Node goal;
	#endregion

	#region Properties
	public Vector3 StartPosition
    {
		get { return this.startPosition; }
	}
	public Vector3 GoalPosition
	{
		get { return this.goalPosition; }
	}

	public Node StartNode
    {
		get { return this.start; }
		set { this.start = value; }
    }
	public Node GoalNode
    {
		get { return this.goal; }
		set { this.goal = value; }
    }
	#endregion

	#region Unity Methods
	// Start is called before the first frame update
	void Start()
    {
		path = new List<Node>();
		visited = new List<Node>();
	}

    // Update is called once per frame
    void Update()
    {
		if (isSearching)
        {
			if (isDFS)
				path = runDepthFirstSearch(grid, start, goal);

			if (isBFS)
				path = runBreadthFirstSearch(grid, start, goal);

			if (isGreedy)
				path = runGreedySearch(grid, start, goal);

			if (isAStar)
				path = runAStarSearch(grid, start, goal);


			grid.Path = path;
			grid.IsPathFound = true;
			agent.IsMoving = true;
		}
	}
	#endregion

	#region Uninformed Search Methods
	public List<Node> runBreadthFirstSearch(Grid grid, Node start, Node goal)
	{
		//Debug.Log("Search starting...");
		Node current = start;

		path.Clear();
		visited.Clear();
		//openList.Clear();

		// Acts as a queue
		LinkedList<Node> frontier = new LinkedList<Node>();
		frontier.AddFirst(current);

		while (frontier.Count > 0)
		{
			// Gets the node from the stack & adds it to the closed list
			current = frontier.First();
			frontier.RemoveFirst();
			visited.Add(current);

			//Debug.Log("Current: " + current.GraphLocation);

			// Goal check
			if (current.GridLocation == goal.GridLocation /*|| current.WorldLocation == goal.WorldLocation*/)
			{
				//Debug.Log("At the goal!");
				break;
			}
			else
			{
				//Debug.Log("Not at the goal.. Expanding the current node");

				// Expand the current node
				frontier = SearchUtilities.runExpandNode(current, ref frontier, ref visited, grid);
			}
		}

		if (frontier.Count == 0 || current.GridLocation == goal.GridLocation /*|| current.WorldLocation == goal.WorldLocation*/)
		{
			Debug.Log("Path is being made...");

			path = SearchUtilities.runSetPath(current, path);
			//path.Add(goal);
			//path.Add(start);
			path.Reverse();

			Debug.Log("Path has been made...");
		}

		//Debug.Log("Search ending...");

		this.isSearching = false;
		return path;
	}

	public List<Node> runDepthFirstSearch(Grid grid, Node start, Node goal)
	{
		//Debug.Log("Search starting...");
		Node current = start;

		path.Clear();
		visited.Clear();
		//openList.Clear();

		// Acts as a queue
		LinkedList<Node> frontier = new LinkedList<Node>();
		frontier.AddFirst(current);

		while (frontier.Count > 0)
		{
			// Gets the node from the stack & adds it to the closed list
			current = frontier.Last();
			frontier.RemoveLast();
			visited.Add(current);

			//Debug.Log("Current: " + current.GraphLocation);

			// Goal check
			if (current.GridLocation == goal.GridLocation /*|| current.WorldLocation == goal.WorldLocation*/)
			{
				//Debug.Log("At the goal!");
				break;
			}
			else
			{
				//Debug.Log("Not at the goal.. Expanding the current node");

				// Expand the current node
				frontier = SearchUtilities.runExpandNode(current, ref frontier, ref visited, grid);
			}
		}

		if (frontier.Count == 0 || current.GridLocation == goal.GridLocation /*|| current.WorldLocation == goal.WorldLocation*/)
		{
			Debug.Log("Path is being made...");

			path = SearchUtilities.runSetPath(current, path);
			//path.Add(goal);
			//path.Add(start);
			path.Reverse();

			Debug.Log("Path has been made...");
		}

		//Debug.Log("Search ending...");

		this.isSearching = false;
		return path;
	}
	#endregion

	#region Informed Searches
	public List<Node> runGreedySearch(Grid grid, Node start, Node goal)
	{
		//Debug.Log("Search starting...");
		Node current = start;

		path.Clear();
		visited.Clear();

		PriorityQueue<Node> frontier = new PriorityQueue<Node>();
		List<Node> frontierCopy = new List<Node>();

		frontier.Enqueue(current);
		frontierCopy.Add(current);


		while (frontier.Count > 0)
		{
			// Goal check
			if (current.GridLocation == goal.GridLocation)
			{
				//Debug.Log("At the goal!");
				break;
			}
			else
			{
				current = frontier.Dequeue();
				frontierCopy.Remove(current);
				visited.Add(current);

				frontier = SearchUtilities.runExpandNode(current, ref frontier, ref frontierCopy, ref visited, grid, goal, false);
			}
		}

		if (frontier.Count == 0 || current.GridLocation == goal.GridLocation)
		{
			Debug.Log("Path is being made...");

			path = SearchUtilities.runSetPath(current, path);
			//path.Add(goal);
			//path.Add(start);
			path.Reverse();

			Debug.Log("Path has been made...");
		}

		//Debug.Log("Search ending...");

		this.isSearching = false;
		return path;
	}

	public List<Node> runAStarSearch(Grid grid, Node start, Node goal)
	{
		//Debug.Log("Search starting...");
		Node current = start;

		path.Clear();
		visited.Clear();

		PriorityQueue<Node> frontier = new PriorityQueue<Node>();
		List<Node> frontierCopy = new List<Node>();

		frontier.Enqueue(current);
		frontierCopy.Add(current);


		while (frontier.Count > 0)
		{
			// Goal check
			if (current.GridLocation == goal.GridLocation)
			{
				//Debug.Log("At the goal!");
				break;
			}
			else
			{
				current = frontier.Dequeue();
				frontierCopy.Remove(current);
				visited.Add(current);

				frontier = SearchUtilities.runExpandNode(current, ref frontier, ref frontierCopy, ref visited, grid, goal, true);
			}
		}

		if (frontier.Count == 0 || current.GridLocation == goal.GridLocation)
		{
			Debug.Log("Path is being made...");

			path = SearchUtilities.runSetPath(current, path);
			//path.Add(goal);
			//path.Add(start);
			path.Reverse();

			Debug.Log("Path has been made...");
		}

		//Debug.Log("Search ending...");

		this.isSearching = false;
		return path;
	}
	#endregion
}
