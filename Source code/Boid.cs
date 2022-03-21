using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour {
    public School School { get; set; }

    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;
    
    void Start() {
        Velocity = Random.insideUnitSphere * 2;
    }

    public void UpdateSimulation(float deltaTime) {
        //Clear acceleration from last frame
        Acceleration = Vector3.zero;

        //Apply forces
        Acceleration += (Vector3)School.GetForceFromBounds(this);
        Acceleration += GetConstraintSpeedForce();
        Acceleration += GetSteeringForce();

        //Step simulation
        Velocity += deltaTime * Acceleration;
        Position +=  0.5f * deltaTime * deltaTime * Acceleration + deltaTime * Velocity;
    }

    Vector3 GetSteeringForce() {
        Vector3 cohesionForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 separationForce = Vector3.zero;
        Vector3 evadingPredatorForce = Vector3.zero;

        Vector3 averageBoidVelocity = Vector3.zero;
        Vector3 averageBoidPosition = Vector3.zero;

        int numberOfAlignmentBoids = 0;
        int numberOfCohesionBoids = 0;

        //Boid forces
        foreach (Boid neighbor in School.BoidManager.GetNeighbors(this, School.NeighborRadius)) {
            float distance = (neighbor.Position - Position).magnitude;

            //Separation force
            if (distance < School.SeparationRadius) {
                separationForce += School.SeparationForceFactor * ((School.SeparationRadius - distance) /
                distance) * (Position - neighbor.Position);
            }

            //First we sum the velocity/position. 
            if (distance < School.AlignmentRadius) {
                numberOfAlignmentBoids++;
                averageBoidVelocity += neighbor.Velocity;
            }
            if (distance < School.CohesionRadius) {
                numberOfCohesionBoids++;
                averageBoidPosition += neighbor.Position;
            }
        }

        //Here we take the average by dividing by the number of boids for each behaviour. 
        if (numberOfAlignmentBoids != 0) averageBoidVelocity = averageBoidVelocity / numberOfAlignmentBoids;
        if (numberOfCohesionBoids != 0) averageBoidPosition = averageBoidPosition / numberOfCohesionBoids;

        //Set cohesion/alignment forces here
        if (numberOfAlignmentBoids != 0) alignmentForce = School.AlignmentForceFactor * (averageBoidVelocity - Velocity);
        if (numberOfCohesionBoids != 0) cohesionForce = School.CohesionForceFactor * (averageBoidPosition - Position);

        evadingPredatorForce = EvadePredator();

        return alignmentForce + cohesionForce + separationForce + evadingPredatorForce;
    }

    Vector3 EvadePredator() {
        Predator predator = School.BoidManager.GetPredator();
        float distance = (predator.GetPosition() - Position).magnitude;

        Vector3 force = Vector3.zero;
        if (distance < School.PredatorEvadingRadius) {
            force += School.PredatorEvadingForceFactor * ((School.PredatorEvadingRadius - distance) / distance) *
            (Position - predator.GetPosition());
        }

        return force;
    }

    Vector3 GetConstraintSpeedForce() {
        Vector3 force = Vector3.zero;

        //Apply drag
        force -= School.Drag * Velocity;

        float vel = Velocity.magnitude;
        if (vel > School.MaxSpeed) { 
            //If speed is above the maximum allowed speed, apply extra friction force
            force -= (20.0f * (vel - School.MaxSpeed) / vel) * Velocity;
        }
        else if (vel < School.MinSpeed) {
            //Increase the speed slightly in the same direction if it is below the minimum
            force += (5.0f * (School.MinSpeed - vel) / vel) * Velocity;
        }
        
        return force;
    }
}
