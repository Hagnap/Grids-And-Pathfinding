using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	#region Variables
	[SerializeField] private int layerMask_Ground;
	[SerializeField] private int layerMask_Obstacle;
	[SerializeField] private int layerMask_Agent;

	[SerializeField] private bool updateGrid;
	private bool isPathFound;

	[SerializeField] private SearchAlgorithms search;
	[SerializeField] private AgentController agentController;

	private Node[,] world;
	private List<Node> path;

	private int nodeSize;

	private int terrainWidth;
	private int terrainLength;

	private int rows;
	private int cols;
	private int ground;
	private int obstacle;
	private int agent;
	#endregion

	#region Properties
	public bool IsPathFound
	{
		get { return this.isPathFound; }
		set { this.isPathFound = value; }
	}

	public List<Node> Path
    {
		get { return this.path; }
		set { this.path = value; }
    }
	public Node[,] World
    {
        get { return this.world; }
        set { this.world = value; }
    }

	public int Rows
    {
		get { return this.rows; }
    }

	public int Cols
    {
		get { return this.cols; }
    }
	#endregion

	#region Unity Methods
	// Start is called before the first frame update
	void Start()
    {
		updateGrid = false;

		terrainWidth = 100;
		terrainLength = 100;

		ground  = 1 << layerMask_Ground;
		obstacle = 1 << layerMask_Obstacle;
		agent = 1 << layerMask_Agent;

		nodeSize = 5;

		rows = terrainWidth / nodeSize;
		cols = terrainLength / nodeSize;

		world = new Node[rows, cols];

		CreateGrid();

		search.StartNode = new Node(0, world[(int)search.StartPosition.x, (int)search.StartPosition.y].WorldLocation, search.StartPosition, 0f, 0f);
		search.GoalNode = new Node(0, world[(int)search.GoalPosition.x, (int)search.GoalPosition.y].WorldLocation, search.GoalPosition, 0f, 0f);
	}

    // Update is called once per frame
    void Update()
    {
		if(updateGrid)
        {
			if(!isPathFound)
            {
				UpdateGrid();
				updateGrid = false;
			}

			else
            {
				UpdateGridWithPath(path);
				agentController.IsPathGenerated = true;
				updateGrid = false;
			}
		}
	}
	#endregion

	#region Grid Methods
	public void CreateGrid()
	{

		float startX = 0;
		float startZ = 0;

		float nodeCenterOffset = nodeSize / 2f;

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				float x = startX + nodeCenterOffset + (nodeSize * col);
				float z = startZ + nodeCenterOffset + (nodeSize * row);

				Vector3 worldLocation = new Vector3(x, 20f, z);
				Vector2 gridLocation = new Vector2(row, col);

				// Does our raycast hit anything at this point in the map
				RaycastHit hit;

				//	HITS AN OBSTACLE
				if (Physics.Raycast(worldLocation, Vector3.down, out hit, Mathf.Infinity, obstacle)) 
				{				
					Debug.DrawRay(worldLocation, Vector3.down * 20, Color.red, 50000);
					
					worldLocation.y = 1f; // Update y so nodes have a height of 1
					//Debug.Log(worldLocation + " → " + gridLocation);
					Debug.Log("Hit something at row: " + row + " col: " + col + " --- World Position: " + worldLocation);

					world[row, col] = new Node(1, worldLocation, gridLocation, 0f, 0f);
				}

				//	DOESN'T HIT AN OBSTACLE
				else if (Physics.Raycast(worldLocation, Vector3.down, out hit, Mathf.Infinity, ground))
                {
					Debug.DrawRay(worldLocation, Vector3.down * 20, Color.green, 50000);

					worldLocation.y = 1f; // Update y so nodes have a height of 1
					//Debug.Log(worldLocation + " → " + gridLocation);

					world[row, col] = new Node(0, worldLocation, gridLocation, 0f, 0f);
				}
			}
		}
	}

	public void UpdateGrid()
	{
		float startX = 0;
		float startZ = 0;

		float nodeCenterOffset = nodeSize / 2f;

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				float x = startX + nodeCenterOffset + (nodeSize * col);
				float z = startZ + nodeCenterOffset + (nodeSize * row);

				Vector3 worldLocation = new Vector3(x, 20f, z);
				Vector2 gridLocation = new Vector2(row, col);

				//Debug.Log(worldLocation + " → " + gridLocation);

				// Does our raycast hit anything at this point in the map
				RaycastHit hit;

				//	HITS AN OBSTACLE
				if (Physics.Raycast(worldLocation, Vector3.down, out hit, Mathf.Infinity, obstacle))
				{

					print("Hit something at row: " + row + " col: " + col);
					Debug.DrawRay(worldLocation, Vector3.down * 20, Color.red, 50000);
					worldLocation.y = 1f; // Update y so nodes have a height of 1
					world[row, col].Data = 1;
					Debug.Log("Row: " + row + ", Col: " + col + " → " + world[row, col].Data);
				}

				//	HITS THE GROUND
				else if (Physics.Raycast(worldLocation, Vector3.down, out hit, Mathf.Infinity, ground))
				{
					Debug.DrawRay(worldLocation, Vector3.down * 20, Color.green, 50000);
					worldLocation.y = 1f; // Update y so nodes have a height of 1
					world[row, col].Data = 0;
					Debug.Log("Row: " + row + ", Col: " + col + " → " + world[row, col].Data);
				}
			}
		}
	}

	public void UpdateGridWithPath(List<Node> path)
    {
		float startX = 0;
		float startZ = 0;

		float nodeCenterOffset = nodeSize / 2f;

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				float x = startX + nodeCenterOffset + (nodeSize * col);
				float z = startZ + nodeCenterOffset + (nodeSize * row);

				Vector3 worldLocation = new Vector3(x, 20f, z);
				Vector2 gridLocation = new Vector2(row, col);

				//Debug.Log(worldLocation + " → " + gridLocation);

				// Does our raycast hit anything at this point in the map
				RaycastHit hit;

				//	HITS AN OBSTACLE
				if (Physics.Raycast(worldLocation, Vector3.down, out hit, Mathf.Infinity, obstacle))
				{

					//Debug.Log("Hit something at row: " + row + " col: " + col);
					Debug.DrawRay(worldLocation, Vector3.down * 20, Color.red, 50000);
					worldLocation.y = 1f; // Update y so nodes have a height of 1
					world[row, col].Data = 1;
					//Debug.Log("Row: " + row + ", Col: " + col + " → " + world[row, col].Data);
				}

				//	HITS THE GROUND
				else
                {
					if (CheckList(path, new Vector2(row, col)))
					{
						Debug.DrawRay(worldLocation, Vector3.down * 20, Color.blue, 50000);
						//worldData[row, col] = 0;
						world[row, col].Data = 0;
						//Debug.Log("Row: " + row + ", Col: " + col + " → " + world[row, col].Data);
					}
					else
                    {
						Debug.DrawRay(worldLocation, Vector3.down * 20, Color.green, 50000);
						//worldData[row, col] = 0;
						world[row, col].Data = 0;
						//Debug.Log("Row: " + row + ", Col: " + col + " → " + world[row, col].Data);
					}
				}
			}
		}
	}

	public bool CheckList(List<Node> nodeList, Vector2 location)
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
	#endregion
}
