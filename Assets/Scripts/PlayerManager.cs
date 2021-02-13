using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Public Fields
        
        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        // [SerializeField]
        // public GameObject HeadBall;

    #endregion


    #region Private Field
        
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;
        
        bool IsFiring;

        Color randColor;

        #if UNITY_5_4_OR_NEWER
            void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
            {
                this.CalledOnLevelWasLoaded(scene.buildIndex);
            }
        #endif

    #endregion

    #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
                // stream.SendNext(randColor);
            }
            else
            {
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                // this.HeadBall.GetComponent<Renderer>().material.color = (Color)stream.ReceiveNext();
            }
        }
    #endregion

    #region MonoBehaviour Callbacks
        
        void Awake(){
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);

            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            randColor = new Color(
                Random.Range(0f, 1f), 
                Random.Range(0f, 1f), 
                Random.Range(0f, 1f)
            );
        }

        void Start(){
            #if UNITY_5_4_OR_NEWER
                // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif

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

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo =  Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
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

        #if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
            void OnLevelWasLoaded(int level)
            {
                this.CalledOnLevelWasLoaded(level);
            }
        #endif


        void CalledOnLevelWasLoaded(int level)
        {
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

        #if UNITY_5_4_OR_NEWER
            public override void OnDisable()
            {
                // Always call the base to remove callbacks
                base.OnDisable ();
                UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        #endif

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
