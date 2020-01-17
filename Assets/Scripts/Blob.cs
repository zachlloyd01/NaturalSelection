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
        blobWander();
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
    #endregion

}
