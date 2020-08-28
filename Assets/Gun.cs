using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunState
{ 
    
}


public class Gun : MonoBehaviour, IPickup
{
  PickupState _State = PickupState.Dropped;
  Player _Owner = null;

  // Start is called before the first frame update
  void Start()
  {

  }

  void OnTriggerEnter(Collider other)
  {

  }

  public void OnShoot()
  {

  }

  public void OnDrop(GameObject who)
  {
    _State = PickupState.Dropped;
    _Owner = null;
  }

  public void OnPickup(GameObject who)
  {
    _State = PickupState.PickedUp;
    _Owner = who.GetComponent<Player>();
  }

  public PickupState GetPickupState()
  {
    return _State;
  }
}
