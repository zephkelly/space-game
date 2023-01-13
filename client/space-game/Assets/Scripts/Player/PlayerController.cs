using UnityEngine;
using Mirror;

enum ControlScheme
{
  RelativeToCamera,
  RelativeToPlayer
}

public class PlayerController : NetworkBehaviour
{
  [SerializeField]
  private Character character;
  private Ship ship;
  private InputManager inputManager;
  private ControlScheme controlScheme;
  [SerializeField]
  private GameObject cameraPrefab;
  private CameraManager cameraManager;

  private Rigidbody2D playerRigid2D;
  private Transform playerTransform;
  private Vector2 mouseDirectionFromPlayer;

  private void Awake()
  {
    if (character == null) {
      character = GetComponent<Character>();
    }

    playerRigid2D = GetComponent<Rigidbody2D>();
    playerTransform = GetComponent<Transform>();

    character.Ship = new Ship(ShipClass.Scout, ShipEngine.Ion, ShipPrimaryWeapon.Laser, ShipSecondaryWeapon.None, ShipSpecialWeapon.None);
    ship = character.Ship;

    inputManager = new InputManager();
    controlScheme = ControlScheme.RelativeToCamera;

    Vector3 cameraSpawnPos = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);

    cameraManager = new CameraManager(
      Instantiate(cameraPrefab, cameraSpawnPos, Quaternion.identity),
      playerTransform,
      inputManager
    );
  }

  private void Start()
  {
    if (!isLocalPlayer) {
      cameraManager.SetEnabled(false);
      return;
    }

    playerRigid2D.centerOfMass = Vector2.zero;
  }

  private void Update()
  {
    if (!isLocalPlayer) return;
    
    inputManager.UpdateInput(playerTransform.position);
    LookAtMouse();
  }

  private void FixedUpdate()
  {
    if (!isLocalPlayer) return;

    Move(controlScheme);
    cameraManager.FollowPlayer();

    //Linear drag
    playerRigid2D.AddForce(-playerRigid2D.velocity * playerRigid2D.mass, ForceMode2D.Force);
      if (playerRigid2D.velocity.magnitude < 0.1f) playerRigid2D.velocity = Vector2.zero;
  }

  private void LookAtMouse(bool lerpRotation = false)
  {
    float mouseAngle = Mathf.Atan2(
      inputManager.MouseToPlayerPosition.y,
      inputManager.MouseToPlayerPosition.x
    ) * Mathf.Rad2Deg;

    Quaternion mouseDirection = Quaternion.AngleAxis(mouseAngle - 90, Vector3.forward);

    if (!lerpRotation) {
      playerTransform.rotation = mouseDirection;
    } else {
      playerTransform.rotation = Quaternion.Lerp(
        playerTransform.rotation,
        mouseDirection,
        ship.EngineTurnSpeed * Time.deltaTime
      );
    }
  }

  private void Move(ControlScheme currentControlScheme)
  {
    switch(currentControlScheme) {
      case ControlScheme.RelativeToCamera:
        MoveRelativeToCamera();
        break;
      case ControlScheme.RelativeToPlayer:
        MoveRelativeToPlayer();
        break;
    }

    void MoveRelativeToCamera()
    {
      Vector2 inputNormalised = inputManager.InputAxisRaw;
      inputNormalised.Normalize();

      if (inputManager.GetShift) {

        playerRigid2D.AddForce(inputNormalised * ship.EngineSpeed, ForceMode2D.Force);
      } else {

        playerRigid2D.AddForce(inputNormalised * 20, ForceMode2D.Force);
      }
    }

    void MoveRelativeToPlayer()
    {
      if (inputManager.InputAxisRaw == Vector2.zero) return;

      if (inputManager.GetShift) {

        playerRigid2D.AddForce(playerTransform.up * ship.EngineSpeed, ForceMode2D.Force);
      } else {

        playerRigid2D.AddForce(playerTransform.up * 20, ForceMode2D.Force);
      }
    }
  }
}