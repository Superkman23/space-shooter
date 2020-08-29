using UnityEngine;

public class Bullet : MonoBehaviour {
  public int _Damage = 10;
  public Collider2D[] _Ignoring = null;

  void OnTriggerEnter2D (Collider2D collision) {
    foreach (var ignore in _Ignoring) {
      if (collision == ignore) {
        return;
      }
    }

    if (collision.GetComponent<Bullet>()) {
      return;
    }

    var health = collision.GetComponent<IHealth> ();
    health?.TakeHealth (_Damage);

    Destroy (gameObject);
  }

  void OnTriggerStay2D (Collider2D collision) {
    foreach (var ignore in _Ignoring) {
      if (collision == ignore) {
        return;
      }
    }

    if (collision.GetComponent<Bullet>()) {
      return;
    }

    var health = collision.GetComponent<IHealth> ();
    health?.TakeHealth (_Damage);

    Destroy (gameObject);
  }
}
