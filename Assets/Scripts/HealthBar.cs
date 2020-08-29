using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
  [SerializeField] Player _PlayerFocus = null;
  [SerializeField] Vector2 _PositionOffset = Vector2.up;
  Image _Image;

  private void Awake()
  {
    _Image = GetComponent<Image>();
  }


  // Update is called once per frame
  void Update () {
    transform.position = _PlayerFocus.transform.position + new Vector3 (_PositionOffset.x, _PositionOffset.y, _PlayerFocus.transform.position.z);
  }
}
