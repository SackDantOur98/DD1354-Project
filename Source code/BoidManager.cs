using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    List<Boid> m_boids;

    Predator m_predator;

    void Start()
    {
        m_boids = new List<Boid>();

        var schools = GameObject.FindObjectsOfType<School>();

        foreach (var school in schools)
        {
            school.BoidManager = this;
            //Spawn fish
            m_boids.AddRange(school.SpawnFish());
            //Spawn predator
            m_predator = school.SpawnPredator();
        }
    }

    void FixedUpdate()
    {
        foreach (Boid boid in m_boids)
        {
            boid.UpdateSimulation(Time.fixedDeltaTime);
            m_predator.UpdateSimulation(Time.fixedDeltaTime);
        }
    }

    public Predator GetPredator() {
        return m_predator;
    }

    public List<Boid> GetPreyCollection() {
        return m_boids;
    }

    public IEnumerable<Boid> GetNeighbors(Boid boid, float radius)
    {
        float radiusSq = radius * radius;
        foreach (var other in m_boids)
        {
            if (other != boid && (other.Position - boid.Position).sqrMagnitude < radiusSq)
                yield return other;
        }
    }
}
