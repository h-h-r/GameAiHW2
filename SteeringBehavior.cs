using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the place to put all of the various steering behavior methods we're going
/// to be using. Probably best to put them all here, not in NPCController.
/// </summary>

public class SteeringBehavior : MonoBehaviour {

    // The agent at hand here, and whatever target it is dealing with
    public NPCController agent;
    public NPCController target;

    // Below are a bunch of variable declarations that will be used for the next few
    // assignments. Only a few of them are needed for the first assignment.

    // For pursue and evade functions
    public float maxPrediction;
    public float maxAcceleration;

    // For arrive function
    public float maxSpeed;
    public float targetRadiusL;
    public float slowRadiusL;
    public float timeToTarget;

    // For Face function
    public float maxRotation;
    public float maxAngularAcceleration;
    public float targetRadiusA;
    public float slowRadiusA;

    // For wander function
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    private float wanderOrientation;

    //hw2 haoran
    private float avoidDistance;
    private RaycastHit hitInfo;

    // Holds the path to follow
    public GameObject[] Path;
    public int current = 0;

    protected void Start() {
        agent = GetComponent<NPCController>();
        wanderOrientation = agent.orientation;
    }

    public Vector3 Seek() {
        Vector3 linear_acc = target.position - agent.position; //seek direction vector

        //clip to max linear acceleration
        if (linear_acc.magnitude > this.maxAcceleration){
            linear_acc = linear_acc.normalized * maxAcceleration;
        }

        //clip to max speed is handled in the UpdateMovement in NPCController.cs 
        //angular acceleration will be handled by face()  

        return linear_acc;  //returns the linear acc 
    }

    public Vector3 Flee()
    {
        return new Vector3(0f, 0f, 0f);
    }


    // Calculate the target to pursue
    public Vector3 Pursue() {
        return new Vector3(0f, 0f, 0f);
    }

    // Calculate the angular acceleration required to rotate to target
    public float Face()
    {

        Vector3 direction = target.position - agent.position;
        return FaceTo(direction);
    }

    public float FaceTo(Vector3 direction) { 
        // Check for a zero direction, and make no change if so
        if (direction.magnitude == 0)
        {
            return 0;
        }

        // Get anount of angle need to rotate
        float rotationAmount = Mathf.Atan2(direction.x, direction.z) - agent.orientation;
        //agent.orientaion range [-inf,inf]

        // clip to (-pi, pi) interval
        while (rotationAmount > Mathf.PI)
        {
            rotationAmount -= 2 * Mathf.PI;
        }
        while (rotationAmount < -Mathf.PI)
        {
            rotationAmount += 2 * Mathf.PI;
        }

        // if already facing target, set angular speed to zero
        if (Mathf.Abs(rotationAmount) < targetRadiusA)
        {
            agent.rotation = 0;
        }

        // greater than slowRadius => clip to max rotation speed
        // less than slowRadius => clip to scaled rotation speed 
        float rotationSpeed = (rotationAmount > slowRadiusA ? maxRotation : maxRotation * Mathf.Abs(rotationAmount) / slowRadiusA);

        // get the correct rotation direction
        rotationSpeed *= rotationAmount / Mathf.Abs(rotationAmount);

        // calculate the rotation acceleration
        float angular_acc = rotationSpeed - agent.rotation;
        angular_acc /= timeToTarget;

        // clip to max angular acc if needed
        if (Mathf.Abs(angular_acc) > maxAngularAcceleration)
        {
            angular_acc /= Mathf.Abs(angular_acc);
            angular_acc *= maxAngularAcceleration;
        }

        return angular_acc;
    }


    //what we have: raycast collation point and normal of surface that ray hits
    public bool PerformWhisker(out RaycastHit hitInfo)
    {
        //hhr
        Vector3 faceDir = new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        faceDir.Normalize();
        Vector3 leftDir = new Vector3(Mathf.Sin(agent.orientation - 0.3f), 0, Mathf.Cos(agent.orientation - 0.3f));
        leftDir.Normalize();
        Vector3 rightDir = new Vector3(Mathf.Sin(agent.orientation + 0.3f), 0, Mathf.Cos(agent.orientation + 0.3f));
        rightDir.Normalize();
        Vector3 raySource = agent.position;
        raySource = new Vector3(raySource.x, 0, raySource.z);
        agent.DrawWhiskers(agent.position + leftDir * wanderOffset, agent.position + rightDir * wanderOffset, raySource);
        //Debug.Log("orientation:"+agent.orientation);
        RaycastHit hitL;
        if (Physics.Raycast(raySource, leftDir, out hitL, wanderOffset))
        {
            //Debug.Log(hitL.collider.name);
            agent.label.text = "<==: " + hitL.collider.name;
            hitInfo = hitL;
            return true;
        }
        RaycastHit hitR;
        if (Physics.Raycast(raySource, rightDir, out hitR, wanderOffset))
        {
            //Debug.Log(hitR.collider.name);
            agent.label.text = hitR.collider.name + "==>";
            hitInfo = hitR;
            return true;
        }
        hitInfo = new RaycastHit();
        return false;
    }

    public (Vector3,float) SeekAndFaceToNewTarget(RaycastHit info)
    {
        //calculate position of avoidence target
        Vector3 newTarget = info.point + info.normal * wanderOffset * 4;
        Vector3 linear_acc = newTarget - agent.position; //seek direction vector

        //clip to max linear acceleration
        if (linear_acc.magnitude > this.maxAcceleration)
        {
            linear_acc = linear_acc.normalized * maxAcceleration;
        }

        float anglular_acc = FaceTo(newTarget - agent.position);
        return (linear_acc, anglular_acc);
        //face


    }

    //return linear and angular acc
    public (Vector3,float) WanderWithAvoidance()
    {
        if (PerformWhisker(out hitInfo))
        {
            return SeekAndFaceToNewTarget(hitInfo);
        }
        Vector3 linear = maxSpeed * new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        //angular = ai.Wander();
        return (linear,Wander());
    }

    // wander returns the angular_acc(account for face direction) 
    public float Wander()
    //    public float Wander(out Vector3 linear)
    {

        // adjust the initial wanderOrientation with a small random angle
        wanderOrientation += (Random.value - Random.value) * wanderRate;

        // Calculate the combined target orientation
        float orientation = wanderOrientation + agent.orientation;


        // the wander circle center position
        Vector3 position = agent.position + wanderOffset * new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        //agent.DrawConcentricCircle(wanderRadius);
        agent.DrawCircle(position, wanderRadius);

        

        // Calculate the wander target 
        position += wanderRadius * new Vector3(Mathf.Sin(orientation), 0, Mathf.Cos(orientation));

        // direction to wander target
        Vector3 direction = position - agent.position;

        // Get the naive direction to the target
        float rotation = Mathf.Atan2(direction.x, direction.z) - agent.orientation;

        //clip to [-pi,pi]
        while (rotation > Mathf.PI)
        {
            rotation -= 2 * Mathf.PI;
        }
        while (rotation < -Mathf.PI)
        {
            rotation += 2 * Mathf.PI;
        }
        float rotationSize = Mathf.Abs(rotation);

        // within targetRadius -> set roration speed to 0
        if (rotationSize < targetRadiusA)
        {
            agent.rotation = 0;
        }

        //calculate desire rotation speed
        float rotationSpeed = (rotationSize > slowRadiusA ? maxRotation : maxRotation * rotationSize / slowRadiusA);

        // apply direction
        rotationSpeed *= rotation / rotationSize;

        // Acceleration tries to get to the target rotation
        float angular_acc = rotationSpeed - agent.rotation;
        angular_acc /= timeToTarget;//angular acc

        // clip angular_acc
        if (Mathf.Abs(angular_acc) > maxAngularAcceleration)
        {
            angular_acc /= Mathf.Abs(angular_acc);
            angular_acc *= maxAngularAcceleration;
        }

        return angular_acc;

    }


    // ETC.

}
