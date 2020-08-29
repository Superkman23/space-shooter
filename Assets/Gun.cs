using UnityEngine;

public class Gun : MonoBehaviour, IPickup
{
  [Header("Settings")]
  [SerializeField] float _BulletForce = 10;
  [SerializeField] int _ThrownDamage = 5;
  [SerializeField] Vector2 _OffsetFromPlayer = Vector2.zero;
  int _BulletsLeft = 10;

  [Header("Components")]
  [SerializeField] GameObject _BulletObject = null;
  Rigidbody2D _Rigidbody = null;

  // "States"
  // Thrown is when the player throws the gun after running out of ammo - will still do damage to anything that has health
  bool _Thrown = false;
  PickupState _State = PickupState.Dropped;

  void Awake()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.isKinematic = true;
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    // If the thing colliding is the player
    if (other.CompareTag("Player") && _State == PickupState.Dropped)
    {
      // Trigger the pickup event
      OnPickup(other.gameObject);
    }

    if (_Thrown)
    {
      // Try hit something, if nothing then just destroy itself
      var health = other.GetComponent<IHealth>();
      health?.TakeHealth(_ThrownDamage);

      _Rigidbody.velocity = Vector2.zero;
      _Rigidbody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
      GetComponent<Collider2D>().enabled = false;
      Invoke("DestroyGun", 5);
    }
  }

  void DestroyGun() => Destroy(gameObject);

  public bool Shoot()
  {
    // Shoot one of the bullets in the magazine
    _BulletsLeft--;

    // Get the direction the player is facing, then instantiate a new bullet and shoot the bullet in that direction
    int direction = (int)transform.parent.GetComponent<Player>()._Direction;
    GameObject newBullet = Instantiate(_BulletObject, transform.position + (Vector3.right * -direction / 1.5f), transform.parent.rotation);
    newBullet.GetComponent<Rigidbody2D>().AddForce((Vector3.right * -direction * _BulletForce), ForceMode2D.Impulse);

    // Returns true if the gun is empty, in which case the gun is thrown
    return _BulletsLeft == 0;
  }

  public void Throw()
  {
    // Get the direction the player is facing, then instantiate a new bullet and shoot the bullet in that direction
    int direction = (int)transform.parent.GetComponent<Player>()._Direction;

    // Officially throw the gun
    _Thrown = true;
    transform.parent = null;

    // Throw the gun
    _Rigidbody.position += Vector2.right * -direction;
    _Rigidbody.isKinematic = false;
    _Rigidbody.AddForce((Vector3.right * -direction * _BulletForce), ForceMode2D.Impulse);
  }

  public void OnDrop(GameObject who)
  {
    _State = PickupState.Dropped;

    transform.parent = null;
  }

  public void OnPickup(GameObject who)
  {
    _State = PickupState.PickedUp;

    who.GetComponent<PlayerGunController>()._Holding = this;

    transform.parent = who.transform;
    transform.localPosition = _OffsetFromPlayer;
  }

  public PickupState GetPickupState()
  {
    return _State;
  }
}
