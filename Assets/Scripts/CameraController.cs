using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour {
  [HideInInspector]
  public List<Transform> _Targets = null;
  [SerializeField] Vector2 _MinMaxY = new Vector2 (0, 10);
  [SerializeField] Vector2 _MinMaxX = new Vector2 (-5, 5);
  [SerializeField] float _PositionSmoothing = 5;

  void Update () {
    Vector2 _Midpoint = Vector2.zero;
    for (int i = 0; i < _Targets.Count; i++) {
      if (_Targets[i] != null) {
        Transform point = _Targets[i];
        _Midpoint.x += point.position.x;
        _Midpoint.y += point.position.y;
      }
    }

    _Midpoint.x /= _Targets.Count;
    _Midpoint.y /= _Targets.Count;

    // Clamp
    _Midpoint.x = Mathf.Max (_Midpoint.x, _MinMaxX.y);
    _Midpoint.x = Mathf.Min (_Midpoint.x, _MinMaxX.x);
    _Midpoint.y = Mathf.Max (_Midpoint.y, _MinMaxY.y);
    _Midpoint.y = Mathf.Min (_Midpoint.y, _MinMaxY.x);

    transform.position = Vector3.Lerp (transform.position, new Vector3 (_Midpoint.x, _Midpoint.y, -10), _PositionSmoothing * Time.deltaTime);
  }
}
