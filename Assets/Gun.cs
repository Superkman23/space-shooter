using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IPickup
{
  [SerializeField] GameObject _BulletObject = null;
  PickupState _State = PickupState.Dropped;

  int _BulletsLeft = 10;

  void OnTriggerEnter(Collider other)
  {
    // If the thing colliding is the player
    if (other.CompareTag("Player"))
    {
      // Trigger the pickup event
      OnPickup(other.gameObject);
    }
  }

  // Returns true if the gun is empty
  public bool Shoot()
  {
    _BulletsLeft--;
    GameObject newBullet = Instantiate(_BulletObject,transform.position, transform.parent.rotation);
    newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * 10);

    return _BulletsLeft == 0;
  }

  public void Throw()
  {
    Destroy(gameObject);
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
