using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryWeaponFireWrapper : MonoBehaviour
{
  [SerializeField] ParticleSystem laserParticleSystem;
  private ParticleSystemRenderer laserParticleRenderer;
  List<ParticleCollisionEvent> collisonEvents = new List<ParticleCollisionEvent>();

  public void Awake()
  {
    laserParticleSystem = gameObject.GetComponent<ParticleSystem>();
    laserParticleRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
  }

  public void Shoot()
  {
    laserParticleSystem.Play();
  }

  private void OnParticleCollision(GameObject hitObject)
  {
    //Grab where our particles are colliding
    laserParticleSystem.GetCollisionEvents(hitObject, collisonEvents);
    Vector2 hitPoint = collisonEvents[0].intersection;

    //Add force to the object we hit
    var directionOfForce = (hitPoint - (Vector2)transform.position).normalized;
    var objectRigid2D = hitObject.GetComponent<Rigidbody2D>();
  }
}
