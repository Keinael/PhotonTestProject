using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameManager : Photon.PunBehaviour 
    {
        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        
        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);

                LoadArena();
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected isMasterClient " + PhotonNetwork.isMasterClient);

            LoadArena();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("Room for" + PhotonNetwork.room.PlayerCount);
        }
        
        void Start()
        {
            Instance = this;

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance==null)
                {
                    Debug.Log("We are Instantiating LocalPlayer from "+SceneManager.GetActiveScene().name);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.Log("Ignoring scene load for "+ SceneManager.GetActiveScene().name);
                }
            }
        }

    }
}


