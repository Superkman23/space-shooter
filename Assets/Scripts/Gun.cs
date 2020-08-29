using UnityEngine;

public class Gun : MonoBehaviour, IPickup {
  [Header ("Settings")]
  [SerializeField] float _BulletForce = 10;
  [SerializeField] int _ThrownDamage = 5;
  [SerializeField] Vector2 _OffsetFromPlayer = Vector2.zero;
  [SerializeField] int _ClipSize = 10;

  [Header ("Components")]
  [SerializeField] GameObject _BulletObject = null;
  Rigidbody2D _Rigidbody = null;
  Collider2D _Collider = null;

  class GunParent {
    public Collider2D _Collider = null;
    public Rigidbody2D _Rigidbody = null;
    public Player _Player = null;
  }
  GunParent _Parent = new GunParent ();

  // "States"
  // Thrown is when the player throws the gun after running out of ammo - will still do damage to anything that has health
  bool _Thrown = false;
  PickupState _State = PickupState.Dropped;

  void Awake () {
    _Rigidbody = GetComponent<Rigidbody2D> ();
    _Rigidbody.isKinematic = true;

    _Collider = GetComponent<Collider2D> ();
  }

  void OnTriggerStay2D (Collider2D other) {
    // If the thing colliding is the player
    if (other.CompareTag ("Player") && _State == PickupState.Dropped) {
      // Trigger the pickup event
      OnPickup (other.gameObject);
    }

    if (_Thrown && other != _Collider) {
      // Try hit something, if nothing then just destroy itself
      var health = other.GetComponent<IHealth> ();
      health?.TakeHealth (_ThrownDamage);

      _Rigidbody.velocity = Vector2.zero;
      _Rigidbody.AddForce (Vector2.up * 5, ForceMode2D.Impulse);
      GetComponent<Collider2D> ().enabled = false;
      Invoke ("DestroyGun", 5);
    }
  }

  void DestroyGun () => Destroy (gameObject);

  public bool Shoot () {
    if (_ClipSize > 0) {
      // Get the direction the player is facing, then instantiate a new bullet and shoot the bullet in that direction
      int direction = (int) _Parent._Player._Direction;
      GameObject newBullet = Instantiate (_BulletObject, transform.position + (Vector3.right * -direction / 1.5f), transform.parent.rotation);
      newBullet.GetComponent<Rigidbody2D> ().velocity = (Vector3.right * -direction * _BulletForce) + (Vector3.right * _Parent._Rigidbody.velocity.x / 2);
      newBullet.GetComponent<Bullet> ()._Parent = _Parent._Collider;

    }
    // Shoot one of the bullets in the magazine
    _ClipSize--;
    // Returns true if the gun is empty, in which case the gun is thrown
    return _ClipSize < 0;
  }

  public void Throw () {
    // Get the direction the player is facing, then instantiate a new bullet and shoot the bullet in that direction
    int direction = (int) _Parent._Player._Direction;

    // Officially throw the gun
    _Thrown = true;
    transform.parent = null;

    // Throw the gun
    _Rigidbody.isKinematic = false;
    transform.position += Vector3.right * -direction / 2;
    _Rigidbody.AddForce (Vector3.right * -direction * _BulletForce, ForceMode2D.Impulse);

    _Parent = null;
  }

  public void OnDrop (GameObject who) {
    // NOT USED
  }

  public void OnPickup (GameObject who) {
    PlayerGunController controller = who.GetComponent<PlayerGunController> ();
    if (controller._Holding != null) {
      return;
    }

    controller._Holding = this;

    _State = PickupState.PickedUp;

    transform.parent = who.transform;
    _Parent._Rigidbody = who.GetComponent<Rigidbody2D> ();
    _Parent._Collider = who.GetComponent<Collider2D> ();
    _Parent._Player = who.GetComponent<Player> ();

    transform.localPosition = _OffsetFromPlayer;
    transform.localRotation = Quaternion.identity;
  }

  public PickupState GetPickupState () {
    return _State;
  }
}
