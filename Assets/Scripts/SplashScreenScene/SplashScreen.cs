using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    private bool _videoDone;

    private void Start()
    {
        _videoPlayer.loopPointReached += ChangeScene;

        _videoPlayer.Play();
        StartCoroutine(LoadScene());
    }

    private void ChangeScene(VideoPlayer source)
    {
        //SceneManager.LoadScene("MainScene");
        _videoDone = true;
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainScene");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {

            if (asyncOperation.progress >= 0.9f && _videoDone)
            {
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
