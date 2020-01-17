using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Blob : MonoBehaviour
{

    #region variables

    [Header("Creature Specific")]
    public blobData data; //Get attributes from here

    [Header("Scripted Values")]
    public float speed; //How fast..... duh

    [Header("Global Attributes")]
    [SerializeField] private float speedMultiplier = 10; //Default value to multiply the randomized speed value by
    public float energy; //How much energy the creature has
    
    [Header("Materials")]
    public Material defaultMaterial; //Default boi
    public Material slowMaterial; //Slow boi
    public Material fastMaterial; //Speedy boi

    [Header("Navigation Variables")]
    public NavMeshAgent agent; //Walky boi
    public float maxWalkDistance; //Distance from center the blob can go
    public GameObject ground; //Floory boi

    [Header("Sight Variables")]
    public float fieldOfViewAngle = 110f; //Radius the enemy can see
    public bool foodInSight; //Can it see any food?
    public Vector3 foodPosition;
    #endregion

    #region Default Functions
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); //Relative object instantiation
        ground = GameObject.Find("Ground"); //Set the ground for the random movements
        maxWalkDistance = 20f; //How far can it go? Well in this case.... That
        setMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        GameObject food = FindClosestFood();
        foodInSight = checkInSight(food);
        if(!foodInSight)
        {
            blobWander();
        }
        else
        {
            goToFood(food);
        }
        
    }

    #endregion

    #region Value-Setting Functions
    private void setMaterial()
    {
        //Check speed and set the color of the blob accordingly
        if (data.speed > 5)
        {
            gameObject.GetComponent<MeshRenderer>().material = fastMaterial;
        }
        else if (data.speed > 1)
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        else if (data.speed < 1 && data.speed > 0)
        {
            gameObject.GetComponent<MeshRenderer>().material = slowMaterial;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Navigation Values

    bool RandomPoint(Vector3 center, float range, out Vector3 result) //A boolean that takes in a center and radius, outputs weather a Vector is on the NavMesh and a Vector to traverse to that is on the mesh
    {
        for (int i = 0; i < 30; i++) //Run 30 times
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range; //Random point in a sphere with height/radius of 1
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //If the generated point is on the navmesh
            {
                result = hit.position; //output the resultant point
                return true; //Say that it is on the mesh
            }
        }
        result = Vector3.zero; //Else travel to 0,0
        return false; //The pt was not on the navmesh, will run again next frame
    }

    public GameObject FindClosestFood()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Food");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public bool checkInSight(GameObject food)
    {
        Vector3 direction = food.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if(angle < fieldOfViewAngle)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, 6))
            {
                if(hit.collider.gameObject == food)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Navigate

    private void blobWander()
    {
        Vector3 point; //An unset Vector to be used to go to
        if (RandomPoint(ground.transform.position, maxWalkDistance, out point)) //Outputs the point, takes in the ground and radius values
        {
            agent.SetDestination(point); //Make the agent go to the point if its on the navmesh
        }
    }

    private void goToFood(GameObject food)
    {
        agent.SetDestination(food.transform.position);
    }

    #endregion


}
