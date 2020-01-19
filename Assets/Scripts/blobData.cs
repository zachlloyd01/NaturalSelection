using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blob Data")]
public class blobData : ScriptableObject
{
    public float speed;
    public float aggression;
    public float sight;

    public blobData(Dictionary<string, float> dict)
    {
        foreach(KeyValuePair<string, float> entry in dict)
        {
            this.GetType().GetProperty(entry.Key).SetValue(this, entry.Value, null);
        }
    }
}
