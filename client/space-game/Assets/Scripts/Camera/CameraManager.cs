using UnityEngine;
using Mirror;

public class CameraManager
{
  private Camera camera;
  private GameObject cameraObject;
  private Transform cameraTransform;

  private Transform playerTransform;
  private InputManager inputManager;

  private Vector3 cameraFollowOffset = new Vector3(0f, 0f, -10f);
  private float mouseInterpolateDistance = 0.2f;
  private float cameraPanSpeed = 0.125f;

  [SerializeField] ParallaxController[] starfieldsLayers;

  public CameraManager(GameObject _cameraObject, GameObject _parallaxStarfieldObject, Transform _playerTransform, InputManager _inputManager)
  {
    cameraObject = _cameraObject;
    cameraTransform = cameraObject.GetComponent<Transform>();
    camera = cameraObject.GetComponent<Camera>();
    playerTransform = _playerTransform;
    inputManager = _inputManager;

    starfieldsLayers = _parallaxStarfieldObject.GetComponentsInChildren<ParallaxController>();
  }

  public void SetCameraPosition(Vector3 _position)
  {
    cameraTransform.position = _position;
  }

  public void SetNewPlayerTransform(Transform _playerTransform)
  {
    playerTransform = _playerTransform;
  }

  public void FollowPlayer()
  {
    if (playerTransform == null) return;

    Vector3 cameraOffset = playerTransform.position + cameraFollowOffset;
    Vector3 cameraFollowMouseOffset = cameraOffset + (Vector3)(inputManager.MouseToPlayerPosition * mouseInterpolateDistance);

    Vector3 cameraLastPosition = cameraTransform.position;
    cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraFollowMouseOffset, cameraPanSpeed);

    if (cameraLastPosition == null) return;
    UpdateParallaxing(cameraLastPosition);
  }

  private void UpdateParallaxing(Vector2 cameraLastPosition)
  {
    //Starfields
    for (int i = 0; i < starfieldsLayers.Length; i++)
    {
      starfieldsLayers[i].Parallax(cameraLastPosition);
    }
  }
}
