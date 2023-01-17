using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private ParticleSystem starfieldParticleSystem;
    private ParticleSystem.Particle[] stars;
    private Transform cameraTransform;

    [SerializeField] Transform parallaxLayer;
    [SerializeField] int starsMax = 100;
    [SerializeField] float starSizeMin = 0.15f;
    [SerializeField] float starSizeMax = 0.4f;
    [SerializeField] float starSpawnRadius = 10;
    [SerializeField] float parallaxFactor = 0.9f;

    private float starDistanceSqr;
    private float starClipDistanceSqr;
    private static int particleZ = 2;

    private void Awake()
    {
      cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
      starfieldParticleSystem = GetComponent<ParticleSystem>();

      starDistanceSqr = starSpawnRadius * starSpawnRadius;
    }

    private void Start () 
    {
      CreateStars();
    }

    private void CreateStars()
    {
      stars = new ParticleSystem.Particle[starsMax];

      for (int i = 0; i < starsMax; i++)
      {
        stars[i].position = ((Vector3)Random.insideUnitCircle * starSpawnRadius) + cameraTransform.position;
        stars[i].position = new Vector3(stars[i].position.x, stars[i].position.y, particleZ);
        stars[i].startColor = new Color(1,1,1, 1);
        stars[i].startSize = Random.Range(starSizeMin, starSizeMax);
      }

      starfieldParticleSystem.SetParticles(stars, stars.Length);
    }
 
    private void Update() 
    {
			if (cameraTransform == null) return;
      Vector3 cameraParallaxDelta = (Vector2)(cameraTransform.position - parallaxLayer.position);

      for (int i = 0; i < starsMax; i++)
      {
        Vector2 starPosition = (Vector2)(stars[i].position + parallaxLayer.position);

        if((starPosition - (Vector2)cameraTransform.position).sqrMagnitude > starDistanceSqr) 
        {
          stars[i].position = (Vector3)(Random.insideUnitCircle.normalized * starSpawnRadius) + cameraParallaxDelta;
        }
      }
    }

    public void Parallax(Vector3 cameraLastPosition)   //called on camera manager
    {
      Vector3 cameraDelta = (Vector2)(cameraTransform.position - cameraLastPosition); 

      parallaxLayer.position = Vector3.Lerp(
        parallaxLayer.position, 
        parallaxLayer.position + cameraDelta, 
        parallaxFactor * Time.deltaTime
      );

      if (stars == null || starfieldParticleSystem == null) return;
      starfieldParticleSystem.SetParticles(stars, stars.Length);
    }
  
}
