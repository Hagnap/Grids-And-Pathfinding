using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Node : IComparable<Node>
{
	#region Variables
	private int data;

	private float stepCost;
	private float heuristicCost;
	private float totalCost;

	private Vector3 worldLocation;
	private Vector2 gridLocation;

	private Node parent;
	#endregion

	#region Properties
	public int Data
    {
		get { return this.data; }
		set { this.data = value; }
	}

	public Node Parent
	{
		get { return this.parent; }
		set { this.parent = value; }
	}

	public float HeuristicCost
	{
		get { return this.heuristicCost; }
		set { this.heuristicCost = value; }
	}

	public float StepCost
	{
		get { return this.stepCost; }
		set { this.stepCost = value; }
	}

	public float TotalCost
	{
		get { return this.totalCost; }
		set { this.totalCost = value; }
	}

	public Vector3 WorldLocation
    {
		get { return this.worldLocation; }
		set { this.worldLocation = value; }
    }

	public Vector2 GridLocation
    {
		get { return this.gridLocation; }
		set { this.gridLocation = value; }
    }

    #endregion

    #region Constructor
    public Node(int data, Vector3 worldLocation, Vector2 gridLocation, float stepCost, float heuristicCost)
    {
		this.data = data;
		this.worldLocation = worldLocation;
		this.gridLocation = gridLocation;
		this.stepCost = stepCost;
		this.heuristicCost = heuristicCost;
		this.totalCost = stepCost + heuristicCost;

		this.parent = null;
    }
	#endregion

	#region Overrides
	public int CompareTo(Node other)
	{
		if (this.totalCost < other.totalCost)
			return -1;

		else if (this.totalCost > other.totalCost)
			return 1;

		else
			return 0;
	}
	#endregion
}
