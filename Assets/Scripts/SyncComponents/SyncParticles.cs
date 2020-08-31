using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SyncParticles : NetworkBehaviour
{
	[SerializeField] ParticleSystem _ParticleSystem = null;

  [SyncVar(hook = nameof(SyncActive))] bool _IsEnabled = false;

  [Command]
  public void SetActive(bool active)
  {
    _IsEnabled = active;
  }


  void SyncActive(bool oldBool, bool newBool)
  {
    if (newBool)
    {
      _ParticleSystem.Play();
    }
    else
    {
      _ParticleSystem.Stop();
    }
  }

}
