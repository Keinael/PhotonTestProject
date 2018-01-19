using UnityEngine;

namespace Assets.Scripts
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Public Variables
 
        /// <summary>
        /// The PUN loglevel
        /// </summary>
        public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players,
        /// and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.")]
        public byte MaxPlayersPerRoom = 4;

        #endregion

        #region Private Variables

        /// <summary>
        /// This client's version number.
        /// </summary>
        private string _gameVersion = "1";

        #endregion
        
        #region MonoBehaviour CallBacks

        void Awake()
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.logLevel = LogLevel;
        }

        void Start()
        {
            
        }
        
        #endregion

        #region Public Methods

        public void Connect()
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #endregion

        #region Photon.PunBehaviour CallBacks
 
        public override void OnConnectedToMaster()
        {
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()  
            PhotonNetwork.JoinRandomRoom();
        }
 
        public override void OnDisconnectedFromPhoton()
        {
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
        }

        #endregion
        
    }
}
