using UnityEngine;

public class PlayerGunController : MonoBehaviour {
  [Header ("Settings")]
  [SerializeField] KeyCode _ShootKey = default;
  [SerializeField] float _HoldFireSpeed = 0.3f;
  float _HoldFireTimeR;

  [HideInInspector]
  public Gun _Holding = null;

  // Update is called once per frame
  void Update () {
    _HoldFireTimeR -= Time.deltaTime;
    if (_Holding == null) {
      return;
    }
    if (Input.GetKeyDown (_ShootKey) || (Input.GetKey (_ShootKey) && _HoldFireTimeR <= 0)) {
      _HoldFireTimeR = _HoldFireSpeed;
      // Shoot the gun, then check if there are any bullets left
      if (_Holding.Shoot ()) {
        // There are no bullets left, so throw the gun
        _Holding.Throw ();
        _Holding = null;
      }
    }
  }
}
