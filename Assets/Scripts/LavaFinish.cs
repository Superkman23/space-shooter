using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFinish : MonoBehaviour {
  private void OnTriggerEnter2D (Collider2D collision) {
    if (collision.CompareTag ("Player")) {
      var player = collision.GetComponent<Player> ();
      GameController._Instance.EndGame (player);

      for (int i = 0; i < GameController._Instance._Players.Count; i++) {
        if (GameController._Instance._Players[i] != player) {
          GameController._Instance._Players[i].Die ();
        }
      }
    }
  }
}
