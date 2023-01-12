using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
  [SerializeField]
  private Character character;
  private InputManager inputManager;

  private Rigidbody2D playerRigid2d;
  private Transform playerTransform;

  private void Awake()
  {
    if (character == null) {
      character = GetComponent<Character>();
    }

    playerRigid2d = GetComponent<Rigidbody2D>();
    playerTransform = GetComponent<Transform>();

    inputManager = new InputManager();
  }

  private void Start()
  {
    playerRigid2d.centerOfMass = Vector2.zero;
  }

  private void Update()
  {
    if (!isLocalPlayer) return;
    
    inputManager.UpdateInput();
    LookAtMouse();
  }

  private void FixedUpdate()
  {
    if (!isLocalPlayer) return;

    //Move();
  }

  private void LookAtMouse()
  {
    Vector2 mouseDirectionFromPlayer = inputManager.MouseWorldPosition - (Vector2)playerTransform.position;
    mouseDirectionFromPlayer.Normalize();

    float mouseAngle = Mathf.Atan2(
      mouseDirectionFromPlayer.y,
      mouseDirectionFromPlayer.x
    ) * Mathf.Rad2Deg;

    Quaternion mouseDirection = Quaternion.AngleAxis(mouseAngle - 90, Vector3.forward);

    playerTransform.rotation = Quaternion.Slerp(
      playerTransform.rotation,
      mouseDirection,
      5f * Time.deltaTime
    );
  }
}