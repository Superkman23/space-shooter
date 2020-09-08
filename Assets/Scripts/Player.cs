using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
  Rigidbody2D _Rigidbody;

  [Header("Visuals")]
  [SerializeField] Color _EnemyColor = Color.red;
  [SerializeField] Color _PlayerColor = Color.blue;
  [SerializeField] ParticleSystem _JetpackParticles = null;
  SyncParticles _ParticleSync;
  SpriteRenderer _Renderer;

  [Header("Movement")]
  [SerializeField] float _MoveSpeed = 10;
  [SerializeField] float _Acceleration = 50;
  [SerializeField] float _JetpackMaxSpeed = 10;
  [SerializeField] float _JetpackAcceleration = 1;

  [Header("Shooting")]
  [SerializeField] GameObject _BulletPrefab;
  [SerializeField] float _ShootDelay = 0.2f;
  float _ShootDelayR;

  void Start()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.isKinematic = !hasAuthority;

    _ParticleSync = GetComponent<SyncParticles>();

    _Renderer = GetComponent<SpriteRenderer>();
    _Renderer.color = hasAuthority ? _PlayerColor : _EnemyColor;

    _ShootDelayR = 0;

    if (hasAuthority) { SetParticles(_JetpackParticles, false); }
  }

  [Client]
  void FixedUpdate()
  {
    if (!hasAuthority) { return; }
    _ShootDelayR -= Time.fixedDeltaTime;
    Vector2 input = GetInput();

    HandleMovement(input);
    FlipSprite(input);
  }

  void HandleMovement(Vector2 input)
  {
    SetParticles(_JetpackParticles, input.y > 0);

    if(input.y > 0)
    {
      Jetpack();
    }

    if(input.y < 0)
    {
      if(_ShootDelayR < 0)
      {
        _ShootDelayR = _ShootDelay;
        Shoot(gameObject);
      }
    }

    float force = input.x * _MoveSpeed;
    force -= _Rigidbody.velocity.x;
    float acceleration = _Acceleration * Time.deltaTime;
    force = Mathf.Clamp(force, -acceleration, acceleration);
    _Rigidbody.velocity += Vector2.right * force;
  }

  void SetParticles(ParticleSystem system, bool active)
  {
    if (active)
    {
      system.Play();
      _ParticleSync.SetActive(true);
    }
    else
    {
      system.Stop();
      _ParticleSync.SetActive(false);
    }
  }

  void Jetpack()
  {
    if(_Rigidbody.velocity.y < _JetpackMaxSpeed)
    {
      _Rigidbody.velocity += Vector2.up * _JetpackAcceleration * Time.fixedDeltaTime;
    }
  }
  void FlipSprite(Vector2 input)
  {
    if(input.x < 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
    } else if(input.x > 0)
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
  }

  [Command]
  void Shoot(GameObject creator)
  {
    GameObject bullet = Instantiate(_BulletPrefab, transform.position, transform.rotation);
    Bullet bScript = bullet.GetComponent<Bullet>();
    bScript._Creator = creator;
    NetworkServer.Spawn(bullet);
    Destroy(bullet, 3);
  }

  Vector2 GetInput()
  {
    Vector2 input = Vector2.zero;
    if (Input.GetKey(KeyCode.W))
    {
      input.y++;
    }
    if (Input.GetKey(KeyCode.A))
    {
      input.x--;
    }
    if (Input.GetKey(KeyCode.S))
    {
      input.y--;
    }
    if (Input.GetKey(KeyCode.D))
    {
      input.x++;
    }
    return input;
  }
}
