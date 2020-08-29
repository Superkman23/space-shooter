using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour {
  // Start is called before the first frame update
  void Start () {
    GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
    foreach (var player in players) {
      player.GetComponent<Player> ()._MidairJump = false;
      player.GetComponent<Rigidbody2D> ().gravityScale = 1;
    }
  }

  // Update is called once per frame
  void Update () {

  }
}
