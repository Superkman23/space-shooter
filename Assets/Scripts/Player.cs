using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
  SpriteRenderer _Renderer;
  PhysicsLink _Link;
  [SerializeField] Sprite _RedSprite;
  [SerializeField] Sprite _BlueSprite;

  [SerializeField] float _Acceleration;
  [SerializeField] float _MoveSpeed;

  void Start()
  {
    _Link = GetComponent<PhysicsLink>();
    _Renderer = GetComponent<SpriteRenderer>();
    _Renderer.sprite = hasAuthority ? _BlueSprite : _RedSprite;
  }
  [Client]
  void FixedUpdate()
  {
    if (!hasAuthority) { return; }
    CMDSendInput(GetInput());
  }

  [Command]
  void CMDSendInput(Vector2 input)
  {
    Vector2 velocityChange = Vector2.zero;


    // Movement
    float force = input.x * _MoveSpeed;
    force -= _Link._Rigidbody.velocity.x;
    float acceleration = _Acceleration * Time.fixedDeltaTime;
    force = Mathf.Clamp(force, -acceleration, acceleration);

    velocityChange.x = force;
    RpcApplyForce(velocityChange);
  }

  [ClientRpc]
  void RpcApplyForce(Vector2 input)
  {
    _Link.AddDirectForce(input);
  }


  Vector2 GetInput()
  {
    Vector2 input = Vector2.zero;
    if (Input.GetKey(KeyCode.W))
    {
      input.y++;
    }
    if (Input.GetKey(KeyCode.A))
    {
      input.x--;
    }
    if (Input.GetKey(KeyCode.S))
    {
      input.y--;
    }
    if (Input.GetKey(KeyCode.D))
    {
      input.x++;
    }
    return input;
  }


}
