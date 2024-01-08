using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public int numFish;

    public GameObject[] allFish;
    public Vector3 spawnBoundaries;
    public bool followLeader;

    [HideInInspector]
    public GameObject leader;
    [Range(0.0f, 50.0f)] public float leaderWeight;
    [Range(0.0f, 10.0f)] public float leaderTimer;
    public GameObject leaderTarget;

    [Header("Fish Settings")]
    [Range(0.0f, 10.0f)] public float minSpeed;
    [Range(0.0f, 10.0f)] public float maxSpeed;
    [Range(0.0f, 10.0f)] public float neighbourDistance;
    [Range(0.0f, 10.0f)] public float rotationSpeed;

    [Header("Target Settings")]
    public Vector3 targetPos;
    public bool randomlyMoveTarget;
    public Vector3 targetBoundaries;
    [Range(0.0f, 10.0f)] public float targetMoveTimer;
    public bool showTarget;

    // Start is called before the first frame update
    void Start()
    {
        allFish = new GameObject[numFish];

        for (int i = 0; i < numFish; ++i)
        {
            Vector3 randPos = new Vector3(Random.Range((-1) * spawnBoundaries.x, spawnBoundaries.x),
                Random.Range((-1)*spawnBoundaries.y, spawnBoundaries.y),
                Random.Range((-1) * spawnBoundaries.z, spawnBoundaries.z));
            Vector3 pos = this.transform.position + randPos;
            Vector3 randDirection = new Vector3(Random.Range((-1) * spawnBoundaries.x, spawnBoundaries.x),
                Random.Range((-1) * spawnBoundaries.y, spawnBoundaries.y),
                Random.Range((-1) * spawnBoundaries.z, spawnBoundaries.z));
            allFish[i] = (GameObject)Instantiate(fishPrefab, pos, Quaternion.LookRotation(randDirection));
            allFish[i].GetComponent<Flock>().myManager = this;
        }

        InvokeRepeating("PickLeader", 0, leaderTimer);

        // Target

        leaderTarget.transform.position = targetPos;

        if (randomlyMoveTarget)
        {
            InvokeRepeating("MoveTarget", 0, targetMoveTimer);
        }

        if (!showTarget)
        {
            leaderTarget.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // Picks a new leader
    void PickLeader()
    {
        if (leader != null)
        {
            leader.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }

        // Pick fish
        leader = allFish[Random.Range(0, numFish)];

        leader.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    void MoveTarget()
    { 
        Vector3 randPos = new Vector3(Random.Range((-1) * targetBoundaries.x, targetBoundaries.x),
                Random.Range((-1) * targetBoundaries.y, targetBoundaries.y),
                Random.Range((-1) * targetBoundaries.z, targetBoundaries.z));

        leaderTarget.transform.position = randPos;
    }
}
