using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject _loadingPanel, _roomJoiningPanel, _waitingRoomPanel, _nameItemprefab;
    [SerializeField] TMP_InputField _playerName, _roomCode;
    [SerializeField] TMP_Text _roomMessage, _roomNameAndCreater, _warning;
    public Transform _nameContentObj;
    bool _join = true, _create = true, _connected;

    private void Start()
    {
        if (PlayerPrefs.HasKey("exit"))
        {
            if (PlayerPrefs.GetInt("exit") == 1)
            {
                _loadingPanel.SetActive(false);
                _roomJoiningPanel.SetActive(true);
                _connected = true;
                PlayerPrefs.SetInt("exit", 0);
            }
        }
        else
        {
            PlayerPrefs.SetInt("exit", 0);
        }
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectToBestCloudServer();
        _playerName.onSelect.AddListener(NameSelect);
        _playerName.onDeselect.AddListener(NameDeselect);
        _roomCode.onSelect.AddListener(RoomSelectJoinCode);
        _roomCode.onDeselect.AddListener(RoomDeselectJoinCode);
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (!_connected)
            {
                if (!PhotonNetwork.InLobby && PhotonNetwork.CurrentLobby == null)
                {
                    PhotonNetwork.JoinLobby();
                }
                if (PhotonNetwork.InLobby)
                {
                    _loadingPanel.SetActive(false);
                    _roomJoiningPanel.SetActive(true);
                    _connected = true;
                }
                if (_playerName.text.Length > 1)
                {
                    PhotonNetwork.LocalPlayer.NickName = _playerName.text;
                }
            }
        }
    }

    void NameSelect(string i)
    {
        if (i.Length < 1)
        {
            _playerName.text = " ";
        }
    }

    void NameDeselect(string i)
    {
        if (i == " ")
        {
            _playerName.text = "";
        }
    }

    void RoomSelectJoinCode(string i)
    {
        if (i.Length < 1)
        {
            _roomCode.text = " ";
        }
    }

    void RoomDeselectJoinCode(string i)
    {
        if (i == " ")
        {
            _roomCode.text = "";
        }
    }

    public void JoinRoom()
    {
        if (_join)
        {
            if (PhotonNetwork.JoinRoom(_roomCode.text))
            {
                _join = false;
            }
        }
        if (!_join)
        {
            Invoke("canjoinfalse", 0.5f);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (_playerName.text.Length > 1)
        {
            PhotonNetwork.LocalPlayer.NickName = _playerName.text;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        }
        _connected = true;
        PhotonNetwork.KeepAliveInBackground = 30000000;
        _roomNameAndCreater.text = "Room: '" + PhotonNetwork.CurrentRoom.Name + "' Created by: '" + PhotonNetwork.MasterClient.NickName + "'";
        _roomJoiningPanel.SetActive(false);
        _waitingRoomPanel.SetActive(true);
        Dictionary<int, Player> i = PhotonNetwork.CurrentRoom.Players;
        foreach (Player player in i.Values)
        {
            GameObject text = Instantiate(_nameItemprefab, _nameContentObj);
            text.name = player.UserId;
            text.GetComponent<TMP_Text>().text = player.NickName;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        GameObject[] nameobj = GameObject.FindGameObjectsWithTag("Name");
        foreach (GameObject game in nameobj)
        {
            Destroy(game);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        _roomMessage.text = "Room doesn't exist...";
        Invoke("messagenull", 5f);
    }

    public void CreateRoom()
    {
        if (_roomCode.text.Length > 2)
        {
            if (_create)
            {
                if (!PhotonNetwork.CreateRoom(_roomCode.text, new RoomOptions() { MaxPlayers = 2 }))
                {
                    _create = true;
                    Invoke("joinfalse", 0.5f);
                    _roomMessage.text = "Room Code is already in use...";
                    Invoke("messagenull", 5f);
                    if (_playerName.text.Length > 1)
                    {
                        PhotonNetwork.LocalPlayer.NickName = _playerName.text;
                    }
                    else
                    {
                        PhotonNetwork.LocalPlayer.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber.ToString();
                    }
                }
            }
        }
        else
        {
            _roomMessage.text = "Room name Length should be more than 2...";
            Invoke("messagenull", 5f);
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        if (_playerName.text.Length > 1)
        {
            PhotonNetwork.LocalPlayer.NickName = _playerName.text;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        }
        _connected = true;
        _roomNameAndCreater.text = "Room: '" + PhotonNetwork.CurrentRoom.Name + "' Created by: '" + PhotonNetwork.MasterClient.NickName + "'";
        _create = false;
        _roomJoiningPanel.SetActive(false);
        _waitingRoomPanel.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void MainMenuRoomLeft()
    {
        SceneManager.LoadScene(0);
        PhotonNetwork.LeaveRoom();
    }

    public void RoomExit()
    {
        _warning.text = "";
        _playerName.text = "";
        _roomCode.text = "";
        foreach (GameObject Name in GameObject.FindGameObjectsWithTag("Name"))
        {
            Destroy(Name);
        }
        if (PhotonNetwork.LeaveRoom())
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            if (PhotonNetwork.ConnectUsingSettings())
            {
                PhotonNetwork.JoinLobby();
                _roomJoiningPanel.SetActive(true);
                _create = true;
                _waitingRoomPanel.SetActive(false);
            }
        }
    }

    public void startClicked()
    {
        if (PhotonNetwork.LocalPlayer.UserId == PhotonNetwork.MasterClient.UserId && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(1);
        }
        else if(PhotonNetwork.LocalPlayer.UserId != PhotonNetwork.MasterClient.UserId)
        {
            _warning.text = "Only room creater can start the game...";
            Invoke("Warningnull", 2f);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            _warning.text = "Two Players are required...";
            Invoke("Warningnull", 2f);
        }
    }

    void messagenull()
    {
        _roomMessage.text = "";
    }

    void Warningnull()
    {
        _warning.text = "";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        GameObject[] nameobj = GameObject.FindGameObjectsWithTag("Name");
        foreach (GameObject game in nameobj)
        {
            Destroy(game);
        }
        Dictionary<int, Player> i = PhotonNetwork.CurrentRoom.Players;
        foreach (Player player in i.Values)
        {
            GameObject text = Instantiate(_nameItemprefab, _nameContentObj);
            text.name = player.UserId;
            if (player.NickName == "")
            {
                text.GetComponent<TMP_Text>().text = "Player" + player.ActorNumber.ToString();
            }
            else
            {
                text.GetComponent<TMP_Text>().text = player.NickName;
            }
        }
        _roomNameAndCreater.text = "Room: '" + PhotonNetwork.CurrentRoom.Name + "' Created by: '" + PhotonNetwork.MasterClient.NickName + "'";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GameObject[] nameobj = GameObject.FindGameObjectsWithTag("Name");
        foreach (GameObject game in nameobj)
        {
            Destroy(game);
        }
        Dictionary<int, Player> i = PhotonNetwork.CurrentRoom.Players;
        foreach (Player player in i.Values)
        {
            GameObject text = Instantiate(_nameItemprefab, _nameContentObj);
            text.name = player.UserId;
            text.GetComponent<TMP_Text>().text = player.NickName;
        }
    }

    void canjoinfalse()
    {
        _join = true;
    }

    void joinfalse()
    {
        _create = true;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("exit", 0);
    }
}
