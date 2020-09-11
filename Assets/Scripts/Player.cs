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
  [SerializeField] KeyCode _ShootKey = KeyCode.S;
  float _ShootDelayR;
  bool _PressedShootButton;

  [Header("Health")]
  [SerializeField] float _MaxHealth = 5;

  //Game Stats
  [SyncVar] float _Health;
  //TODO: Add playername

  void Start()
  {
    _Health = _MaxHealth;


    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.isKinematic = !hasAuthority;

    _ParticleSync = GetComponent<SyncParticles>();

    _Renderer = GetComponent<SpriteRenderer>();
    _Renderer.color = hasAuthority ? _PlayerColor : _EnemyColor;

    _ShootDelayR = 0;

    if (isLocalPlayer) { SetParticles(_JetpackParticles, false); }
  }

  private void Update()
  {
    if (!isLocalPlayer) { return; }
    if (Input.GetKeyDown(_ShootKey))
    {
      _PressedShootButton = true;
    }
  }


  void FixedUpdate()
  {
    if (!isLocalPlayer) { return; }
    _ShootDelayR -= Time.fixedDeltaTime;
    Vector2 input = GetInput();

    SetParticles(_JetpackParticles, input.y > 0);

    if ((_ShootDelayR < 0 && Input.GetKey(_ShootKey)) || _PressedShootButton)
    {
      _PressedShootButton = false;
      _ShootDelayR = _ShootDelay;
      CmdShoot();
    }

    //Vertical Movement
    if (input.y > 0)
    {
      if (_Rigidbody.velocity.y < _JetpackMaxSpeed)
      {
        _Rigidbody.velocity += Vector2.up * _JetpackAcceleration * Time.fixedDeltaTime;
      }
    }
    //Horizontal Movement
    float force = input.x * _MoveSpeed;
    force -= _Rigidbody.velocity.x;
    float acceleration = _Acceleration * Time.deltaTime;
    force = Mathf.Clamp(force, -acceleration, acceleration);
    _Rigidbody.velocity += Vector2.right * force;

    //Sprite flipping
    if (input.x < 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    else if (input.x > 0)
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
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

  [Command]
  void CmdShoot()
  {
    GameObject bullet = Instantiate(_BulletPrefab, transform.position, transform.rotation);
    bullet.GetComponent<Bullet>().SetCreator(gameObject);
    NetworkServer.Spawn(bullet);
  }

  public void TakeDamage(int damage)
  {
    _Health -= damage;
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
