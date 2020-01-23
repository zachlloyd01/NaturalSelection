using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cycleManager : MonoBehaviour
{
    public bool isDay;
    // Start is called before the first frame update
    void Start()
    {
        isDay = false;
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
        List<GameObject> doneCreatures = new List<GameObject>();

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
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("creature"))
        {
            if(doneCreatures.Contains(go)) { }

            else
            {
                return;
            }
        }
        if(doneCreatures.Count == GameObject.FindGameObjectsWithTag("creature").Length)
        {
            isDay = false;
            setState();
        }
    }

    private void setDay()
    {
        if(isDay)
        {
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("creature"))
            {
                go.GetComponent<Blob>().run = true;
            }
        }
    }

    private void setState()
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("creature"))
        {
            go.GetComponent<Blob>().passTraits();
        }
        isDay = true;
        setDay();

    }
}
