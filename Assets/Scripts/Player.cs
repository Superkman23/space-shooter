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
  [SerializeField] float _Acceleration = 50;
  [SerializeField] float _Deceleration = 2;
  [SerializeField] float _JetpackMaxSpeed = 10;
  [SerializeField] float _JetpackAcceleration = 1;
  [SerializeField] Vector2 _MovementLimits = new Vector2(5, 5);

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
    Vector2 input = GetInput();
    HandleInput(input);
    //CMDSendInput(input);
  }

  void HandleInput(Vector2 input)
  {
    Vector2 velocityChange = Vector2.zero;
    // Movement
    float acceleration = _Acceleration * Time.fixedDeltaTime;
    velocityChange.x = input.x * acceleration;
    FlipSprite(input.x);

    // Jumping
    if (input.y > 0)
    {
      Jump();
      DoJetpackParticles(true);
    }
    else
    {
      DoJetpackParticles(false);
    }

    _Link.AddDirectForce(velocityChange);

    Vector2 velocity = _Link._Rigidbody.velocity;
    if (input.x == 0) { velocity.x /= _Deceleration; } // Handles deceleration
    velocity.x = Mathf.Clamp(velocity.x, -_MovementLimits.x, _MovementLimits.x);
    velocity.y = Mathf.Clamp(velocity.y, -_MovementLimits.y, _MovementLimits.y);

    _Link._Rigidbody.velocity = velocity;
  }

  [Command]
  void CMDSendInput(Vector2 input)
  {
    Vector2 velocityChange = Vector2.zero;
    // Movement
    float acceleration = _Acceleration * Time.fixedDeltaTime;
    velocityChange.x = input.x * acceleration;
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

    Vector2 velocity = _Link._Rigidbody.velocity;
    if(input.x == 0) { velocity.x /= _Deceleration; } // Handles deceleration
    velocity.x = Mathf.Clamp(velocity.x, -_MovementLimits.x, _MovementLimits.x);
    velocity.y = Mathf.Clamp(velocity.y, -_MovementLimits.y, _MovementLimits.y);

    _Link._Rigidbody.velocity = velocity;
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
    if (!hasAuthority) { return; }
    Jump();
  }
  void Jump() 
  {
    if (_Link._Rigidbody.velocity.y < _JetpackMaxSpeed)
      _Link.AddDirectForce(Vector2.up * _JetpackAcceleration);
  }


  [ClientRpc]
  void RpcFlipSprite(float input)
  {
    if (hasAuthority) { return; }
    FlipSprite(input);
  }
  void FlipSprite(float input)
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
    if (!hasAuthority) { return; }
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
