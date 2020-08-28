using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  Rigidbody2D _Rigidbody;
  bool _Direction; //true = right
  [SerializeField] ControlLayout _Layout = 0;
  [SerializeField] float _MaxSpeed;
  [SerializeField] float _Acceleration;
  Vector2 _VelocityChange;


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
  }

  private void FixedUpdate()
  {
    HandleMovement();



    _Rigidbody.velocity += _VelocityChange;
    _VelocityChange = Vector2.zero;
    if(_Rigidbody.velocity.x > 0){
      _Direction = true;
    } else if(_Rigidbody.velocity.x < 0)  {
      _Direction = false;
    }
  }

  void HandleMovement()
  {
    Vector2 input = GetInput(_Layout);

    if(input.y > 0)
    {
      if (IsGrounded())
      {
        Jump();
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
    _VelocityChange.y += 10;
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
