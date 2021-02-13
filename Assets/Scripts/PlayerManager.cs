using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Public Fields
        
        [Tooltip("The current Health of our player")]
        public float Health = 1f;

    #endregion


    #region Private Field
        
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;
        bool IsFiring;

    #endregion

    #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsFiring);
            }
            else
            {
                this.IsFiring = (bool)stream.ReceiveNext();
            }
        }
    #endregion

    #region MonoBehaviour Callbacks
        
        void Awake(){
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }
        }

        void Start(){
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                this.ProcessInputs();
                if (Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }

            if (beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }
        }

        void OnTriggerEnter(Collider other){
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f;
        }

        void OnTriggerStay(Collider other){
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f*Time.deltaTime;
        }

    #endregion

    #region Custom
        
        void ProcessInputs(){
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
