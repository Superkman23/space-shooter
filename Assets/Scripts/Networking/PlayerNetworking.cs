using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworking : NetworkBehaviour
{
  /*
  [SerializeField] private Vector3 vector = Vector3.zero;
  // Update is called once per frame
  [Client]
  void Update()
  {
    if (!hasAuthority) { return; }
    if (!Input.GetKeyDown(KeyCode.Space))
    {
      return;
    }
    transform.Translate(vector);
  }*/
  Rigidbody2D _Rigidbody;

  public enum Direction
  {
    Left = -1,
    Right = 1
  }

  [HideInInspector]
  public Direction _Direction = Direction.Left;
  [HideInInspector]
  public int _PlayerNumber = 0;

  [SerializeField] ControlLayout _Layout = 0;
  [SerializeField] float _MaxSpeed;
  [SerializeField] float _Acceleration;
  Vector2 _VelocityChange;

  [SerializeField] bool _MidairJump;
  [SerializeField] float _JumpDelay;
  float _JumpDelayR;
  bool _WantToJump;

  [SerializeField] LayerMask _WhatIsGround;
  [SerializeField] Vector2 _TopLeftGround;
  [SerializeField] Vector2 _BottomRightGround;

  [Header("Components")]
  [SerializeField] ParticleSystem _DeathParticle = null;

  int _Health = 30;

  public enum ControlLayout
  {
    WASD,
    Arrows,
    IJKL
  }

  private void Awake()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
  }

  private void Update()
  {
    if (!hasAuthority) { return; }
    if (!_WantToJump)
    {
      _WantToJump = GetInput(InputType.Down).y > 0;
    }
  }
  [Client]
  private void FixedUpdate()
  {
    if (!hasAuthority) { return; }
    Vector2 input = GetInput(InputType.Hold);

    HandleMovement(input);

    _JumpDelayR -= Time.deltaTime;

    _Rigidbody.velocity += _VelocityChange;
    _VelocityChange = Vector2.zero;

    if (input.x > 0)
    {
      _Direction = Direction.Left;
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    else if (input.x < 0)
    {
      _Direction = Direction.Right;
      transform.rotation = Quaternion.Euler(0, 180, 0);
    }
  }

  void HandleMovement(Vector2 input)
  {
    if (input.y > 0)
    {
      if ((IsGrounded() || _MidairJump) && (_JumpDelayR <= 0 || _WantToJump))
      {
        Jump();
        _JumpDelayR = _JumpDelay;
        _WantToJump = false;
      }
    }

    float target = input.x * _MaxSpeed;

    target -= _Rigidbody.velocity.x;

    float acceleration = _Acceleration * Time.deltaTime;
    target = Mathf.Clamp(target, -acceleration, acceleration);
    _VelocityChange.x = target;
  }

  void Jump()
  {
    _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, 10);
  }
  bool IsGrounded() => Physics2D.OverlapArea(_TopLeftGround + (Vector2)transform.position, _BottomRightGround + (Vector2)transform.position, _WhatIsGround);

  enum InputType
  {
    Hold,
    Down
  }
  Vector2 GetInput(InputType inputType)
  {
    Vector2 input = Vector2.zero;

    switch (inputType)
    {
      case InputType.Hold:
        if (_Layout == ControlLayout.WASD)
        {
          if (Input.GetKey(KeyCode.W))
          {
            input.y++;
          }
          if (Input.GetKey(KeyCode.S))
          {
            input.y--;
          }
          if (Input.GetKey(KeyCode.D))
          {
            input.x++;
          }
          if (Input.GetKey(KeyCode.A))
          {
            input.x--;
          }
        }
        else if (_Layout == ControlLayout.Arrows)
        {
          if (Input.GetKey(KeyCode.UpArrow))
          {
            input.y++;
          }
          if (Input.GetKey(KeyCode.DownArrow))
          {
            input.y--;
          }
          if (Input.GetKey(KeyCode.RightArrow))
          {
            input.x++;
          }
          if (Input.GetKey(KeyCode.LeftArrow))
          {
            input.x--;
          }
        }
        else
        {
          if (Input.GetKey(KeyCode.I))
          {
            input.y++;
          }
          if (Input.GetKey(KeyCode.K))
          {
            input.y--;
          }
          if (Input.GetKey(KeyCode.L))
          {
            input.x++;
          }
          if (Input.GetKey(KeyCode.J))
          {
            input.x--;
          }
        }
        break;
      case InputType.Down:
        if (_Layout == ControlLayout.WASD)
        {
          if (Input.GetKeyDown(KeyCode.W))
          {
            input.y++;
          }
          if (Input.GetKeyDown(KeyCode.S))
          {
            input.y--;
          }
          if (Input.GetKeyDown(KeyCode.D))
          {
            input.x++;
          }
          if (Input.GetKeyDown(KeyCode.A))
          {
            input.x--;
          }
        }
        else if (_Layout == ControlLayout.Arrows)
        {
          if (Input.GetKeyDown(KeyCode.UpArrow))
          {
            input.y++;
          }
          if (Input.GetKeyDown(KeyCode.DownArrow))
          {
            input.y--;
          }
          if (Input.GetKeyDown(KeyCode.RightArrow))
          {
            input.x++;
          }
          if (Input.GetKeyDown(KeyCode.LeftArrow))
          {
            input.x--;
          }
        }
        else
        {
          if (Input.GetKeyDown(KeyCode.I))
          {
            input.y++;
          }
          if (Input.GetKeyDown(KeyCode.K))
          {
            input.y--;
          }
          if (Input.GetKeyDown(KeyCode.L))
          {
            input.x++;
          }
          if (Input.GetKeyDown(KeyCode.J))
          {
            input.x--;
          }
        }
        break;
    }
    return input;
  }

  // Health interface implementation
  public int GetHealth() => _Health;
  public void SetHealth(int value) => _Health = value;
  public void AddHealth(int amount) => _Health += amount;
  public void TakeHealth(int amount)
  {
    _Health -= amount;
    Debug.Log(_Health);
    if (_Health <= 0)
    {
      Instantiate(_DeathParticle, transform.position, Quaternion.identity);
      //GameController._Instance.PlayerDeath(this);
      Destroy(gameObject);
    }
  }
}
