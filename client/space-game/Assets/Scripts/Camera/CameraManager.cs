using UnityEngine;
using Mirror;

public class CameraManager
{
  private GameObject cameraObject;
  private Transform cameraTransform;
  private Camera camera;
  private Transform playerTransform;
  private InputManager inputManager;

  private Vector3 cameraFollowOffset = new Vector3(0f, 0f, -10f);
  private float mouseInterpolateDistance = 0.2f;
  private float cameraPanSpeed = 0.125f;

  public CameraManager(GameObject _cameraObject, Transform _playerTransform, InputManager _inputManager)
  {
    cameraObject = _cameraObject;
    cameraTransform = cameraObject.GetComponent<Transform>();
    camera = cameraObject.GetComponent<Camera>();
    playerTransform = _playerTransform;
    inputManager = _inputManager;
  }

  public void SetCameraPosition(Vector3 _position)
  {
    cameraTransform.position = _position;
  }

  public void FollowPlayer()
  {
    if (playerTransform == null) return;

    Vector3 cameraOffset = playerTransform.position + cameraFollowOffset;
    Vector3 cameraFollowMouseOffset = cameraOffset + (Vector3)(inputManager.MouseToPlayerPosition * mouseInterpolateDistance);

    Vector3 cameraLastPosition = cameraTransform.position;
    cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraFollowMouseOffset, cameraPanSpeed);
  }
}
