using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
  SpriteRenderer _Renderer;
  [SerializeField] Sprite _RedSprite;
  [SerializeField] Sprite _BlueSprite;

  [SerializeField] float _MoveSpeed;

  void Awake()
  {
    _Renderer = GetComponent<SpriteRenderer>();
  }
  [Client]
  void FixedUpdate()
  {
    if (!hasAuthority) { return; }
    CMDSendInput(GetInput());
  }

  [Command]
  void CMDSendInput(Vector2 input)
  {
    RpcRunInput(input);
  }

  [ClientRpc]
  void RpcRunInput(Vector2 input)
  {
    transform.Translate(input * _MoveSpeed);
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
