using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The name of the room/lobby
    /// </summary>
    [Tooltip("The name of the auction room")]
    [SerializeField]
    private string roomName = "Plantion veiling";

    #region Text labels
    /// <summary>
    /// Shows the amount of players in the room
    /// </summary>
    [Tooltip("Shows the amount of players in the room")]
    [SerializeField]
    private Text playerNumbers;

    /// <summary>
    /// Shows the name the room
    /// </summary>
    [Tooltip("Shows the name the room")]
    [SerializeField]
    private Text roomNameText;

    /// <summary>
    /// Shows if the server is connected to master
    /// </summary>
    [Tooltip("Shows if the server is connected to master")]
    [SerializeField]
    private Text isConnectedText;

    /// <summary>
    /// Shows if the room is succesfuly created
    /// </summary>
    [Tooltip("Shows if the room is succesfuly created")]
    [SerializeField]
    private Text roomCreationText;
    #endregion

    #region Input fields
    /// <summary>
    /// Thie field where the host name should be entered
    /// </summary>
    [Tooltip("The field where the host name should be entered")]
    [SerializeField]
    private InputField hostField;

    /// <summary>
    /// Thie field where the max players should be entered
    /// </summary>
    [Tooltip("Thie field where the max players should be entered")]
    [SerializeField]
    private InputField maxPlayersField;
    #endregion
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private string maxPlayersPerRoom = "0";

    /// <summary>
    /// The name of the host
    /// </summary>
    [Tooltip("The name of the host")]
    [SerializeField]
    private string userName = "Host";

    void Awake()
    {
        // DIT OP TRUE ZETTEN LATER
        PhotonNetwork.AutomaticallySyncScene = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Setup();
        SetupInputfield(hostField, userName);
        SetupInputfield(maxPlayersField, maxPlayersPerRoom);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            //{
            //    Debug.Log(PhotonNetwork.PlayerList[i].NickName);
            //}
            //Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            playerNumbers.text = "Current players in room: " + PhotonNetwork.PlayerList.Length;
        }
    }

    public void ExecuteDebug()
    {
        Debug.Log("Current amount of rooms on server: " + PhotonNetwork.CountOfRooms);
        SetupRoom("TEST kamer");
    }

    /// <summary>
    /// Setups the room for use
    /// </summary>
    public void Setup()
    {
        if (PhotonNetwork.IsConnected)
            SetupRoom();
        else
        {
            PhotonNetwork.LocalPlayer.NickName = "host";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    /// <summary>
    /// Makes a room on the server with the right settings
    /// </summary>
    private void SetupRoom(string name = "Plantion Veiling")
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = byte.Parse(maxPlayersPerRoom)
        };
        PhotonNetwork.CreateRoom(name, roomOptions, null);
        Debug.Log("Created Room");
    }


    #region UI Functions
    /// <summary>
    /// Setups the input field with the data from the PlayerPref file
    /// </summary>
    /// <param name="field">The input field</param>
    /// <param name="key">The key used to find the value in PlayerPref file</param>
    private void SetupInputfield(InputField field, string key)
    {
        InputField _inputField = field;
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(key))
                _inputField.text = PlayerPrefs.GetString(key);
        }
        else
            Debug.Log("Inputfield == null");

    }

    /// <summary>
    /// Sets the name of the host client
    /// </summary>
    /// <param name="value">The name of the host client</param>
    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Host name == null");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(userName, value);
    }

    public void SetPlayerAmount(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Max players == null");
            return;
        }
        PlayerPrefs.SetString(maxPlayersPerRoom, value);
    }

    public void CloseRoom()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("Kicked " + player.NickName);
            PhotonNetwork.CloseConnection(player);
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount == 0)
            PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void RestartServer()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        isConnectedText.text = "Connected to master server: TRUE";
        SetupRoom();
    }

    public override void OnJoinedLobby()
    {

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room made with name: " + roomName);
        roomCreationText.text = "Room is created: TRUE";
        roomNameText.text = "Roomname: " + roomName;
    }

    public override void OnLeftRoom()
    {
        roomCreationText.text = "Room is created: FALSE";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomCreationText.text = "Room is created: FALSE";
    }
    #endregion
}
