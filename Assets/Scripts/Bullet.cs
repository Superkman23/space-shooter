using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
  Rigidbody2D _Rigidbody;
  [SerializeField] float _Speed;
  GameObject _Creator;
  [SerializeField] int _Damage;

  private void Awake()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.gravityScale = 0;
    _Rigidbody.velocity = transform.right * _Speed;
  }

  [ServerCallback]
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.transform.CompareTag("Player"))
    {
      if(collision.gameObject != _Creator)
      {
        Debug.Log("Hit player!");
        collision.gameObject.GetComponent<Player>().TakeDamage(_Damage);
        NetworkServer.Destroy(gameObject);
      }
    } else if (!collision.transform.CompareTag("Bullet"))
    {
      NetworkServer.Destroy(gameObject);
    }


  }

  public void SetCreator(GameObject creator)
  {
    _Creator = creator;
  }
}
