using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerAnimatorManager : Photon.MonoBehaviour
    {
        #region Public Properties

        public float DirectionDampTime = .25f;

        #endregion

        #region Private Properties

        private Animator _animator;

        #endregion

        #region Monobehaviour Messages

        void Start ()
        {
            _animator = GetComponent<Animator>();
            if (!_animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }
	
        void Update ()
        {

            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;;
            }

            if (!_animator)
            {
                return;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Base Layer.Run"))
            {
                if (Input.GetButtonDown("Jump"))
                {
                    _animator.SetTrigger("Jump");
                }
            }

            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            if (v < 0)
            {
                v = 0;
            }

            _animator.SetFloat("Speed", h * h + v * v);

            _animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
        }
        #endregion
    }
}


