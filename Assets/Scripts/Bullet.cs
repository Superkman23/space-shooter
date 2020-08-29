using UnityEngine;

public class Bullet : MonoBehaviour {
  public int _Damage = 10;
  public Collider2D _Parent = null;

  void OnTriggerEnter2D (Collider2D collision) {
    if (collision == _Parent) {
      return;
    }

    var health = collision.GetComponent<IHealth> ();
    health?.TakeHealth (_Damage);

    Destroy (gameObject);
  }

  void OnTriggerStay2D (Collider2D collision) {
    if (collision == _Parent) {
      return;
    }

    var health = collision.GetComponent<IHealth> ();
    health?.TakeHealth (_Damage);

    Destroy (gameObject);
  }
}
