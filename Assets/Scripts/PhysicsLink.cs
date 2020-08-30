using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PhysicsLink : NetworkBehaviour {
  public Rigidbody2D _Rigidbody;
  [SyncVar] public Vector2 _Position;
  [SyncVar] public Vector2 _Velocity;
  [SyncVar] public float _AngularVelocity;
  [SyncVar] public float _Rotation;

  void FixedUpdate () {
    if (isServer) {
      _Position = _Rigidbody.position;
      _Rotation = _Rigidbody.rotation;
      _Velocity = _Rigidbody.velocity;
      _AngularVelocity = _Rigidbody.angularVelocity;

      _Rigidbody.position = _Position;
      _Rigidbody.rotation = _Rotation;
      _Rigidbody.velocity = _Velocity;
      _Rigidbody.angularVelocity = _AngularVelocity;
    }
    else // Not server so must be client
    {
      _Rigidbody.position = _Position + _Velocity * (float) NetworkTime.rtt;
      _Rigidbody.rotation = _Rotation * _AngularVelocity * (float) NetworkTime.rtt;
      _Rigidbody.velocity = _Velocity;
      _Rigidbody.angularVelocity = _AngularVelocity;
    }
  }

  [Command] //function that runs on server when called by a client
  public void CmdResetPose () {
    _Rigidbody.position = new Vector3 (0, 1, 0);
    _Rigidbody.velocity = new Vector3 ();
  }
  public void ApplyForce (Vector2 force, ForceMode2D FMode) //apply force on the client-side to reduce the appearance of lag and then apply it on the server-side
  {
    _Rigidbody.AddForce (force, FMode);
    CmdApplyForce (force, FMode);
  }
  public void AddDirectForce (Vector2 force) //apply force on the client-side to reduce the appearance of lag and then apply it on the server-side
  {
    if (!hasAuthority) { return; }
    _Rigidbody.velocity += force;
    CmdAddDirectForce (force);
  }

  [Command]
  public void CmdApplyForce (Vector2 force, ForceMode2D FMode) {
    _Rigidbody.AddForce (force, FMode); //apply the force on the server side
  }

  [Command]
  public void CmdAddDirectForce (Vector2 force) {
    _Rigidbody.velocity += force;
  }

}
