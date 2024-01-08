using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Flock : MonoBehaviour
{

    public FlockManager myManager;

    int speed;

    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {   
        Combination();
    
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                      Quaternion.LookRotation(direction),
                                      myManager.rotationSpeed * Time.deltaTime);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);
    }

    // Neighbour center of mass
    Vector3 Cohesion()
    {
        Vector3 cohesion = Vector3.zero;
        int num = 0;
        foreach (GameObject go in myManager.allFish)
        {
            if (go != this.gameObject)
            {
                float distance = Vector3.Distance(go.transform.position,
                                                  transform.position);
                if (distance <= myManager.neighbourDistance)
                {
                    cohesion += go.transform.position;
                    num++;
                }
            }
        }
        if (num > 0)
            cohesion = (cohesion / num - transform.position).normalized * speed;

        return cohesion;
    }

    // Average neighbour heading
    Vector3 Align()
    {
        Vector3 align = Vector3.zero;
        int num = 0;
        foreach (GameObject go in myManager.allFish)
        {
            if (go != this.gameObject)
            {
                float distance = Vector3.Distance(go.transform.position,
                                                  transform.position);
                if (distance <= myManager.neighbourDistance)
                {
                    align += go.GetComponent<Flock>().direction;
                    num++;
                }
            }
        }
        if (num > 0)
        {
            align /= num;
            speed = (int)Mathf.Clamp(align.magnitude, myManager.minSpeed, myManager.maxSpeed);
        }

        return align;
    }

    // Avoid crowding neighbours
    Vector3 Separation()
    {
        Vector3 separation = Vector3.zero;
        foreach (GameObject go in myManager.allFish)
        {
            if (go != this.gameObject)
            {
                float distance = Vector3.Distance(go.transform.position,
                                                  transform.position);
                if (distance <= myManager.neighbourDistance)
                    separation -= (transform.position - go.transform.position) /
                                  (distance * distance);
            }
        }

        return separation;
    }

    // Follow leader
    Vector3 Leader()
    {
        Vector3 leader = (myManager.leader.transform.position - transform.position) * myManager.leaderWeight;

        return leader;
    }

    // Follow Target

    Vector3 Target()
    {
        Vector3 target = (myManager.leaderTarget.transform.position - transform.position);

        return target;
    }

    // Combine Cohesion/Align/Separation in a single vector
    void Combination()
    {

        direction = Cohesion() + Align() + Separation();

        if (myManager.followLeader)
        {
            if (this.gameObject != myManager.leader)
            {
                direction += Leader();
            }
            else
            {
                direction = Target();
            }
        }

        direction = direction.normalized * speed;
    }
}
