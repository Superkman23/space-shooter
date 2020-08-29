public interface IHealth {
  // Returns the amount of health after adding
  void AddHealth (int amount);

  // Returns the amount of health after subtracting
  void TakeHealth (int amount);

  int GetHealth ();
  void SetHealth (int value);
}
