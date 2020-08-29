using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCapper : MonoBehaviour
{
  [SerializeField] int _MaxFPS;
  // Start is called before the first frame update
  void Start()
  {
    Application.targetFrameRate = _MaxFPS;
  }

}
