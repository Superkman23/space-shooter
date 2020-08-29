using UnityEngine;

public class Gun : MonoBehaviour, IPickup {
  [SerializeField] GameObject _BulletObject = null;
  [SerializeField] Vector2 _OffsetFromPlayer = Vector2.zero;
  PickupState _State = PickupState.Dropped;

  int _BulletsLeft = 10;

  void OnTriggerEnter2D (Collider2D other) {
    // If the thing colliding is the player
    if (other.CompareTag ("Player") && _State == PickupState.Dropped) {
      // Trigger the pickup event
      OnPickup (other.gameObject);
    }
  }

  // Returns true if the gun is empty
  public bool Shoot () {
    Debug.Log ("Shooting gun");

    _BulletsLeft--;
    GameObject newBullet = Instantiate (_BulletObject, transform.position, transform.parent.rotation);
    newBullet.GetComponent<Rigidbody> ().AddForce (transform.parent.GetComponent<Player> ().*10);

    return _BulletsLeft == 0;
  }

  public void Throw () {
    Destroy (gameObject);
  }

  public void OnDrop (GameObject who) {
    _State = PickupState.Dropped;

    transform.parent = null;
  }

  public void OnPickup (GameObject who) {
    _State = PickupState.PickedUp;

    who.GetComponent<PlayerGunController> ()._Holding = this;

    transform.parent = who.transform;
    transform.localPosition = _OffsetFromPlayer;
  }

  public PickupState GetPickupState () {
    return _State;
  }
}
