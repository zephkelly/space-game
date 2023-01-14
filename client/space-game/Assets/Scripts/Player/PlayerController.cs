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

  [SerializeField]
  private GameObject parallaxingStarfieldPrefab;

  [SerializeField]
  private GameObject primaryWeaponObject;
  [SerializeField]
  private ParticleSystem primaryWeaponParticleSystem;

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

    //Here we would instead inject the player stats from the server
    character.Ship = new Ship(ShipClass.Scout, ShipEngine.Ion, ShipPrimaryWeapon.Laser, ShipSecondaryWeapon.None, ShipSpecialWeapon.None);
    ship = character.Ship;
  }

  private void Start()
  {
    var primaryWeaponCollision = primaryWeaponParticleSystem.collision;

    if (!isLocalPlayer) {
      gameObject.layer = LayerMask.NameToLayer("Enemy");
      gameObject.tag = "Enemy";
      primaryWeaponObject.tag = "Enemy";
      primaryWeaponCollision.collidesWith  = 1 << LayerMask.NameToLayer("Player");
      return;
    } else {
      gameObject.layer = LayerMask.NameToLayer("Player");
      gameObject.tag = "Player";
      primaryWeaponObject.tag = "Player";
      primaryWeaponCollision.collidesWith  = 1 << LayerMask.NameToLayer("Enemy");
    }
    
    inputManager = new InputManager();
    controlScheme = ControlScheme.RelativeToCamera;

    playerRigid2D.centerOfMass = Vector2.zero;
    CreateCamera();
  }

  public void CreateCamera()
  {
    Vector3 cameraSpawnPos = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);

    cameraManager = new CameraManager(
      Instantiate(cameraPrefab, cameraSpawnPos, Quaternion.identity),
      Instantiate(parallaxingStarfieldPrefab, Vector3.zero, Quaternion.identity),
      playerTransform,
      inputManager
    );
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
    Vector3 mousePosition = inputManager.MouseToPlayerPosition.normalized;

    float mouseAngle = Mathf.Atan2(
      mousePosition.y,
      mousePosition.x
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

        playerRigid2D.AddForce(inputNormalised * 60, ForceMode2D.Force);
      }
    }

    void MoveRelativeToPlayer()
    {
      if (inputManager.InputAxisRaw == Vector2.zero) return;

      if (inputManager.GetShift) {

        playerRigid2D.AddForce(playerTransform.up * ship.EngineSpeed, ForceMode2D.Force);
      } else {

        playerRigid2D.AddForce(playerTransform.up * 60, ForceMode2D.Force);
      }
    }
  }

  private void OnParticleCollision(GameObject hitObject)
  {
    if (hitObject.tag == "Enemy") {
      Debug.Log("Hit enemy");

      TakeDamage(10);   
    }
  }

  private void TakeDamage(int damage)
  {
    character.TakeDamage(damage);

    Debug.Log("Taken damage");
    Debug.Log("Current Health: " + character.Ship.Health);

    if (isServer) TargetTakeDamage(connectionToServer, damage);
  }

  [TargetRpc]
  private void TargetTakeDamage(NetworkConnection target, int damage)
  {
    character.TakeDamage(damage);

    Debug.Log("Taken damage");
    Debug.Log("Current Health: " + character.Ship.Health);
  }
}