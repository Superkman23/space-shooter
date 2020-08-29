using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
  [SerializeField] List<Button> _Buttons = new List<Button> ();

  [SerializeField] Text _PlayerCountText = null;

  void Awake () {
    _PlayerCountText.text = $"Player Count {SettingsController._PlayerCount}";
  }

  void SetButtons (bool active) {
    foreach (var button in _Buttons) {
      button.enabled = active;
    }
  }

  public void GameStart () {
    SetButtons (false);
    SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
  }

  public void ChangePlayerCountButton () {
    int count = ++SettingsController._PlayerCount;
    if (count > 3) {
      SettingsController._PlayerCount = 2;
    }

    _PlayerCountText.text = $"Player Count {SettingsController._PlayerCount}";
  }
}
