using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerAnimatorManager : Photon.MonoBehaviour
    {
        public float DirectionDampTime = .25f;

        private Animator animator;

        void Start ()
        {
            animator = GetComponent<Animator>();
            if (!animator)
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

            if (!animator)
            {
                return;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Base Layer.Run"))
            {
                if (Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("Jump");
                }
            }

            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            if (v < 0)
            {
                v = 0;
            }

            animator.SetFloat("Speed", h * h + v * v);

            animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
        }
    }
}


