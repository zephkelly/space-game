using UnityEngine;

public class CameraManager
{
  private GameObject cameraObject;
  private Camera camera;
  private Transform cameraTransform;
  private InputManager inputManager;

  public CameraManager(GameObject _cameraObject, InputManager _inputManager)
  {
    cameraObject = _cameraObject;
    camera = cameraObject.GetComponent<Camera>();
    cameraTransform = cameraObject.transform;

    inputManager = _inputManager;
  }

  public void SetEnabled(bool _enabled)
  {
    camera.enabled = _enabled;
  }

  public void SetCameraPosition(Vector3 _position)
  {
    cameraTransform.position = _position;
  }
}
