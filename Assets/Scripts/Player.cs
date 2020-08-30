using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
  PhysicsLink _Link;

  [Header("Visuals")]
  [SerializeField] Sprite _RedSprite;
  [SerializeField] Sprite _BlueSprite;
  [SerializeField] ParticleSystem _JetpackParticles;
  SpriteRenderer _Renderer;

  [Header("Movement")]
  [SerializeField] float _Acceleration;
  [SerializeField] float _MoveSpeed;

  [Header("Debug")]
  [SyncVar] [SerializeField] Vector2 D_Input;
  [SyncVar] [SerializeField] Vector2 D_Velocity;
  [SyncVar] [SerializeField] Vector2 D_Position;
  [SyncVar] [SerializeField] float D_Force;
  [SyncVar] [SerializeField] float D_VelocityChange;
  [SyncVar] [SerializeField] float D_Acceleration;


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
    D_Input = input; // Debug
    D_Velocity = _Link._Rigidbody.velocity; // Debug
    D_Position = _Link._Rigidbody.position; // Debug
    Vector2 velocityChange = Vector2.zero;

    // Movement
    float force = input.x * _MoveSpeed;
    force -= _Link._Rigidbody.velocity.x * (float)NetworkTime.rtt;
    if(input.x == 0) { force /= 2; }
    D_Force = force; // Debug

    float acceleration = _Acceleration * Time.fixedDeltaTime;
    D_Acceleration = acceleration; //Debug
    force = Mathf.Clamp(force, -acceleration, acceleration);
    velocityChange.x = force;
    D_VelocityChange = velocityChange.x; // Debug
    RpcFlipSprite(input.x);

    // Jumping
    if(input.y > 0)
    {
      RpcJump();
      RPCJetpackParticles(true);
    }
    else
    {
      RPCJetpackParticles(false);
    }

    RpcApplyForce(velocityChange);
    if (_Link._Rigidbody.velocity.magnitude < 0.2f)
    {
      RpcApplyForce(-_Link._Rigidbody.velocity);
    }
  }

  #region RPC
  [ClientRpc]
  void RpcApplyForce(Vector2 input)
  {
    _Link.AddDirectForce(input);
  }
  [ClientRpc]
  void RpcJump()
  {
    if(_Link._Rigidbody.velocity.y < 10)
      _Link.AddDirectForce(Vector2.up);
  }
  [ClientRpc]
  void RpcFlipSprite(float input)
  {
    if (input > 0)
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    else if (input < 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
    }
  }
  [ClientRpc]
  void RPCJetpackParticles(bool active)
  {
    DoJetpackParticles(active);
  }
  void DoJetpackParticles(bool active)
  {
    if (active)
    {
      _JetpackParticles.Play();
    }
    else
    {
      _JetpackParticles.Stop();
    }
  }

  #endregion

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
