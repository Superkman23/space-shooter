using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class EntitySpawner : MonoBehaviour {
  [SerializeField] GameObject _ToSpawn = null;
  [SerializeField] bool _UseList = false;
  [SerializeField] List<Transform> _SpawnPositions = new List<Transform>();

  [SerializeField] bool _RandomSpawnTime = false;
  [SerializeField] Vector2Int _RandomSpawnTimeLimits = new Vector2Int(5, 10);

  [SerializeField] float _SpawnTime = 0;

  Random _Rng = new Random();
  bool _CanSpawn = true;
  float _Timer = 0;

  void Awake()
  {
    if (_UseList && _SpawnPositions.Count == 0)
    {
      _UseList = false;
    }
  }

  // Update is called once per frame
  void Update () {
    if (_CanSpawn) {
      _Timer += Time.deltaTime;
      if (_Timer >= _SpawnTime) {
        if (_UseList)
        {
          Instantiate (_ToSpawn, _SpawnPositions[_Rng.Next(0, _SpawnPositions.Count)].position, Quaternion.identity).GetComponent<IEntitySpawn> ().OnEnterSpawn (this);
        }
        else
        {
          Instantiate (_ToSpawn, transform.position, Quaternion.identity).GetComponent<IEntitySpawn> ().OnEnterSpawn (this);
        }

        if (_RandomSpawnTime)
        {
          _SpawnTime = (float)(_Rng.Next(_RandomSpawnTimeLimits.x, _RandomSpawnTimeLimits.y - 1) + _Rng.NextDouble());
        }

        _CanSpawn = false;
        _Timer = 0;
      }
    }
  }

  public void ObjectLeaveSpawn () {
    _CanSpawn = true;
  }
}
