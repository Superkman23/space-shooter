using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class MainMenu : NetworkBehaviour
{
  [SerializeField] TMP_InputField _NameInput;
  [SerializeField] Button _HostButton;
  [SerializeField] Button _JoinButton;
  [SerializeField] Button _HostJoinButton;

  [SerializeField] ShooterNetworkManager _Manager;

  void HostServer()
  {
    _Manager.StartHost();
  }
}
