using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cycleManager : MonoBehaviour
{
    private List<GameObject> doneCreatures = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("creature"))
        {
            go.GetComponent<Blob>().run = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("creature"))
        {
            if(go.GetComponent<Blob>().energy <= 0)
            {
                Destroy(go);
        
            }
            else
            {
                if(go.GetComponent<Blob>().onEdge == true)
                {
                    doneCreatures.Add(go);
                }
            }
        }
        if(doneCreatures.Count == GameObject.FindGameObjectsWithTag("creature").Length)
        {
            setState();
        }
    }

    private void setDay()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("creature"))
        {
            go.GetComponent<Blob>().run = true;
        }
    }

    private void setState()
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("creature"))
        {
            go.GetComponent<Blob>().passTraits();
        }
        setDay();

    }
}
