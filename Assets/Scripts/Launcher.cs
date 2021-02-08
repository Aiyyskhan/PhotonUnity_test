// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{

    #region Public Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    #endregion

    #region Private Serializable Fields

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    #endregion

    #region Private Fields

    /// <summary>
    /// This client's version number. Users are separated from each
    /// other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "0.1";

    void Awake(){
        // 
        // 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Connect();
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods

    public void Connect(){
        
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster(){
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause){
        
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message){
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. \nCalling: PhotonNetwork.CreateRoom()");
        PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom(){
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
