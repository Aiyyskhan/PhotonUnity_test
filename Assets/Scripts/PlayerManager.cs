using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region Private Field
        
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;
        bool IsFiring;

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

        // Update is called once per frame
        void Update()
        {
            ProcessInputs();

            if (beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }
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
