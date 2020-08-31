using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
  Rigidbody2D _Rigidbody;

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

  void Start()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.isKinematic = !hasAuthority;



    _Renderer = GetComponent<SpriteRenderer>();
    _Renderer.sprite = hasAuthority ? _BlueSprite : _RedSprite;
  }

  [Client]
  void FixedUpdate()
  {
    if (!hasAuthority) { return; }
    Vector2 input = GetInput();
    _Rigidbody.velocity = input * _MoveSpeed;

    FlipSprite(input);
  }

  void FlipSprite(Vector2 input)
  {
    if(input.x < 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
    } else if(input.x > 0)
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
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
