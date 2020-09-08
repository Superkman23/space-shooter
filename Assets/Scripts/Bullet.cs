using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
  Rigidbody2D _Rigidbody;
  [SerializeField] float _Speed;

  private void Awake()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    if (!isServer)
    {
      _Rigidbody.isKinematic = true;
    }
  }

  private void Update()
  {
    if(isServer)
      _Rigidbody.MovePosition(transform.position + transform.right * _Speed * Time.fixedDeltaTime);
  }





}
