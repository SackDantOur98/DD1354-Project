using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class School : MonoBehaviour
{
    [SerializeField]
    int m_numFish = 50;

    [SerializeField]
    Boid m_fishPrefab = null;

    [SerializeField]
    Predator m_predatorPrefab = null;

    [SerializeField]
    float predatorSpeed = 1;

    [SerializeField]
    float m_spawnRadius = 10;

    [SerializeField]
    BoxCollider m_bounds = null;

    [SerializeField]
    float m_boundsForceFactor = 5;

    [Header("Boid behaviour data. Experiment with changing these during runtime")]
    [SerializeField]
    float m_cohesionForceFactor = 1;
    public float CohesionForceFactor
    {
        get { return m_cohesionForceFactor; }
        set { m_cohesionForceFactor = value; }
    }

    [SerializeField]
    float m_cohesionRadius = 3;
    public float CohesionRadius
    {
        get { return m_cohesionRadius; }
        set { m_cohesionRadius = value; }
    }

    [SerializeField]
    float m_separationForceFactor = 1;
    public float SeparationForceFactor
    {
        get { return m_separationForceFactor; }
        set { m_separationForceFactor = value; }
    }

    [SerializeField]
    float m_separationRadius = 2;
    public float SeparationRadius
    {
        get { return m_separationRadius; }
        set { m_separationRadius = value; }
    }

    [SerializeField]
    float m_alignmentForceFactor = 1;
    public float AlignmentForceFactor
    {
        get { return m_alignmentForceFactor; }
        set { m_alignmentForceFactor = value; }
    }

    [SerializeField]
    float m_alignmentRadius = 3;
    public float AlignmentRadius
    {
        get { return m_alignmentRadius; }
        set { m_alignmentRadius = value; }
    }

    [SerializeField]
    float m_predatorEvadingRadius = 5;
    public float PredatorEvadingRadius {
        get { return m_predatorEvadingRadius; }
        set { m_predatorEvadingRadius = value; }
    }

    [SerializeField]
    float m_predatorEvadingForceFactor = 5;
    public float PredatorEvadingForceFactor
    {
        get { return m_predatorEvadingRadius; }
        set { m_predatorEvadingRadius = value; }
    }

    [SerializeField]
    float m_maxSpeed = 8;
    public float MaxSpeed
    {
        get { return m_maxSpeed; }
        set { m_maxSpeed = value; }
    }

    [SerializeField]
    float m_minSpeed;
    public float MinSpeed
    {
        get { return m_minSpeed; }
        set { m_minSpeed = value; }
    }

    [SerializeField]
    float m_drag = 0.1f;
    public float Drag
    {
        get { return m_drag; }
        set { m_drag = value; }
    }

    Vector3 force = new Vector3();



    public float NeighborRadius
    {
        get { return Mathf.Max(m_alignmentRadius, Mathf.Max(m_separationRadius, m_cohesionRadius)); }
    }

    public BoidManager BoidManager { get; set; }
    
    public IEnumerable<Boid> SpawnFish() {
        for (int i = 0; i < m_numFish; ++i) {
            Vector3 spawnPoint = CalculateSpawnPos();

            Boid boid = Instantiate(m_fishPrefab, spawnPoint, m_fishPrefab.transform.rotation) as Boid;
            boid.Position = spawnPoint;
            boid.Velocity = Random.insideUnitSphere;
            boid.School = this;
            boid.transform.parent = this.transform;
            yield return boid;
        }
    }

    public Predator SpawnPredator() {

        Vector3 spawnPoint = CalculateSpawnPos();

        Predator predator = Instantiate(m_predatorPrefab, spawnPoint, m_fishPrefab.transform.rotation) as Predator;

        predator.SetPosition(spawnPoint);
        predator.SetVelocity(Random.insideUnitSphere.normalized * predatorSpeed);
        predator.SetSchool(this);
        predator.transform.parent = this.transform;

        return predator;
    }

    Vector3 CalculateSpawnPos() {
        Vector3 spawnPoint = transform.position + m_spawnRadius * Random.insideUnitSphere;

        for (int j = 0; j < 3; ++j)
            spawnPoint[j] = Mathf.Clamp(spawnPoint[j], m_bounds.bounds.min[j], m_bounds.bounds.max[j]);

        return spawnPoint;
    }

    //Prevents the boids from swiming outside the bounded area
    public Vector3 GetForceFromBounds(Boid boid)
    {
        force = Vector3.zero;
        Vector3 centerToPos = (Vector3)boid.Position - transform.position;

        float friction = CalculateBounds(centerToPos);

        force += 0.1f * friction * (Vector3)boid.Velocity;
        return -m_boundsForceFactor * force;
    }

    //Prevents the predator from swiming outside the bounded area
    public Vector3 PredatorBounds(Predator predator) {
        force = Vector3.zero;
        Vector3 centerToPos = predator.GetPosition() - transform.position;

        float friction = CalculateBounds(centerToPos);

        force += 0.1f * friction * predator.GetVelocity();
        return -m_boundsForceFactor * force;
    }

    float CalculateBounds(Vector3 centerToPos) {
        Vector3 minDiff = centerToPos + m_bounds.size * 0.5f;
        Vector3 maxDiff = centerToPos - m_bounds.size * 0.5f;
        float friction = 0.0f;

        for (int i = 0; i < 3; ++i)
        {
            if (minDiff[i] < 0)
                force[i] = minDiff[i];
            else if (maxDiff[i] > 0)
                force[i] = maxDiff[i];
            else
                force[i] = 0;

            friction += Mathf.Abs(force[i]);
        }

        return friction;
    }

    public float GetPredatorSpeed() {
        return predatorSpeed;
    }
}
