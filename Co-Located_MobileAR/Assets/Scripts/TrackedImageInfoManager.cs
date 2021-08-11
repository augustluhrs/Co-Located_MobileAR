using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
    /// and overlays some information as well as the source Texture2D on top of the
    /// detected image.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class TrackedImageInfoManager : MonoBehaviour
    {
        //Mobile Test stuff -- haven't yet rewritten this from the ARF sample, so formatting is different from my other scripts
        private Vector3 qrPos;
        private Quaternion qrRot;
        private Vector3 phonePos;
        public GameObject phone;
        private bool hasScannedQR = false;

        [SerializeField]
        [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
        Camera m_WorldSpaceCanvasCamera;

        /// <summary>
        /// The prefab has a world space UI canvas,
        /// which requires a camera to function properly.
        /// </summary>
        public Camera worldSpaceCanvasCamera
        {
            get { return m_WorldSpaceCanvasCamera; }
            set { m_WorldSpaceCanvasCamera = value; }
        }

        ARTrackedImageManager m_TrackedImageManager;

        void Awake()
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        void OnEnable()
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable()
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        void UpdateInfo(ARTrackedImage trackedImage)
        {
            qrPos = trackedImage.transform.position;
            qrRot = trackedImage.transform.rotation;
            phonePos = phone.transform.position;
            //Debug.LogFormat("QRPos: {0}, PhonePos: {1}", qrPos, phonePos);

            if (!hasScannedQR)
            {
                //start the Launcher.Connect() process
                FindObjectOfType<CoLocated_MobileAR.Launcher>().Connect();

                Debug.LogFormat("QRPos: {0}, PhonePos: {1}",
                    qrPos,
                    phonePos);

                if (PhotonNetwork.IsConnected)
                {
                    //set my qrPos so all clients know where it is in my world space
                    Hashtable prop = new Hashtable();
                    prop.Add("anchorPos", qrPos);
                    prop.Add("anchorRot", qrRot);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

                    //set anchorPos of NetworkPosition component
                    Vector3 arCamPos = gameObject.transform.GetChild(0).transform.position;
                    Quaternion arCamRot = gameObject.transform.GetChild(0).transform.rotation;
                    GameObject clientPrefab = PhotonNetwork.Instantiate("ClientPrefab", arCamPos, arCamRot);

                    if (clientPrefab.GetComponent<PhotonView>().IsMine)
                    {
                        clientPrefab.transform.parent = gameObject.transform.GetChild(0).transform;
                        Debug.Log("the client prefab is mine");
                    }

                    Debug.LogFormat("ClientPrefab Instantiated\n\n\narCamPos: {0}, arCamRot: {1}, anchorPos: {2}",
                        arCamPos,
                        arCamRot,
                        clientPrefab.GetComponent<CoLocated_MobileAR.NetworkPosition>().anchorPos);

                    //make sure this only happens once -- TODO: make it so that if they lose tracking they can reset
                    hasScannedQR = true;
                }
            }
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                UpdateInfo(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
                UpdateInfo(trackedImage);
        }
    }
}