using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
  Rigidbody2D _Rigidbody;
  ParticleSystem _ParticleSystem;
  SpriteRenderer _Renderer;
  [SerializeField] float _Speed;
  Player _Creator;
  [SerializeField] int _Damage;
  [SerializeField] Gradient _FriendlyGradient;
  [SerializeField] Gradient _EnemyGradient;

  private void Awake()
  {
    _ParticleSystem = GetComponent<ParticleSystem>();
    _Renderer = GetComponent<SpriteRenderer>();

    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.gravityScale = 0;
    _Rigidbody.velocity = transform.right * _Speed;
  }


  [ServerCallback]
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.transform.CompareTag("Player"))
    {
      Player pHit = collision.transform.GetComponent<Player>();
      if(pHit != _Creator)
      {
        Debug.Log("Hit player!");
        pHit.TakeDamage(_Damage);
        NetworkServer.Destroy(gameObject);
      }
    } else if (!collision.transform.CompareTag("Bullet"))
    {
      NetworkServer.Destroy(gameObject);
    }


  }

  void UpdateVisuals()
  {
    var col = _ParticleSystem.colorOverLifetime;
    if (_Creator.isLocalPlayer)
    {
      col.color = _FriendlyGradient;
      _Renderer.color = _FriendlyGradient.Evaluate(0);
    }
    else
    {
      col.color = _EnemyGradient;
      _Renderer.color = _EnemyGradient.Evaluate(0);
    }
  }

  public void SetCreator(Player creator)
  {
    _Creator = creator;
    //UpdateVisuals();
  }
}
