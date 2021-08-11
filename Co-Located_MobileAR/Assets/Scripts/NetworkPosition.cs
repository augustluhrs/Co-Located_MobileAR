using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace CoLocated_MobileAR
{
    public class NetworkPosition : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields

        /// <summary>
        /// The position of the client that owns this component, in their relative world space.
        /// </summary>
        Vector3 networkPos;

        /// <summary>
        /// Was having trouble getting rotation to sync with AR Camera in normal Transform View, so doing this now.
        /// </summary>
        Quaternion networkRot;

        /// <summary>
        /// The vector between the client's anchor and their position, used to replicate their position in another world space.
        /// </summary>
        Vector3 offset;

        /// <summary>
        /// The relative rotation of the client to their own anchor rotation, used to replicate their rotation in our world space.
        /// </summary>
        Quaternion relativeRot;

        /// <summary>
        /// a toggle just to ensure the debug timer coroutine only starts after we have variables filled out
        /// </summary>
        bool firstPassDone = false;

        #endregion

        #region Public Fields

        /// <summary>
        /// Variable to store localPos of the anchor that clientPos should be relative to in *this* world space.
        /// For this demo, refers to the QRCode pos, but can be extrapolated to any network anchor.
        /// Is set on recognition of the image, before connection to ensure variable is set before we need it here.
        /// </summary>
        public Vector3 anchorPos;

        /// <summary>
        /// Have to store anchor rotation too to be able to have relative rotation of client prefab.
        /// </summary>
        public Quaternion anchorRot;

        /// <summary>
        /// Now setting anchor pos in called function from TrackedImageInfoManager, not running update until set
        /// </summary>
        //public bool receivedAnchorPos = false;


        #endregion

        #region IPunObservabale Callbacks

        /// <summary>
        /// Creating Custom Position Sync
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(gameObject.transform.position);
                stream.SendNext(gameObject.transform.rotation);
            }
            else
            {
                networkPos = (Vector3)stream.ReceiveNext();
                networkRot = (Quaternion)stream.ReceiveNext();
            }
        }

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// Set the anchorPos to our world space anchor pos
        /// </summary>
        void Start()
        {
            anchorPos = (Vector3)PhotonNetwork.LocalPlayer.CustomProperties["anchorPos"];
            anchorRot = (Quaternion)PhotonNetwork.LocalPlayer.CustomProperties["anchorRot"];

            Debug.LogFormat("Network Position Start \n\n AnchorPos: {0}, AnchorRot: {1}\n\n",
                anchorPos,
                anchorRot);
            //Hashtable prop = new Hashtable();
            //prop.Add("anchorPos", anchorPos);
            //PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
            //Debug.LogFormat("Controller of this object: {0}",
            //    photonView.Controller.UserId);

            //anchorPos = (Vector3)photonView.Controller.CustomProperties["anchorPos"];

        }

        /// <summary>
        /// Here we take the anchorPos of the other client in their world space,
        /// use that to find the offset vector of their current position,
        /// then apply that vector to our anchor Pos to get their position in our world space.
        /// </summary>
        void FixedUpdate()
        {
            //TODO: better way of doing this(ensure doesn't set prop until anchorPos is set) not in start?
            //if (!receivedAnchorPos)
            //{
            //    return;
            //}
            //else

            if (!photonView.IsMine)
            {
                offset = networkPos - (Vector3)photonView.Controller.CustomProperties["anchorPos"];
                gameObject.transform.position = anchorPos + offset;

                relativeRot = Quaternion.Inverse((Quaternion)photonView.Controller.CustomProperties["anchorRot"]) * networkRot;
                gameObject.transform.rotation = anchorRot * relativeRot;

                if (!firstPassDone)
                {
                    Debug.Log("starting log timer coroutine");
                    StartCoroutine( LogTimer() );
                    firstPassDone = true;
                }
            }
        }

        #endregion

        #region Public Functions

        //public void SetAnchorPos(Vector3 pos)
        //{
        //    anchorPos = pos;
        //    Hashtable prop = new Hashtable();
        //    prop.Add("anchorPos", anchorPos);
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
        //    Debug.LogFormat("Owner of this object: {0}",
        //        photonView.Owner.UserId);

        //    receivedAnchorPos = true;
        //}

        #endregion

        #region Coroutines

        IEnumerator LogTimer()
        {
            while(true)
            {
                Debug.LogFormat("NetworkPos: {0}\nTheir AnchorPos: {1}\nOur AnchorPos: {2}\nOffset: {3}\nResult: {4}",
                    networkPos,
                    (Vector3)photonView.Controller.CustomProperties["anchorPos"],
                    anchorPos,
                    offset,
                    gameObject.transform.position);
                yield return new WaitForSeconds(1f);
            }

        }

        #endregion

    }

}

