using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

public class NPCController : MonoBehaviour {
    // Store variables for objects
    private SteeringBehavior ai;    // Put all the brains for steering in its own module
    private Rigidbody rb;           // You'll need this for dynamic steering

    // For speed 
    public Vector3 position;        // local pointer to the RigidBody's Location vector
    public Vector3 velocity;        // Will be needed for dynamic steering

    // For rotation
    public float orientation;       // scalar float for agent's current orientation
    public float rotation;          // Will be needed for dynamic steering

    public float maxSpeed;          // what it says

    public int phase;               // use this to control which "phase" the demo is in

    private Vector3 linear;         // The resilts of the kinematic steering requested
    private float angular;          // The resilts of the kinematic steering requested

    public Text label;              // Used to displaying text nearby the agent as it moves around
    LineRenderer line;              // Used to draw circles and other things
    LineRenderer ray;

    private void Start() {
        ai = GetComponent<SteeringBehavior>();
        rb = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
        ray = GetComponent<LineRenderer>();
        position = rb.position;
        orientation = transform.eulerAngles.y;
        //if (ai.tag == "Hunter")
        //{
        //    orientation = transform.eulerAngles.y-300;
        //}
        //else
        //{
        //    orientation = transform.eulerAngles.y + 300;
        //}

    }

  

    /// <summary>
    /// Depending on the phase the demo is in, have the agent do the appropriate steering.
    /// 
    /// </summary>
    void FixedUpdate() {
        switch (phase) {
            case 0:
                if (label)
                {
                    label.text = tag;
                }
                break;

            case 1:
                if (label) {
                    label.text = ""; 
                }
                if (ai.tag == "Wolf")
                {
                    label.text = "Wander";
                    (linear, angular) = Algo("Wander");
                }
                break;

            case 2:
                if (label) {
                    label.text = "";
                    if (ai.tag == "Hunter")
                    {    
                      
                    }
                    if (ai.tag == "Wolf")
                    {
                        label.text = "Chase Player";
                        (linear, angular) = Algo("ChasePlayer");
                    }
                }

                // linear = ai.whatever();  -- replace with the desired calls
                // angular = ai.whatever();
                break;
            case 3:
                if (label)
                {
                    label.text = "";
                    if (ai.tag == "Hunter")
                    {
                        label.text = "Dynamic pursue";
                        (linear, angular) = Algo("Pursue");
                    }
                    if (ai.tag == "Wolf")
                    {
                        label.text = "Dynamic Evade";
                        (linear, angular) = Algo("Evade");

                    }
                    //linear = ai.Seek();

                    //(linear,angular) = ai.CollisionPrediction();

                    //angular = ai.FaceAway();
                }

                // linear = ai.whatever();  -- replace with the desired calls
                // angular = ai.whatever();
                break;
            case 4:
                if (label) {
                    label.text = "";
                }
                Vector3 collisionPoint;
                if (ai.CollisionPrediction(out collisionPoint) == true)
                {
                    DrawCircle(collisionPoint, 0.1f);
                    linear = ai.Evade(collisionPoint);
                    label.text = "Evade from the pridicted circle!";
                    angular = ai.FaceAway();
                }
                else
                {
                    (linear, angular) = Algo("ChasePlayer");
                }

                

                // linear = ai.whatever();  -- replace with the desired calls
                // angular = ai.whatever();
                break;
            case 5:
                if (label) {
                    label.text = name.Replace("(Clone)", "") + "\nAlgorithm: Fifth algorithm";
                }

                // linear = ai.whatever();  -- replace with the desired calls
                // angular = ai.whatever();
                break;
        }
        update(linear, angular, Time.deltaTime);
        if (label) {
            label.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
        }
    }

    private void update(Vector3 steeringlin, float steeringang, float time) {
        // Update the orientation, velocity and rotation
        orientation += rotation * time;
        velocity += steeringlin * time;
        rotation += steeringang * time;

        if (velocity.magnitude > maxSpeed) {
            velocity.Normalize();
            velocity *= maxSpeed;
        }

        rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
        position = rb.position;
        rb.MoveRotation(Quaternion.Euler(new Vector3(0, Mathf.Rad2Deg * orientation, 0)));
    }

    // <summary>
    // The next two methods are used to draw circles in various places as part of demoing the
    // algorithms.

    /// <summary>
    /// Draws a circle with passed-in radius around the center point of the NPC itself.
    /// </summary>
    /// <param name="radius">Desired radius of the concentric circle</param>
    public void DrawConcentricCircle(float radius) {
        line.positionCount = 51;
        line.useWorldSpace = true;
        float x;
        float z;
        float angle = 20f;

        for (int i = 0; i < 51; i++) {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / 51);
        }
    }

    /// <summary>
    /// Draws a circle with passed-in radius and arbitrary position relative to center of
    /// the NPC.
    /// </summary>
    /// <param name="position">position relative to the center point of the NPC</param>
    /// <param name="radius">>Desired radius of the circle</param>
    public void DrawCircle(Vector3 position, float radius) {
        this.DestroyPoints();
        line.positionCount = 21;
        line.useWorldSpace = true;
        float x;
        float z;
        float angle = 20f;

        for (int i = 0; i < 21; i++) {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z)+position);
            angle += (360f / 21);
        }
    }

    public void DestroyPoints() {
        if (line) {
            line.positionCount = 0;
        }
    }

    //hhr
    public void Draw1Whiskers(Vector3 start, Vector3 end)
    {
        line.positionCount = 2;
        line.useWorldSpace = true;

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    
    }

    public void Draw2Whiskers(Vector3 left, Vector3 right, Vector3 orig)
    {
        line.positionCount = 5;
        line.useWorldSpace = true;

        line.SetPosition(0, orig);
        line.SetPosition(1, left);
        line.SetPosition(2, orig);
        line.SetPosition(3, right);
        line.SetPosition(4, orig);

    }
    public void Draw3Whiskers(Vector3 left, Vector3 mid, Vector3 right, Vector3 orig)
    {
        line.positionCount = 7;
        line.useWorldSpace = true;

        line.SetPosition(0, orig);
        line.SetPosition(1, left);
        line.SetPosition(2, orig);
        line.SetPosition(3, mid);
        line.SetPosition(4, orig);
        line.SetPosition(5, right);
        line.SetPosition(6, orig);

    }

    public (Vector3,float) Algo(string algo)
    {
        float angular = 0f;
        //default linear acc
        Vector3 linear = ai.maxAcceleration * new Vector3(Mathf.Sin(ai.GetComponent<NPCController>().orientation), 0, Mathf.Cos(ai.GetComponent<NPCController>().orientation));

        RaycastHit hitInfo;
        //if (ai.PerformWhisker(out hitInfo) == true && hitInfo.collider.tag != "Player")
        //if ((ai.PerformWhiskerAlongVelocity(out hitInfo) == true) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != ai.tag)

        if (( ai.PerformWhisker(out hitInfo) == true) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != ai.tag)
        {
            //seek new target based on hitinfo
            if (hitInfo.collider.name == "Tree(Clone)")
            {
                ai.avoidTree = true;
                ai.treePosition = hitInfo.point;
                linear = ai.Flee(ai.treePosition);
                angular = ai.FaceAway(position - ai.treePosition);
                label.text = "Tree Avoidence";
            }
            else //ray hit wall 
            {
                (linear, angular) = ai.SeekAndFaceToNewTarget(hitInfo);
                label.text = "Wall Avoidence";
            }
        }
        else if ((ai.PerformWhiskerAlongVelocity(out hitInfo) == true ) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != ai.tag)
        {
            //seek new target based on hitinfo
            if (hitInfo.collider.name == "Tree(Clone)")
            {
                ai.avoidTree = true;
                ai.treePosition = hitInfo.point;
                linear = ai.Flee(ai.treePosition);
                angular = ai.FaceAway(position - ai.treePosition);
                label.text = "Tree Avoidence";
            }
            else //ray hit wall 
            {
                (linear, angular) = ai.SeekAndFaceToNewTarget(hitInfo);
                label.text = "Wall Avoidence";
            }
        }
        else
        {
            if (ai.avoidTree == true)
            {
                if ((position - ai.treePosition).magnitude < ai.treeEscapeRadius)
                {
                    linear = ai.Flee(ai.treePosition);
                    angular = 0;
                    angular = ai.FaceAway(position - ai.treePosition);
                    label.text = "Tree Avoidence";
                }
                else
                {
                    ai.avoidTree = false;
                }

            }
            else
            {
                if (algo == "Pursue")
                {
                    linear = ai.Pursue();
                    angular = ai.Face();
                    
                }else if (algo == "Evade")
                {
                    linear = ai.Evade();
                    angular = ai.FaceAway();
                }else if (algo == "ChasePlayer")
                {
                    linear = ai.ChasePlayer();
                    angular = ai.FaceToPlayer();
                }else if (algo == "Wander")
                {
                    angular = ai.Wander();
                }
                
            }
        }
        return (linear,angular);
    }

}
