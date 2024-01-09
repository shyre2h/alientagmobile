using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class BundleWebLoader : MonoBehaviour
{
    private string[] platforms = new string[] { Path.Combine(Application.streamingAssetsPath, "Android"), Path.Combine(Application.streamingAssetsPath, "Windows") };

    // Start is called before the first frame update
    void Start()
    {
        foreach (string platform in platforms)
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(platform, "mapbundle"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("map");
            GameObject created = Instantiate(prefab);
            created.transform.SetParent(this.transform);

            myLoadedAssetBundle.Unload(false);
        }
    }

}