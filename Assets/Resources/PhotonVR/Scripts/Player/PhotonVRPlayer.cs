using System;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using TMPro;

using Photon.VR.Cosmetics;

namespace Photon.VR.Player
{
    public class PhotonVRPlayer : MonoBehaviourPun, IPunObservable
    {
        [Header("Objects")]
        public Transform Head;
        public Transform Body;
        public Transform LeftHand;
        public Transform RightHand;
        public Rigidbody Rigidbody;
        [Tooltip("The objects that will get the colour of the player applied to them")]
        public List<MeshRenderer> ColourObjects;


        [Header("Infection attributes")]
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public Material eyeMat;
        public Material redMat, blueMat, cyanMat, yellowMat, magentaMat, greyMat, greenMat;
        public Material infectedMat;

        [Header("Cosmetics Parents")]
        public Transform HeadCosmetics;
        public Transform FaceCosmetics;
        public Transform BodyCosmetics;
        public Transform LeftHandCosmetics;
        public Transform RightHandCosmetics;

        [Header("Other")]
        public TextMeshPro NameText;
        public bool HideLocalPlayer = true;

        // bool canPlay = false;
        //private Vector3 TargetPosition;
        //private Quaternion TargetRotation;
        public Vector3 realVel = Vector3.zero;
        public Quaternion realRotation = Quaternion.identity;
        public Vector3 realPosition = Vector3.zero;
        public double currentTime = 0.0;
        public double currentPacketTime = 0.0;
        public double lastPacketTime = 0.0;
        public double timeToReachGoal = 0.0;

        private void OnEnable()
        {
            GameManager.SetInfectedNotifier += SetInfectedListener;
        }

        private void OnDisable()
        {
            GameManager.SetInfectedNotifier -= SetInfectedListener;
        }
        private void Awake()
        {
            if (photonView.IsMine)
            {
                PhotonVRManager.Manager.LocalPlayer = this;
                if (HideLocalPlayer)
                {
                    Head.gameObject.SetActive(false);
                    Body.gameObject.SetActive(false);
                    RightHand.gameObject.SetActive(false);
                    LeftHand.gameObject.SetActive(false);
                    NameText.gameObject.SetActive(false);
                }
            }

            // It will delete automatically when you leave the room
            DontDestroyOnLoad(gameObject);

            _RefreshPlayerValues();
        }


        private void Start()
        {
            if (photonView.IsMine)
            {
                PhotonVRManager.Manager.tabletManager.photonView = photonView;
            }
        }


        private void Update()
        {
            if (photonView.IsMine)
            {
                Head.transform.position = PhotonVRManager.Manager.Head.transform.position;
                Head.transform.rotation = PhotonVRManager.Manager.Head.transform.rotation;

                RightHand.transform.position = PhotonVRManager.Manager.RightHand.transform.position;
                LeftHand.transform.position = PhotonVRManager.Manager.LeftHand.transform.position;

                /*
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    Quaternion fixRotationRight = new Quaternion(PhotonVRManager.Manager.RightHand.transform.rotation.x + 0.2f,
                        PhotonVRManager.Manager.RightHand.transform.rotation.y,
                        PhotonVRManager.Manager.RightHand.transform.rotation.z,
                        PhotonVRManager.Manager.RightHand.transform.rotation.w);

                    Quaternion fixRotationLeft = new Quaternion(PhotonVRManager.Manager.LeftHand.transform.rotation.x + 0.2f,
                        PhotonVRManager.Manager.LeftHand.transform.rotation.y,
                        PhotonVRManager.Manager.LeftHand.transform.rotation.z,
                        PhotonVRManager.Manager.LeftHand.transform.rotation.w);

                    RightHand.transform.rotation = fixRotationRight;
                    LeftHand.transform.rotation = fixRotationLeft;
                }
                else
                {
                    RightHand.transform.rotation = PhotonVRManager.Manager.RightHand.transform.rotation;
                    LeftHand.transform.rotation = PhotonVRManager.Manager.LeftHand.transform.rotation;
                }
                */

                RightHand.transform.rotation = PhotonVRManager.Manager.RightHand.transform.rotation;
                LeftHand.transform.rotation = PhotonVRManager.Manager.LeftHand.transform.rotation;
            }
            else
            {
                float distance = Vector3.Distance(transform.position, realPosition);
                float angle = Quaternion.Angle(transform.rotation, realRotation);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, realRotation, angle * Time.deltaTime * PhotonNetwork.SerializationRate);
                if (distance > 3)
                {
                    transform.position = realPosition;
                }
                else if (distance < 0.03)
                {
                    transform.position = Vector3.MoveTowards(transform.position, realPosition, 0.1f);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, realPosition, distance * Time.deltaTime * (PhotonNetwork.SerializationRate / 2));
                }
            }
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(PhotonVRManager.Manager.Rigidbody.velocity);
            }
            else
            {
                realPosition = (Vector3)stream.ReceiveNext();
                realRotation = (Quaternion)stream.ReceiveNext();
                realVel = (Vector3)stream.ReceiveNext();

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                Vector3 direction = Vector3.zero;
                if (Mathf.Abs(realVel.y) > 0.1f)
                {
                    direction = realVel;
                }
                else
                {
                    direction = new Vector3(realVel.x, 0, realVel.z);
                }
                realPosition += (direction * lag);
            }
        }

        public void RefreshPlayerValues() => photonView.RPC("RPCRefreshPlayerValues", RpcTarget.All);

        [PunRPC]
        private void RPCRefreshPlayerValues()
        {
            _RefreshPlayerValues();
        }

        private void _RefreshPlayerValues()
        {


            // Name
            if (NameText != null)
                NameText.text = photonView.Owner.NickName;

            // Cosmetics - it's a little ugly to look at
            PhotonVRCosmeticsData cosmetics = JsonUtility.FromJson<PhotonVRCosmeticsData>((string)photonView.Owner.CustomProperties["Cosmetics"]);
            if (HeadCosmetics != null)
                foreach (Transform cos in HeadCosmetics)
                    if (cos.name != cosmetics.Head)
                        cos.gameObject.SetActive(false);
                    else
                        cos.gameObject.SetActive(true);
            if (BodyCosmetics != null)
                foreach (Transform cos in BodyCosmetics.transform)
                    if (cos.name != cosmetics.Body)
                        cos.gameObject.SetActive(false);
                    else
                        cos.gameObject.SetActive(true);
            if (FaceCosmetics != null)
                foreach (Transform cos in FaceCosmetics.transform)
                    if (cos.name != cosmetics.Face)
                        cos.gameObject.SetActive(false);
                    else
                        cos.gameObject.SetActive(true);
            if (LeftHandCosmetics != null)
                foreach (Transform cos in LeftHandCosmetics.transform)
                    if (cos.name != cosmetics.LeftHand)
                        cos.gameObject.SetActive(false);
                    else
                        cos.gameObject.SetActive(true);
            if (RightHandCosmetics != null)
                foreach (Transform cos in RightHandCosmetics.transform)
                    if (cos.name != cosmetics.RightHand)
                        cos.gameObject.SetActive(false);
                    else
                        cos.gameObject.SetActive(true);



            //For colour, making sure we are not infected

            {
                // Color newColor = JsonUtility.FromJson<Color>((string)photonView.Owner.CustomProperties["Colour"]);
                Color newColor = JsonUtility.FromJson<Color>((string)photonView.Owner.CustomProperties["Colour"]);
                Material selectedMat;
                Material[] mats;

                // Debug.LogWarning("Refreshed player: " + newColor);

                // switch (newColor)
                // {
                //     case "Red": selectedMat = redMat; break;
                //     case "Blue": selectedMat = blueMat; break;
                //     case "Cyan": selectedMat = cyanMat; break;
                //     case "Yellow": selectedMat = yellowMat; break;
                //     case "Magenta": selectedMat = magentaMat; break;
                //     case "Grey": selectedMat = greyMat; break;
                //     //default is green
                //     default: selectedMat = greenMat; break;

                // }

                if (newColor == Color.red)
                    selectedMat = redMat;
                else if (newColor == Color.blue)
                    selectedMat = blueMat;
                else if (newColor == Color.cyan)
                    selectedMat = cyanMat;
                else if (newColor == Color.yellow)
                    selectedMat = yellowMat;
                else if (newColor == Color.magenta)
                    selectedMat = magentaMat;
                else if (newColor == Color.grey)
                    selectedMat = greyMat;
                else
                    selectedMat = greenMat;


                if (cosmetics.Infected == "true")
                    mats = new Material[] { infectedMat, eyeMat };
                else
                    mats = new Material[] { selectedMat, eyeMat };



                skinnedMeshRenderer.materials = mats;
            }

        }

        void SetInfectedListener(bool value)
        {
            // canPlay = value;
            if (photonView.IsMine)
            {

                //Game started
                if (value)
                {
                    //TODO: This check should be done for the initial infected player and not masterclient
                    //It is masterclient below, since we are temporarily setting the first infected as so
                    // if (PhotonNetwork.IsMasterClient)
                    //     GetComponent<PlayerDetails>().SetInfection(true);

                    if (GameManager.instance.infectedActorNumber == photonView.Owner.ActorNumber)
                        GetComponent<PlayerDetails>().SetInfection(true);

                }
                else
                {
                    GetComponent<PlayerDetails>().SetInfection(false);
                }
            }
        }
    }
}