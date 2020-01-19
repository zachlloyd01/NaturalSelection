using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    #region Variables

    [Header("Variables")]
    [SerializeField] private GameObject blob;
    [SerializeField] private GameObject food;
    [SerializeField] private float radius;
    public GameObject ground;
    #endregion

    #region Default Functions
    // Start is called before the first frame update
    void Start()
    {
        radius = (ground.GetComponent<Collider>().bounds.size.x) / 2f;
        spawnFood();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    #endregion

    #region Spawning Logic
    private void spawnFood()
    {
        bool spawned;
        for (int i = 0; i < 30; i++)
        {
            spawned = false;
            while (!spawned)
            {
                Vector3 point; //An unset Vector to be used to go to
                if (RandomPoint(ground.transform.position, radius, out point)) //Outputs the point, takes in the ground and radius values
                {
                    food = Instantiate(food, point, Quaternion.identity);
                    food.tag = "Food";
                    food.name = $"Food - {food.transform.position} - Increment: {i}";
                    spawned = true;
                }
            }
        }
    }

    #endregion

    #region Values

    private bool RandomPoint(Vector3 center, float range, out Vector3 result) //A boolean that takes in a center and radius, outputs weather a Vector is on the NavMesh and a Vector to traverse to that is on the mesh
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
}
