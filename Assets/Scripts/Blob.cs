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
    public float wanderRadius;
    public float wanderTimer;
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float timer;
    
    #endregion

    #region Default Functions
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        speed = data.speed * speedMultiplier;
        agent.speed = speed;
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
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    #endregion

    #region Navigate
    private void blobWander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }
    #endregion

}
