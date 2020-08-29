using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
  [SerializeField] GameObject _ToSpawn = null;
  [SerializeField] float _SpawnTime = 0;

  float _Timer = 0;

  // Update is called once per frame
  void Update()
  {
    _Timer += Time.deltaTime;
    if (_Timer >= _SpawnTime)
    {
      Instantiate(_ToSpawn, transform.position, Quaternion.identity);
      _Timer = 0;
    }
  }
}
