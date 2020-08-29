using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupState {
  PickedUp,
  Dropped,
}

public interface IPickup {
  // _who ("who" picked the object up)
  void OnPickup (GameObject who);

  // _who ("who" dropped the object up)
  void OnDrop (GameObject who);

  // Returns whether or not the object has been picked up
  PickupState GetPickupState ();
}
