using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {
  [SerializeField] float _RisingSpeed = 2.5f;

  float _Timer = 0;
  void Update () {
    transform.position = Vector3.Lerp (transform.position, transform.position + (Vector3.up * Mathf.Sin (_Timer) * 5) + Vector3.up * (_RisingSpeed * _Timer), 0.001f);
    _Timer += Time.deltaTime;
  }

  void OnCollisionEnter2D (Collision2D collision) {
    var health = collision.gameObject.GetComponent<IHealth> ();
    health?.SetHealth (0);
  }
}
