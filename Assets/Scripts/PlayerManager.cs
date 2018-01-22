using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerManager : Photon.PunBehaviour 
    {

        #region Public Variables
 
        [Tooltip("The Beams GameObject to control")]
        public GameObject Beams;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;
        #endregion
 
        #region Private Variables
 
        //True, when the user is firing
        bool IsFiring;
 
        #endregion
 
        #region MonoBehaviour CallBacks
 
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            if (Beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                Beams.SetActive(false);
            }
 
        }
 
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            ProcessInputs ();
 
            // trigger Beams active state 
            if (Beams != null && IsFiring != Beams.GetActive()) 
            {
                Beams.SetActive(IsFiring);
            }

            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
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

        #endregion
 
        #region Custom
 
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
        #endregion
    }

}