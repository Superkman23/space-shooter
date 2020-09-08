using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
  Rigidbody2D _Rigidbody;
  [SerializeField] float _Speed;
  public GameObject _Creator; //The player who created the game object

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

    if(collision.transform.CompareTag("Player"))
    {
      if(collision.gameObject != _Creator)
      {
        Debug.Log("Hit player!");
        NetworkServer.Destroy(gameObject);
        //TODO add damage things here
      }
    } else if (!collision.transform.CompareTag("Bullet"))
    {
      NetworkServer.Destroy(gameObject);
    }


  }

}
