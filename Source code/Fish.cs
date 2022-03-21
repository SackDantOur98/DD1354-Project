using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour 
{
    Boid m_boid = null;
    Predator m_predator = null;

	void Start () 
    {
        if (transform.tag == "Predator") m_predator = GetComponent<Predator>();
        else m_boid = GetComponent<Boid>();
	}
	
	void Update () {
        Vector3 velocity = Vector3.zero;
        Vector3 position = Vector3.zero;

        if (transform.tag == "Predator") {
            velocity = m_predator.GetVelocity();
            position = m_predator.GetPosition();
        }
        else {
            velocity = m_boid.Velocity;
            position = m_boid.Position;
        } 

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 6);
            
        }

        transform.position = position;
        transform.position += new Vector3(0, 0, transform.parent.position.z); //inherit z-position
	}
}
