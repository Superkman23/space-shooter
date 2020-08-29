using UnityEngine;

public class PlayerGunController : MonoBehaviour {
  [Header ("Settings")]
  [SerializeField] KeyCode _ShootKey = default;

  [HideInInspector]
  public Gun _Holding = null;

  // Update is called once per frame
  void Update () {
    if (_Holding != null && Input.GetKeyDown (_ShootKey)) {
      // Shoot the gun, then check if there are any bullets left
      if (_Holding.Shoot ()) {
        // There are no bullets left, so throw the gun
        _Holding.Throw ();
        _Holding = null;
      }
    }
  }
}
