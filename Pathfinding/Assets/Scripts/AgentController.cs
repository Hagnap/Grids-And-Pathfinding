using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
	#region Variables
	[SerializeField] float timeToTarget;
	[SerializeField] float speed;
	[SerializeField] float turnSpeed;
	[SerializeField] float radiusOfSatisfaction;
	[SerializeField] Grid grid;

	private Rigidbody rb;
	private bool isMoving;
	private bool isPathGenerated;
	Quaternion rotation;
	private Vector3 direction;
	private float distance;
	private int index;
	#endregion

	#region Properties
	public bool IsMoving
    {
		get { return this.isMoving; }
		set { this.isMoving = value; }
    }
	public bool IsPathGenerated
	{
		get { return this.isPathGenerated; }
		set { this.isPathGenerated = value; }
    }
	# endregion
	
	# region Unity Methods
	// Start is called before the first frame update
    void Start()
    {
		index = 0;
		isMoving = false;
		isPathGenerated = false;

		rb = this.GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
        if(isMoving && isPathGenerated)
		{
			//Debug.Log("Going to " + grid.Path[index].WorldLocation);

			runKinematicArrive(grid.Path[index].WorldLocation);

			if (index >= grid.Path.Count)
			{
				index = 0;
				isMoving = !isMoving;
			}
		}
    }
	#endregion

	#region Custom Methods
	public void runKinematicArrive(Vector3 target)
	{
		// Gets the direction to the target
		direction = target - this.transform.position;

		distance = direction.magnitude;
		// Check if the agent is 'close enough' to the target
		if (distance > this.GetComponent<AgentController>().radiusOfSatisfaction)
		{
			// Makes the agent get to the target in `timeToTarget` seconds
			direction /= timeToTarget;

			// Face the direction we want the agent to move towards
			rotation = Quaternion.LookRotation(direction);
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, turnSpeed * Time.deltaTime);

			// Normalize vector to get just the direction and then set the speed
			direction.Normalize();
			direction *= speed;

			// Sets the agent's velocity
			rb.velocity = direction;
		}
		else
		{
			// Stops the agent if close enough
			// Sets the agent's velocity
			index++;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			return;
		}
	}
	#endregion

	#region IEnumerators

	#endregion
}
