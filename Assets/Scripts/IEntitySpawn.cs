public interface IEntitySpawn {
  // When the object is created
  void OnEnterSpawn (EntitySpawner spawner);

  // When the object has left the spawn area
  void OnLeaveSpawn ();
}
