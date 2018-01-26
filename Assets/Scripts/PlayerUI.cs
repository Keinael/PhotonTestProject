using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts;


namespace Assets.Scripts
{
    public class PlayerUI : MonoBehaviour 
    {
        [Tooltip("Height of the HP bar")]
        public float HPBarHeight;

        [Tooltip("UI Text to display Player's Name")]
        public Text PlayerNameText;
 
        [Tooltip("UI Slider to display Player's Health")]
        public Slider PlayerHealthSlider;
 
        [Tooltip("Pixel offset from the player target")]
        public Vector3 ScreenOffset = new Vector3(0f,30f,0f);

        private PlayerManager _target;

        float _characterControllerHeight = 0f;
        Transform _targetTransform;
        Vector3 _targetPosition;

        void Awake()
        {
            this.GetComponent<Transform>().SetParent (GameObject.Find("Canvas").GetComponent<Transform>());
        }

        void Update()
        {
            // Reflect the Player Health
            if (PlayerHealthSlider != null) 
            {
                PlayerHealthSlider.value = _target.Health;
            }

            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
            if (_target == null) 
            {
                Destroy(this.gameObject);
                return;
            }
        }

        public void SetTarget(PlayerManager target)
        {
            if (target == null) 
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.",this);
                return;
            }
            // Cache references for efficiency
            _target = target;

            CharacterController _characterController = _target.GetComponent<CharacterController> ();

            if (_characterController != null)
            {
                _characterControllerHeight = _characterController.height;
            }

            if (PlayerNameText != null) 
            {
                PlayerNameText.text = _target.photonView.owner.NickName;
            }
        }

        void LateUpdate() 
        {

            if (_target.transform != null)
            {
                _targetPosition = _target.transform.position;
                _targetPosition.y += _characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint (_targetPosition) + ScreenOffset;
            }
        }
    }
}