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
  [SerializeField] Vector2 _MaxSpeedLimits = Vector2.zero;

  [Header("Shooting")]
  [SerializeField] GameObject _BulletPrefab;
  [SerializeField] Transform _BulletSpawnPosition;
  [SerializeField] float _HoldShootDelay = 0.2f;
  [SerializeField] float _MinShootDelay = 0.1f;
  [SerializeField] KeyCode _ShootKey = KeyCode.S;
  float _ShootDelayR;
  bool _PressedShootButton;

  [Header("Health")]
  [SerializeField] float _MaxHealth = 5;

  //Game Stats
  [SyncVar] float _Health;
  [SyncVar] bool _IsDead;
  float _DeathTime;
  //TODO: Add playername

  void Start()
  {
    _Rigidbody = GetComponent<Rigidbody2D>();
    _Rigidbody.isKinematic = !hasAuthority;

    _ParticleSync = GetComponent<SyncParticles>();

    _Renderer = GetComponent<SpriteRenderer>();

    Setup();
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
    if (_IsDead) {
      if(_DeathTime >= 3)
      {
        CmdRespawn();
      }
      _DeathTime += Time.deltaTime;
      return; 
    }

    if (!isLocalPlayer) { return; }


    Vector2 input = GetInput();

    SetParticles(_JetpackParticles, input.y > 0);

    //Shooting
    if ((_ShootDelayR <= 0 && Input.GetKey(_ShootKey)) || _PressedShootButton)
    {
      float difference = _HoldShootDelay - _MinShootDelay;


      _PressedShootButton = false;
      if(_ShootDelayR - difference <= 0)
      {
        CmdShoot();
        _ShootDelayR = _HoldShootDelay;
      }
    }
    _ShootDelayR -= Time.fixedDeltaTime;

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

    //Clamp speed to max movement
    Vector2 tempVelocity = _Rigidbody.velocity;
    tempVelocity.x = Mathf.Clamp(tempVelocity.x, -_MaxSpeedLimits.x, _MaxSpeedLimits.x);
    tempVelocity.y = Mathf.Clamp(tempVelocity.y, -_MaxSpeedLimits.y, _MaxSpeedLimits.y);
    _Rigidbody.velocity = tempVelocity;

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

  void Setup()
  {
    _Health = _MaxHealth;
    _ShootDelayR = 0;
    _Renderer.color = hasAuthority ? _PlayerColor : _EnemyColor;
    _DeathTime = 0;
    _IsDead = false;
    _Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    _Rigidbody.velocity = Vector2.zero;
    if (isLocalPlayer) { SetParticles(_JetpackParticles, false); }
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
    GameObject bullet = Instantiate(_BulletPrefab, _BulletSpawnPosition.position + (Vector3)(_Rigidbody.velocity * (float)NetworkTime.rtt), transform.rotation);
    bullet.GetComponent<Bullet>().SetCreator(this);
    NetworkServer.Spawn(bullet);
  }

  [ClientRpc]
  void Die()
  {
    _Renderer.color = Color.black;
    SetParticles(_JetpackParticles, false);
    Debug.Log(transform.name + "Died. feels bad :(");
    _Rigidbody.constraints = RigidbodyConstraints2D.None;
    _IsDead = true;
  }

  [Command]
  void CmdRespawn()
  {
    RpcRespawn();
  }

  [ClientRpc]
  void RpcRespawn()
  {
    //Restore Player stats
    Debug.Log(transform.name + "has respawned");
    Transform spawnPosition = NetworkManager.singleton.GetStartPosition();
    transform.rotation = spawnPosition.rotation;
    transform.position = spawnPosition.position;
    Setup();
  }

  public void TakeDamage(int damage)
  {
    _Health -= damage;
    if (_Health <= 0)
    {
      Die();
    }
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
