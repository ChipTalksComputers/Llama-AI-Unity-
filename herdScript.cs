using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class herdScript : MonoBehaviour
{
    public GameObject llama;
    public static List<GameObject> llamas = new List<GameObject>();

    private float herdCenterx = 500;
    private float herdCenterz = 100;

    private void Start()
    {
        int n = Random.Range(3, 7);
        for (int i = 0; i < n; i++)
        {
            llamas.Add(Instantiate(llama, new Vector3(Random.Range(herdCenterx - 25, herdCenterx + 25), 0, Random.Range(herdCenterz - 25, herdCenterz + 25)), Quaternion.Euler(0, Random.Range(0, 360), 0)));
        }
    }
}
