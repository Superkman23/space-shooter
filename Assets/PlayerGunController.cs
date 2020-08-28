using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField] KeyCode _ShootKey = default;

  Gun _Holding = null;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (_Holding != null && Input.GetKeyDown(_ShootKey))
    {

    }
  }

  public void PickupGun(Gun pickedUp)
  {
    _Holding = pickedUp;
  }
}
