using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] Transform[] _Points = new Transform[2];
  [SerializeField] Vector2 _MinMaxY = new Vector2(0, 10);
  [SerializeField] Vector2 _MinMaxX = new Vector2(-5, 5);
  [SerializeField] float _PositionSmoothing = 5;
  Camera _Camera = null;

  void Awake()
  {
    _Camera = GetComponent<Camera>();
  }

  void Update()
  {
    Vector2 _Distance = Vector2.zero;
    foreach (var point in _Points)
    {
      _Distance.x += point.position.x;
      _Distance.y += point.position.y;
    }
    _Distance.x /= _Points.Length;
    _Distance.y /= _Points.Length;

    // Clamp
    _Distance.x = Mathf.Max(_Distance.x, _MinMaxX.y);
    _Distance.x = Mathf.Min(_Distance.x, _MinMaxX.x);
    _Distance.y = Mathf.Max(_Distance.y, _MinMaxY.y);
    _Distance.y = Mathf.Min(_Distance.y, _MinMaxY.x);

    transform.position = Vector3.Lerp(transform.position, new Vector3(_Distance.x, _Distance.y, -10), _PositionSmoothing * Time.deltaTime);
  }
}
