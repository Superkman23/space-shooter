using UnityEngine;

public class Gun : MonoBehaviour, IPickup, IEntitySpawn {
  [Header ("Settings")]
  [SerializeField] float _BulletForce = 10;
  [SerializeField] int _ThrownDamage = 5;
  [SerializeField] Vector2 _OffsetFromPlayer = Vector2.zero;
  [SerializeField] int _ClipSize = 10;
  [SerializeField] bool _UnlimitedAmmo = false;
  
  [Header("Audio")]
  [SerializeField] AudioClip _ShootSfx = null;
  [SerializeField] AudioClip _PickupSfx = null;
  [SerializeField] AudioClip _ThrowSfx = null;
  [SerializeField] AudioClip _ThrowCollideSfx = null;
  
  [Header ("Components")]
  [SerializeField] GameObject _BulletObject = null;
  Rigidbody2D _Rigidbody = null;
  Collider2D _Collider = null;
  AudioSource _Source = null;

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
    _Source = GetComponent<AudioSource> ();
  }

  void OnTriggerStay2D (Collider2D other) {
    // If the thing colliding is the player
    if (other.CompareTag ("Player") && _State == PickupState.Dropped) {
      // Trigger the pickup event
      OnPickup (other.gameObject);
    }

    if (_Thrown && other != _Collider && other.GetComponent<Gun> () == null) {
      // Try hit something, if nothing then just destroy itself
      var health = other.GetComponent<IHealth> ();
      health?.TakeHealth (_ThrownDamage);

      _Source.PlayOneShot(_ThrowCollideSfx);

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
      newBullet.GetComponent<Bullet> ()._Ignoring = new Collider2D[2] { _Parent._Collider, _Collider };

      _Source.PlayOneShot(_ShootSfx);
    }

    if (!_UnlimitedAmmo) {
      // Shoot one of the bullets in the magazine
      _ClipSize--;
    }

    // Returns true if the gun is empty, in which case the gun is thrown
    return _ClipSize < 0;
  }

  public void Throw () {
    // Get the direction the player is facing, then instantiate a new bullet and shoot the bullet in that direction
    int direction = (int) _Parent._Player._Direction;

    // Officially throw the gun
    _Thrown = true;
    transform.parent = null;

    _Source.PlayOneShot(_ThrowSfx);

    // Throw the gun
    _Collider.enabled = true;
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

    // Special case: when the gun is spawned through an entity spawner
    if (_SpawnerParent != null) {
      OnLeaveSpawn ();
    }

    controller._Holding = this;

    _Source.PlayOneShot(_PickupSfx);

    _State = PickupState.PickedUp;
    _Collider.enabled = false;

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

  EntitySpawner _SpawnerParent = null;
  // Empty on purpose
  public void OnEnterSpawn (EntitySpawner spawner) { _SpawnerParent = spawner; }

  public void OnLeaveSpawn () {
    _SpawnerParent.ObjectLeaveSpawn ();
  }
}
