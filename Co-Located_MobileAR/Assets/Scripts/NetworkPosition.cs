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

        Vector3 networkPos;

        #endregion

        #region Public Fields

        /// <summary>
        /// Variable to store localPos of the anchor that clientPos should be relative to.
        /// For this demo, refers to the QRCode pos, but can be extrapolated to any network anchor.
        /// Is set on recognition of the image, before connection to ensure variable is set before we need it here.
        /// </summary>
        public Vector3 anchorPos;

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
            }
            else
            {
                networkPos = (Vector3)stream.ReceiveNext();
            }
        }

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// Set a Custom Property of our anchorPos in our world space so other clients can access it without us constantly sending it.
        /// </summary>
        void Start()
        {
            Hashtable prop = new Hashtable();
            prop.Add("anchorPos", anchorPos);
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
            Debug.LogFormat("Owner of this object: {0}", photonView.Owner.UserId);
        }

        /// <summary>
        /// Here we take the anchorPos of the other client in their world space,
        /// use that to find the offset vector of their current position,
        /// then apply that vector to our anchor Pos to get their position in our world space.
        /// </summary>
        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                Vector3 offset = networkPos - (Vector3)photonView.Owner.CustomProperties["anchorPos"];
                gameObject.transform.position = anchorPos + offset;
            }
        }

        #endregion

    }

}

