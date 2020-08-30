﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhysicsLink : NetworkBehaviour
{
  public Rigidbody2D _Rigidbody;
  [SyncVar] public Vector2 _Position;
  [SyncVar] public Vector2 _Velocity;
  [SyncVar] public float _AngularVelocity;
  [SyncVar] public float _Rotation;

  void FixedUpdate()
  {
    if (isServer)
    {
      _Position = _Rigidbody.position;
      _Rotation = _Rigidbody.rotation;
      _Velocity = _Rigidbody.velocity;
      _AngularVelocity = _Rigidbody.angularVelocity;

      _Rigidbody.position = _Position;
      _Rigidbody.rotation = _Rotation;
      _Rigidbody.velocity = _Velocity;
      _Rigidbody.angularVelocity = _AngularVelocity;
    }
    if (isClient) 
    {
      _Rigidbody.position = _Position;
      _Rigidbody.rotation = _Rotation;
      _Rigidbody.velocity = _Velocity;
      _Rigidbody.angularVelocity = _AngularVelocity;
    }
  }

  public void ApplyForce(Vector2 force, ForceMode2D FMode) //apply force on the client-side to reduce the appearance of lag and then apply it on the server-side
  {
    if (!hasAuthority) { return; }
    _Rigidbody.AddForce(force, FMode);
    CmdApplyForce(force, FMode);
  }

  public void AddDirectForce(Vector2 force) //Directly edit the rigidbody's velocity
  {
    if (!hasAuthority) { return; }
    _Rigidbody.velocity += force;
    CmdAddDirectForce(force);
  }

  public void SetVelocity(Vector2 velocity)
  {
    if (!hasAuthority) { return; }
    _Rigidbody.velocity = velocity;
    CmdSetVelocity(velocity);
  }

  #region Commands

  [Command]
  public void CmdApplyForce(Vector2 force, ForceMode2D FMode)
  {
    _Rigidbody.AddForce(force, FMode); //apply the force on the server side
  }

  [Command]
  public void CmdAddDirectForce(Vector2 force)
  {
    _Rigidbody.velocity += force;
  }

  [Command]
  public void CmdSetVelocity(Vector2 velocity)
  {
    _Rigidbody.velocity = velocity;
  }

  [Command]
  public void CmdResetPose()
  {
    _Rigidbody.position = new Vector3(0, 1, 0);
    _Rigidbody.velocity = new Vector3();
  }
  #endregion
}
