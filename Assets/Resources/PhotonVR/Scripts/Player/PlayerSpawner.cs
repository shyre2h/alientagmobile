using UnityEngine;

using Photon.Pun;

namespace Photon.VR.Player
{
    public class PlayerSpawner : MonoBehaviourPunCallbacks
    {
        [Tooltip("The location of the player prefab")]
        public string PrefabLocation = "PhotonVR/Player";
        private GameObject playerTemp;

        private void Awake() => DontDestroyOnLoad(gameObject);

        public override void OnJoinedRoom()
        {
            playerTemp = PhotonNetwork.Instantiate(PrefabLocation, Vector3.zero, Quaternion.identity);


            //Set the player's cosmetics after joining
            SetCosmetics setCosmetics;
            playerTemp.TryGetComponent<SetCosmetics>(out setCosmetics);
            if (setCosmetics)
                setCosmetics.SetDefaultCosmetics();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Destroy(playerTemp);
        }
    }

}