using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour {

    School school = null;

    MathHelpers mathHelper = new MathHelpers();

    [SerializeField]
    Vector3 predatorPosition;

    [SerializeField]
    Vector3 predatorVelocity;

    [SerializeField]
    Vector3 predatorAcceleration;

    [SerializeField]
    Boid preyToHunt = null;

    bool restAfterEating = false;

    float sightDistance = 10;

    float huntingSpeed = 0.15f;

    int counter = 0;

    public void UpdateSimulation(float deltaTime) {
        //Clear acceleration from last frame
        predatorAcceleration = Vector3.zero;

        if (preyToHunt == null && !restAfterEating) {
            preyToHunt = SearchForPrey();
            print("Searching for prey.");
        }
        else if (preyToHunt != null && !restAfterEating) {
            HuntPrey(preyToHunt);
            print("Hunting prey");
        }
        else print("Resting.");

        PerformRestCheck();

        //Prevents predator from swiming outside boundary
        predatorAcceleration += school.PredatorBounds(this);

        //Step simulation
        predatorVelocity += predatorAcceleration * deltaTime;
        predatorPosition += predatorVelocity * deltaTime;
    }

    //Returns the Boid of the prey if found, null otherwise. 
    Boid SearchForPrey() {
        List<Boid> preyCollection = school.BoidManager.GetPreyCollection();

        foreach (Boid prey in preyCollection) {
            Vector3 currentPoint = predatorPosition;
            Vector3 sightEndPoint = predatorPosition + (predatorVelocity.normalized * sightDistance * 10);

            Vector3 sightVector = sightEndPoint - currentPoint;
            Vector3 targetVector = prey.Position - currentPoint;

            float distanceAlongSightToTarget = mathHelper.ProjectOntoVector(targetVector, sightVector);
            float distanceFromTargetToSightLine = mathHelper.DistancePointToLine(prey.Position, currentPoint, sightEndPoint);

            for (int i = 0; i < 10; i++) {
                float min = sightDistance * ((float)i / 10);
                float max = sightDistance * ((float)(i + 1) / 10);

                if (distanceAlongSightToTarget > min && distanceAlongSightToTarget < max &&
                    distanceFromTargetToSightLine < ((sightDistance / 2) * ((float)(1 + i) / 10)) &&
                    distanceAlongSightToTarget != -1) {
                    return prey;
                }
            }
        }
        return null;
    }

    void HuntPrey(Boid prey) {
        Vector3 directionToHunt = (prey.Position - predatorPosition).normalized;
        predatorVelocity = directionToHunt * huntingSpeed;
    }

    void PerformRestCheck() {
        if (restAfterEating) {
            predatorAcceleration = Vector3.zero;
            predatorVelocity *= 0.999f;
            counter += 1;

            if (counter > 10000) {
                restAfterEating = false;
                predatorVelocity = Random.insideUnitSphere.normalized * school.GetPredatorSpeed();
                counter = 0;
            } 
        }
    }

    void OnTriggerEnter(Collider other) {
        if (preyToHunt != null) {
            if (preyToHunt.gameObject == other.gameObject) {
                restAfterEating = true;
                Destroy(preyToHunt.gameObject);
            }
        }
    }

    public Vector3 GetPosition() {
        return predatorPosition;
    }

    public Vector3 GetVelocity() {
        return predatorVelocity;
    }

    public Vector3 GetAcceleration() {
        return predatorAcceleration;
    }

    public void SetPosition(Vector3 position) {
        predatorPosition = position;
    }

    public void SetVelocity(Vector3 velocity) {
        predatorVelocity = velocity;
    }

    public void SetAcceleration(Vector3 acceleration) {
        predatorAcceleration = acceleration;
    }

    public void SetSchool(School school) {
        this.school = school;
    }
}
