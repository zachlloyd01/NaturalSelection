﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Blob : MonoBehaviour
{

    #region Variables

    private bool goingToFood;
    private bool eaten;
    private bool goingToEdge;
    private GameObject sceneManager;
    public bool run;

    [Header("Creature Specific")]
    public blobData data; //Get attributes from here

    [Header("Scripted Values")]
    public float speed; //How fast..... duh
    public bool onEdge;

    [Header("Global Attributes")]
    [SerializeField] private float speedMultiplier = 10; //Default value to multiply the randomized speed value by
    public float energy; //How much energy the creature has
    public GameObject blobPrefab;
    public GameObject spawnPosition;
    
    [Header("Materials")]
    public Material defaultMaterial; //Default boi
    public Material slowMaterial; //Slow boi
    public Material fastMaterial; //Speedy boi

    [Header("Navigation Variables")]
    public NavMeshAgent agent; //Walky boi
    public float maxWalkDistance; //Distance from center the blob can go
    public GameObject ground; //Floory boi
    public bool atFood;

    [Header("Sight Variables")]
    public float fieldOfViewAngle; //Radius the enemy can see
    public bool foodInSight; //Can it see any food?
    public Vector3 foodPosition;
    public float maxRange = 19;

    [Header("Values")]
    public Dictionary<string, float> genes = new Dictionary<string, float>(); //Gene values by string-key

    #endregion

    #region Default Functions
    // Start is called before the first frame update
    void Start()
    {
        #region Dictionary Values

        onEdge = false;
        genes.Add("speed", data.speed);
        genes.Add("aggression", data.aggression);
        genes.Add("sight", data.sight);

        #endregion

        #region Initial Value Setting

        eaten = false;
        goingToFood = false;
        atFood = false;
        run = false;
        goingToEdge = false;

        agent = GetComponent<NavMeshAgent>(); //Relative object instantiation
        sceneManager = GameObject.FindGameObjectWithTag("GameController");

        agent.speed = genes["speed"];
        fieldOfViewAngle = genes["sight"];

        ground = GameObject.Find("Ground"); //Set the ground for the random movements

        

        #endregion

        #region Function Calls
        setMaterial();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
       if(goingToEdge == true)
        {
            if(gameObject.transform.position == agent.destination)
            {
                onEdge = true;
                energy = 100;
                run = false;
                goingToEdge = false;
            }
            else
            {
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        if (run)
        {
            energyLower();
            GameObject food = FindClosestFood(); //Get the array of all food items and check the closest one by distance
            movement(food);
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

    public void energyLower()
    {
        if (run)
        {
            energy -= (10 / genes["speed"]) * Time.fixedDeltaTime; //Energy lowers as a factor of speed over fixed time
        }
    }
    #endregion

    #region Navigation Values

    Vector3 RandomPoint(float range) //A boolean that takes in a center and radius, outputs weather a Vector is on the NavMesh and a Vector to traverse to that is on the mesh
    {
        Vector3 randPosition = UnityEngine.Random.insideUnitSphere * maxWalkDistance;
        randPosition += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randPosition, out hit, range, 1);
        Vector3 result = hit.position;
        return result;
    }

    public GameObject FindClosestFood()
    {
        GameObject[] gos; //The array of cookeis on the map
        gos = GameObject.FindGameObjectsWithTag("Food"); //Set the array to be all objects with the food tag (added on spawn)
        GameObject closest = null; //The nearest piece of food
        float distance = Mathf.Infinity; //The distance is infinite until one food is defined
        Vector3 position = transform.position; //Current Blob position on the map (This is run during fixed update... could be delayed by a frame or 2)
        foreach (GameObject go in gos) //Every obj in the array
        {
            Vector3 diff = go.transform.position - position; //The Vector to position onto
            float curDistance = diff.sqrMagnitude; //The distance as given by the vector
            if (curDistance < distance) //If the distance is not larger than previous (assuming there is food on the map)
            {
                closest = go; //Current closest GO
                distance = curDistance; //reset distance var (so it gets pro
            }
        }
        return closest;
    }

    public bool checkInSight(GameObject food)
    {
        
        RaycastHit hit;

        if (Vector3.Distance(transform.position, food.transform.position) < maxRange)
        {
            if (Physics.Raycast(transform.position, (food.transform.position - transform.position), out hit, maxRange))
            {
               // Debug.Log(hit.transform);
                if (hit.collider.gameObject == food)
                {
                   // Debug.Log("true");
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
        agent.SetDestination(RandomPoint(maxWalkDistance));
    }

    private void goToFood(GameObject food)
    {
        agent.SetDestination(food.transform.position);

        
    }

    private void movement(GameObject food)
    {
        if (run)
        {
            if (!atFood) //If the agent is not already at food
            {
                foodInSight = checkInSight(food); //Check if the food is within FOV
                if (!foodInSight && !goingToFood) //If no food is in sight, and it is not going to one already (redundancy)
                {
                    blobWander(); //Wander aimlessly (sorta... computer randmoness is only so good)
                }
                else if (!goingToFood)
                {
                    goingToFood = true; //It is going to food!
                    goToFood(food); //Go to the food
                }
                else
                {
                    if (!agent.pathPending) //If there is no destination being baked on the navmesh
                    {
                        if (agent.remainingDistance <= 2) //If the distance to food is less than 2 units
                        {
                            agent.enabled = false; //turn off the agent
                            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) //If there is no path or the agent is not moving (both are usually true, but just in case it hasn't stopped due to a frame skip)
                            {
                                atFood = true; //The blob is at the food, stop running the 
                                goingToFood = false; //We are not travelling anymore
                                Destroy(food); //Destroy the food  (no other blobs can have it)
                                eaten = true;
                                agent.enabled = true; //reenable the navmesh
                                toEdge();
                            }
                        }
                    }
                }
            }
        }
    }

    private void toEdge()
    {
        // agent.enabled = true;
        NavMeshHit hit;
        if(NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
        goingToEdge = true;
    }
    #endregion

    #region Reproduction
    public void passTraits()
    {
        Dictionary<string, float> passedVals = new Dictionary<string, float>();
        foreach(KeyValuePair<string, float> gene in genes)
        {
            int i = UnityEngine.Random.Range(1, 101);
            if (i <= 5)
            {
                int j = UnityEngine.Random.Range(1, 11);
                int val = UnityEngine.Random.Range(1, 101);
                if (j <= 5)
                {
                    passedVals.Add(gene.Key, gene.Value - (.01f * val));
                }
                else
                {
                    passedVals.Add(gene.Key, gene.Value + (.01f * val));
                }
            }
            else
            {
                passedVals.Add(gene.Key, gene.Value);
            }
        }
        blobData newBlob = ScriptableObject.CreateInstance<blobData>();
        newBlob.aggression = passedVals["aggression"];
        newBlob.speed = passedVals["speed"];
        newBlob.sight = passedVals["sight"];

        createBlob(newBlob);
    }

    private void createBlob(blobData newBlob)
    {
        blobPrefab = Instantiate(blobPrefab, spawnPosition.transform.position, Quaternion.identity);
        blobPrefab.GetComponent<Blob>().data = newBlob;
    }
    #endregion

}
