using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunState
{ 
    
}


public class Gun : MonoBehaviour, IPickup
{
  PickupState _State = PickupState.Dropped;

  // Start is called before the first frame update
  void Start()
  {

  }

  void OnTriggerEnter(Collider other)
  {
    // If the thing colliding is the player
    if (other.CompareTag("Player"))
    {
      // Trigger the pickup event
      OnPickup(other.gameObject);
    }
  }

  public void OnShoot()
  {

  }

  public void OnDrop(GameObject who)
  {
    _State = PickupState.Dropped;
  }

  public void OnPickup(GameObject who)
  {
    _State = PickupState.PickedUp;
  }

  public PickupState GetPickupState()
  {
    return _State;
  }
}
