﻿#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && ! UNITY_5_3) || UNITY_2017
#define UNITY_MIN_5_4
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        [Tooltip("The Beams GameObject to control")]
        public GameObject Beams;
        [Tooltip("The current Health of our player")]
        public float Health = 1f;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("The Player's UI GameObject Prefab")]
        public GameObject PlayerUiPrefab;

        bool IsFiring;

        void Awake()
        {
            if ( photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            DontDestroyOnLoad(this.gameObject);

            if (Beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                Beams.SetActive(false);
            }
 
        }

        void Start()
        {
            if (PlayerUiPrefab!=null)
            {
                GameObject _uiGo =  Instantiate(PlayerUiPrefab) as GameObject;
                _uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
            } else 
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.",this);
            }

            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.isMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

#if UNITY_MIN_5_4

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                this.CalledOnLevelWasLoaded(scene.buildIndex);
            };
#endif
        }




        void Update()
        {
            if (photonView.isMine)
            {
                ProcessInputs ();
            }
            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
            // trigger Beams active state 
            if (Beams != null && IsFiring != Beams.GetActive()) 
            {
                Beams.SetActive(IsFiring);
            }
        }
 
        void OnTriggerEnter(Collider other) 
        {

            if (!photonView.isMine)
            {
                return;
            }
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }
 
            Health -= 0.1f;
        }

        void OnTriggerStay(Collider other) 
        {
 
 
            // we dont' do anything if we are not the local player.
            if (!photonView.isMine) 
            {
                return;
            }
 
 
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }
 
 
            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            Health -= 0.1f*Time.deltaTime; 
        }

#if !UNITY_MIN_5_4
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.
///<summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif
 
        void CalledOnLevelWasLoaded(int level)
        {
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        void ProcessInputs()
        {
 
            if (Input.GetButtonDown("Fire1")) 
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }
 
            if (Input.GetButtonUp("Fire1")) 
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }
    }

}