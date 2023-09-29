using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
using Photon.VR.Cosmetics;


public class SetCosmetics : MonoBehaviour
{
    //Called when a player joins a room from PlayerSpawner script
    //PhotonVRManager handles RPC
    public void SetDefaultCosmetics()
    {
        // PhotonVRManager.SetCosmetic(CosmeticType.Head, "paper-hat1");

        // PhotonVRManager.SetCosmetic(CosmeticType.Infected, "true");

        PhotonVRManager.SetCosmetics(new PhotonVRCosmeticsData()
        {
            //Head = "paper-hat1",
            // Body = "null",
            Face = "anarchy-chain"
            //LeftHand = "VRGlove",
            //RightHand = "VRGlove"
            // Infected = "true"
        });
    }

    //Called when a player gets infected from PlayerDetails
    public void SetInfectedCosmetics()
    {
        PhotonVRManager.SetCosmetic(CosmeticType.Infected, "true");
    }


}
