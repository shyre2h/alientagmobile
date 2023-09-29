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
        //PhotonVRManager.SetCosmetic(CosmeticType.Head, "paper-hat1");
        PhotonVRManager.SetCosmetics(new PhotonVRCosmeticsData()
        {
            Head = "VRTopHat",
            //Body = "AnarchyChain"
            //Face = "VRSunglasses",
            //LeftHand = "VRGlove",
            //RightHand = "VRGlove"
        });
    }

    //Called when a player gets infected from PlayerDetails
    public void SetInfectedCosmetics()
    {
        PhotonVRManager.SetCosmetic(CosmeticType.Head, "VRTopHat");
        // PhotonVRManager.SetCosmetics(new PhotonVRCosmeticsData()
        // {
        //     Head = "VRTopHat",
        //     Face = "VRSunglasses",
        //     LeftHand = "VRGlove",
        //     RightHand = "VRGlove"
        // });
    }

    //Toggle between different hats
    // IEnumerator ChangeCosmetic()
    // {
    //     int hatID = 1;
    //     while (hatID <= 3)
    //     {
    //         yield return new WaitForSeconds(2f);
    //         if (hatID == 1)
    //         {
    //             PhotonVRManager.SetCosmetic(CosmeticType.Head, "paper-hat1");
    //             hatID = 2;
    //         }
    //         else if (hatID == 2)
    //         {
    //             PhotonVRManager.SetCosmetic(CosmeticType.Head, "paper-hat2");
    //             hatID = 3;
    //         }
    //         else
    //         {
    //             PhotonVRManager.SetCosmetic(CosmeticType.Head, "paper-hat3");
    //             hatID = 1;
    //         }
    //     }
    // }
}
