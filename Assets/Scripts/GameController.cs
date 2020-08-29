using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
  [SerializeField] TextMeshProUGUI _WinningText = null;
  [SerializeField] CameraController _CameraController = null;

  [SerializeField] List<GameObject> _PlayerPrefabs = new List<GameObject> ();
  [SerializeField] List<Transform> _PlayerSpawnPositions = new List<Transform> ();

  public static GameController _Instance = null;
  List<Player> _Players = new List<Player> ();

  // Start is called before the first frame update
  void Start () {
    _Instance = this;

    // Assign the player numbers
    int maxPlayerCount = SettingsController._PlayerCount;
    GameObject[] players = new GameObject[maxPlayerCount];
    for (int i = 0; i < maxPlayerCount; i++)
    {
      players[i] = Instantiate(_PlayerPrefabs[0], _PlayerSpawnPositions[0].position, Quaternion.identity);
    }

    _CameraController._Targets = new List<Transform> (players.Length);
    int playerCount = 0;
    foreach (GameObject player in players) {
      Player component = player.GetComponent<Player> ();
      _Players.Add (component);
      _CameraController._Targets.Add (player.transform);
      component._PlayerNumber = playerCount++;
    }
  }

  public void PlayerDeath (Player deadPlayer) {
    _Players.Remove (deadPlayer);

    if (_Players.Count == 1) {
      EndGame (_Players[0]);
    }
  }

  bool _GameEnded = false;
  int _WinningPlayerNumber = 0;
  public void EndGame (Player winningPlayer) {
    _WinningText.gameObject.SetActive (true);
    _WinningPlayerNumber = winningPlayer._PlayerNumber;
    _GameEnded = true;
  }

  float _RestartTimer = 0;
  void Update () {
    if (_GameEnded) {
      _RestartTimer += Time.deltaTime;
      _WinningText.text = $"Player {_WinningPlayerNumber+1} won!\nRestarting in {5 - _RestartTimer:#.00}...";
      if (_RestartTimer >= 5) {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
      }
    }
  }
}
