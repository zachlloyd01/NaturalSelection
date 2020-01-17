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
    public NavMeshAgent agent;
    public float maxWalkDistance;
    public GameObject ground;
    #endregion

    #region Default Functions
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ground = GameObject.Find("Ground");
        maxWalkDistance = 20f;
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
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    #endregion

    #region Navigate
    private void blobWander()
    {
        Vector3 point;
        if (RandomPoint(ground.transform.position, maxWalkDistance, out point))
        {
            agent.SetDestination(point);
        }
    }
    #endregion

}
