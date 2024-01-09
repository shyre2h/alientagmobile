using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    private static string[] platforms = new string[] { "Android", "Windows" };
    
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        foreach (string platform in platforms)
        {
            string assetBundleDirectory = "Assets/StreamingAssets/"+ platform;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            if (platform == "Android")
            {
                BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            }
        }
    }
}