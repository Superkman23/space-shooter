using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  public int _Damage = 10;

  void OnTriggerEnter2D (Collider2D collision) {
    var health = collision.GetComponent<IHealth>();
    health?.TakeHealth(_Damage);

    Destroy (gameObject);
  }
}
