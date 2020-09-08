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
    _Rigidbody.gravityScale = 0;
  }

  private void Update()
  {
    if(isServer)
      _Rigidbody.MovePosition(transform.position + transform.right * _Speed * Time.fixedDeltaTime);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (!isServer)
      return;
    if (!collision.transform.CompareTag("Player"))
    {
      NetworkServer.Destroy(gameObject);
    }


  }

}
