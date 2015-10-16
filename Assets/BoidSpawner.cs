using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidSpawner : MonoBehaviour
{
    static public BoidSpawner C;
    public GameObject boidPrefab;
    public int numCubes = 2;
    public float spawnRadius = 100f;
    public float velocityLerpAmt = 0.25f;
    public float velocityMatchingAmt = 0.01f;
    public float mouseAttractionAmt = 0.01f;
    public float mouseAvoidanceAmt = 0.75f;
    public float mouseAvoidanceDist = 50f;
    public Vector3 mousePos;

    private List<Boid> boidsList;

    // Use this for initialization
    void Start()
    {
        boidsList = new List<Boid>();

        C = this;
        for (int i = 0; i < numCubes; i++)
        {
            GameObject obj = Instantiate(boidPrefab);
            boidsList.Add(obj.GetComponent<Boid>());
        }
    }

    void LateUpdate()
    {
        Vector3 mousePos2d =
        new Vector3(Input.mousePosition.x,
        Input.mousePosition.y, this.transform.position.y);
        mousePos = this.GetComponent<Camera>().ScreenToWorldPoint(mousePos2d);
    }

    public List<Boid> GetBoids()
    {
        return boidsList;
    }
}