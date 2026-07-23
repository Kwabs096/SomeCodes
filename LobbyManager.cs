// This code was written by Kwabs
// Implementation of player connection to the lobby using Photon Engine


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TextMeshProUGUI _logText;
    void Start()
    {
        string currentNickName = PlayerPrefs.GetString("name");
        if (currentNickName == "")
        {
            currentNickName = "Player" + Random.Range(1000, 10000);
        }
        _usernameInputField.text = currentNickName;
        PhotonNetwork.NickName = currentNickName;


        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void ChangeNickName()
    {
        string name = _usernameInputField.text;
        PlayerPrefs.SetString("name", name);
        PhotonNetwork.NickName = name;
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to master");
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
    private void Log(string message)
    {
        _logText.text += "\n";
        _logText.text += message;
    }
}
