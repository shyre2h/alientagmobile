using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private Transform _environment;

    public void LoadAssetBundleMap(string location)
    {
        if (location != "Default")
        {

            AssetBundle myLoadedAssetBundle;
            if(Application.platform == RuntimePlatform.Android)
            {
                myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(location, "Android"), "mapbundle"));
            }
            else
            {
                myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(location, "Windows"), "mapbundle"));
            }

            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("map");
            EnableModMap();
            Instantiate(prefab, _environment);

            myLoadedAssetBundle.Unload(false);
        }
        else
        {
            EnableDefaultMap();
        }
    }

    private void EnableModMap()
    {
        for (int i = 0; i < _environment.childCount; i++)
        {
            if (i == 0)
            {
                _environment.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                Destroy(_environment.GetChild(i).gameObject);
            }
        }
    }

    private void EnableDefaultMap()
    {
        for(int i = 0; i<_environment.childCount; i++)
        {
            if (i == 0)
            {
                _environment.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                Destroy(_environment.GetChild(i).gameObject);
            }
        }
    }
}
