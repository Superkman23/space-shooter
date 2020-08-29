using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
  [SerializeField] Player _PlayerFocus = null;
  [SerializeField] Vector2 _PositionOffset = Vector2.up;

  // Update is called once per frame
  void Update () {
    transform.position = _PlayerFocus.transform.position + new Vector3 (_PositionOffset.x, _PositionOffset.y, _PlayerFocus.transform.position.z);
  }
}
