using UnityEngine;
 
 
namespace Assets.Scripts
{
    public class Launcher : Photon.PunBehaviour
    {
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        public byte MaxPlayersPerRoom = 4;
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        public GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;

        public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

        private string _gameVersion = "1";
        private bool isConnecting;

        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        void Awake()
        {
            PhotonNetwork.logLevel = Loglevel;

            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;
            
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;
        }

        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }
        
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
            isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }
 
        public override void OnConnectedToMaster()
        {
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
 
        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");        
        }

        public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
        {
            Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }
 
        public override void OnJoinedRoom()
        {
            Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }
}