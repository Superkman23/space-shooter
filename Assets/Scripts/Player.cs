using UnityEngine;

public class Player : MonoBehaviour, IHealth {
  Rigidbody2D _Rigidbody;

  public enum Direction {
    Left = -1,
    Right = 1
  }

  [HideInInspector]
  public Direction _Direction = Direction.Left;
  [HideInInspector]
  public int _PlayerNumber = 0;

  [SerializeField] ControlLayout _Layout = 0;
  [SerializeField] float _MaxSpeed;
  [SerializeField] float _Acceleration;
  Vector2 _VelocityChange;

  public bool _MidairJump;
  [SerializeField] float _JumpDelay;
  float _JumpDelayR;
  bool _WantToJump;

  [SerializeField] LayerMask _WhatIsGround;
  [SerializeField] Vector2 _TopLeftGround;
  [SerializeField] Vector2 _BottomRightGround;

  [Header ("Audio")]
  [SerializeField] AudioClip _HitSfx = null;
  [SerializeField] AudioClip _DeathSfx = null;
  AudioSource _Source = null;

  [Header ("Components")]
  [SerializeField] ParticleSystem _DeathParticle = null;

  int _Health = 30;

  public enum ControlLayout {
    WASD,
    Arrows,
    IJKL
  }

  private void Awake () {
    _Rigidbody = GetComponent<Rigidbody2D> ();
    _Source = GetComponent<AudioSource> ();
  }

  private void Update () {
    if (!_WantToJump) {
      _WantToJump = GetInput (InputType.Down).y > 0;
    }
  }

  private void FixedUpdate () {
    Vector2 input = GetInput (InputType.Hold);

    HandleMovement (input);

    _JumpDelayR -= Time.deltaTime;

    _Rigidbody.velocity += _VelocityChange;
    _VelocityChange = Vector2.zero;

    if (input.x > 0) {
      _Direction = Direction.Left;
      transform.rotation = Quaternion.Euler (0, 0, 0);
    }
    else if (input.x < 0) {
      _Direction = Direction.Right;
      transform.rotation = Quaternion.Euler (0, 180, 0);
    }
  }

  void HandleMovement (Vector2 input) {
    if (input.y > 0) {
      if ((IsGrounded () || _MidairJump) && (_JumpDelayR <= 0 || _WantToJump)) {
        Jump ();
        _JumpDelayR = _JumpDelay;
        _WantToJump = false;
      }
    }

    float target = input.x * _MaxSpeed;

    target -= _Rigidbody.velocity.x;

    float acceleration = _Acceleration * Time.deltaTime;
    target = Mathf.Clamp (target, -acceleration, acceleration);
    _VelocityChange.x = target;
  }

  void Jump () {
    _Rigidbody.velocity = new Vector2 (_Rigidbody.velocity.x, 10);
  }
  bool IsGrounded () => Physics2D.OverlapArea (_TopLeftGround + (Vector2) transform.position, _BottomRightGround + (Vector2) transform.position, _WhatIsGround);

  enum InputType {
    Hold,
    Down
  }
  Vector2 GetInput (InputType inputType) {
    Vector2 input = Vector2.zero;

    switch (inputType) {
      case InputType.Hold:
        if (_Layout == ControlLayout.WASD) {
          if (Input.GetKey (KeyCode.W)) {
            input.y++;
          }
          if (Input.GetKey (KeyCode.S)) {
            input.y--;
          }
          if (Input.GetKey (KeyCode.D)) {
            input.x++;
          }
          if (Input.GetKey (KeyCode.A)) {
            input.x--;
          }
        }
        else if (_Layout == ControlLayout.Arrows) {
          if (Input.GetKey (KeyCode.UpArrow)) {
            input.y++;
          }
          if (Input.GetKey (KeyCode.DownArrow)) {
            input.y--;
          }
          if (Input.GetKey (KeyCode.RightArrow)) {
            input.x++;
          }
          if (Input.GetKey (KeyCode.LeftArrow)) {
            input.x--;
          }
        }
        else {
          if (Input.GetKey (KeyCode.I)) {
            input.y++;
          }
          if (Input.GetKey (KeyCode.K)) {
            input.y--;
          }
          if (Input.GetKey (KeyCode.L)) {
            input.x++;
          }
          if (Input.GetKey (KeyCode.J)) {
            input.x--;
          }
        }
        break;
      case InputType.Down:
        if (_Layout == ControlLayout.WASD) {
          if (Input.GetKeyDown (KeyCode.W)) {
            input.y++;
          }
          if (Input.GetKeyDown (KeyCode.S)) {
            input.y--;
          }
          if (Input.GetKeyDown (KeyCode.D)) {
            input.x++;
          }
          if (Input.GetKeyDown (KeyCode.A)) {
            input.x--;
          }
        }
        else if (_Layout == ControlLayout.Arrows) {
          if (Input.GetKeyDown (KeyCode.UpArrow)) {
            input.y++;
          }
          if (Input.GetKeyDown (KeyCode.DownArrow)) {
            input.y--;
          }
          if (Input.GetKeyDown (KeyCode.RightArrow)) {
            input.x++;
          }
          if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            input.x--;
          }
        }
        else {
          if (Input.GetKeyDown (KeyCode.I)) {
            input.y++;
          }
          if (Input.GetKeyDown (KeyCode.K)) {
            input.y--;
          }
          if (Input.GetKeyDown (KeyCode.L)) {
            input.x++;
          }
          if (Input.GetKeyDown (KeyCode.J)) {
            input.x--;
          }
        }
        break;
    }
    return input;
  }

  void Die () {
    AudioSource.PlayClipAtPoint (_DeathSfx, transform.position);
    GameController._Instance.PlayerDeath (this);

    Instantiate (_DeathParticle, transform.position, Quaternion.identity);
    Destroy (gameObject);
  }

  // Health interface implementation
  public int GetHealth () => _Health;
  public void SetHealth (int value) { _Health = value; if (value == 0) { Die (); } }
  public void AddHealth (int amount) => _Health += amount;
  public void TakeHealth (int amount) {
    _Health -= amount;

    if (_Health <= 0) {
      Die ();
    }
    else {
      _Source.PlayOneShot (_HitSfx);
    }
  }
}
