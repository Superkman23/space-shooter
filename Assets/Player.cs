using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  Rigidbody2D _Rigidbody;
  SpriteRenderer _Renderer;



  bool _Direction; //true = right
  [SerializeField] ControlLayout _Layout = 0;
  [SerializeField] float _MaxSpeed;
  [SerializeField] float _Acceleration;
  Vector2 _VelocityChange;

  [SerializeField] bool _MidairJump;
  [SerializeField] float _JumpDelay;
  float _JumpDelayR;

  [SerializeField] LayerMask _WhatIsGround;
  [SerializeField] Vector2 _TopLeftGround;
  [SerializeField] Vector2 _BottomRightGround;

  public enum ControlLayout
  {
    WASD,
    Arrows
  }

  private void Awake()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    _Renderer = GetComponent<SpriteRenderer>();
  }

  private void FixedUpdate()
  {
    Vector2 input = GetInput(_Layout);

    HandleMovement(input);



    _JumpDelayR -= Time.deltaTime;

    _Rigidbody.velocity += _VelocityChange;
    _VelocityChange = Vector2.zero;

    if(input.x > 0){
      _Direction = true;
    } else if(input.x < 0)  {
      _Direction = false;
    }
    _Renderer.flipX = !_Direction;
  }

  void HandleMovement(Vector2 input)
  {
    if(input.y > 0)
    {
      if ((IsGrounded() || _MidairJump) && _JumpDelayR <= 0)
      {
        Jump();
        _JumpDelayR = _JumpDelay;
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



  Vector2 GetInput(ControlLayout layout)
  {
    Vector2 input = Vector2.zero;
    switch ((int)_Layout)
    {
      case 0:
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
        break;

      default:
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
        break;

    }
    return input;
  }

}
