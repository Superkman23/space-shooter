using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
  PhysicsLink _Link;

  [Header("Visuals")]
  [SerializeField] Sprite _RedSprite;
  [SerializeField] Sprite _BlueSprite;
  [SerializeField] ParticleSystem _JetpackParticles;
  SpriteRenderer _Renderer;

  [Header("Movement")]
  [SerializeField] float _MoveSpeed = 10;
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

    CMDTransform(GetInput() * _MoveSpeed);
    transform.Translate(GetInput() * _MoveSpeed * Time.fixedDeltaTime);
  }

  [Command]
  void CMDTransform(Vector2 input)
  {
    RPCTransform(input);
  }

  [ClientRpc]
  void RPCTransform(Vector2 input)
  {
    if (hasAuthority) { return; }
    transform.Translate(input * Time.fixedDeltaTime);
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
