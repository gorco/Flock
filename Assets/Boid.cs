using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public Vector3 velocity; // The current velocity
    public Vector3 newVelocity; // The velocity for next frame
    public Vector3 newPosition; // The position for next frame

    public float maxSpeed = 50f;
    public float minSpeed = 5f;

    public float cohesionWeight = 0.1f;
    public float separationWeight = 1f;
    public float aligmentWeight = 0.1f;

    public float separationMin = 4f;
    public float flockDistance = 30f;

    void Awake()
    {
        Vector3 randPos = Random.insideUnitSphere * BoidSpawner.C.spawnRadius;
        randPos.y = 0;
        this.transform.position = randPos;
        velocity = Random.onUnitSphere;
        velocity *= 1f;
        // Give the Boid a random color, but make sure it's not too dark
        Color randColor = Color.black;
        while (randColor.r + randColor.g + randColor.b < 1.0f)
        {
            randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            r.material.color = randColor;
        }
        this.transform.parent = GameObject.Find("Boids").transform;
    }

    void Update()
    {
        // Initialize newVelocity and newPosition to the current values using the Reynolds rules
        newVelocity = velocity + GetFlockBehaviour();
        newPosition = this.transform.position;

        Vector3 randomVel = Random.insideUnitSphere;

        // Utilizes the fields set on the CubeSpawner.C Singleton
        newVelocity += randomVel * BoidSpawner.C.velocityMatchingAmt;

        Vector3 dist;
        dist = BoidSpawner.C.mousePos - this.transform.position;

        if (dist.magnitude > BoidSpawner.C.mouseAvoidanceDist)
        {
            newVelocity += dist * BoidSpawner.C.mouseAttractionAmt;
        }
        else
        {
            // If the mouse is too close, move away quickly!
            newVelocity -= dist * BoidSpawner.C.mouseAvoidanceAmt;
        }
    }

    void LateUpdate()
    {
        // Adjust the current velocity based on newVelocity using a linear
        //  interpolation
        velocity = (1 - BoidSpawner.C.velocityLerpAmt) * velocity +
            BoidSpawner.C.velocityLerpAmt * newVelocity;

        if(velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        } 
        else if (velocity.magnitude < minSpeed)
        {
            velocity = velocity.normalized * minSpeed;
        }

        // Decide on the newPosition
        newPosition = this.transform.position + velocity * Time.deltaTime;
        // Keep everything in the XZ plane
        newPosition.y = 0;
        // Look from the old position at the newPosition to orient the model
        this.transform.LookAt(newPosition);
        // Actually move to the newPosition
        this.transform.position = newPosition;

    }

    Vector3 GetFlockBehaviour()
    {
        // Get all sibling
        List<Boid> boids = BoidSpawner.C.GetBoids();

        Vector3 cohesion = CohesionRule(boids) * cohesionWeight;
        Vector3 separation = SeparationRule(boids) * separationWeight;
        Vector3 aligment = AligmentRule(boids) * aligmentWeight;

        // Reynolds rules
        return cohesion + separation + aligment;
    }

    // Rule1
    Vector3 CohesionRule(List<Boid> someBoids)
    {
        int count = 0;
        Vector3 sum = Vector3.zero;
        foreach (Boid s in someBoids)
        {
            float dist = Vector3.Distance(s.transform.position, this.transform.position);
            if (dist < flockDistance && dist > 0)
            {
                sum += s.transform.position;
                count++;
            }
                
        }
        if (count > 0)
        {
            Vector3 center = sum / (count);
            return center - this.transform.position;
        }
        return Vector3.zero;
    }

    // Rule2
    Vector3 SeparationRule(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        Vector3 dist = Vector3.zero;
        foreach (Boid s in someBoids)
        {
            if(s != this.transform)
            {
                dist = s.transform.position-this.transform.position;
                if (dist.magnitude < separationMin)
                {
                    sum -= dist;
                }
            }
        }
        return sum;
    }

    // Rule3
    public Vector3 AligmentRule(List<Boid> someBoids)
    {
        int count = 0;
        Vector3 sum = Vector3.zero;
        foreach (Boid s in someBoids)
        {
            if (s != this.transform)
            {
                float dist = Vector3.Distance(s.transform.position, this.transform.position);
                if (dist < flockDistance && dist > 0)
                {
                    sum += s.velocity;
                    count++;
                }
            }
            
        }
        if (count > 0)
        {
            Vector3 avg = sum / (count);
            return avg - this.velocity;
        }

        return Vector3.zero;
    }
}