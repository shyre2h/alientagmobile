using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    [SerializeField] Transform blinkUpper;
    [SerializeField] Transform blinkLower;
    float initialPosUpper;
    float targetedPosUpper = -0.1f;
    float blinkTime = 0.5f;
    float blinkTime2 = 0.5f;
    bool performingBlink = false;
    public static Blink instance;
    public static Action<bool> BlinkNotifier;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        initialPosUpper = blinkUpper.localPosition.y;

        //Make sure eyes are closed on start, will open when you join a lobby
        blinkUpper.DOLocalMoveY(targetedPosUpper, 0f);
        blinkLower.DOLocalMoveY(-targetedPosUpper, 0f);
    }


    public void CloseEyes(bool value, Action action)
    {

        if (performingBlink) return;
        DOTween.Kill(blinkUpper);
        DOTween.Kill(blinkLower);
        BlinkNotifier?.Invoke(value);
        if (value)
        {

            blinkUpper.gameObject.SetActive(true);
            blinkLower.gameObject.SetActive(true);

            blinkUpper.DOLocalMoveY(targetedPosUpper, blinkTime);
            blinkLower.DOLocalMoveY(-targetedPosUpper, blinkTime).OnComplete(() => action?.Invoke());
        }
        else
        {
            blinkUpper.DOLocalMoveY(initialPosUpper, blinkTime);
            blinkLower.DOLocalMoveY(-initialPosUpper, blinkTime).OnComplete(() =>
            {
                action?.Invoke();


                blinkUpper.gameObject.SetActive(false);
                blinkLower.gameObject.SetActive(false);
            });
        }
    }




    public void PerformBlinkSequence(Action action)
    {
        performingBlink = true;
        DOTween.Kill(blinkUpper);
        DOTween.Kill(blinkLower);
        blinkUpper.gameObject.SetActive(true);
        blinkLower.gameObject.SetActive(true);

        blinkUpper.DOLocalMoveY(targetedPosUpper, blinkTime);
        blinkLower.DOLocalMoveY(-targetedPosUpper, blinkTime).OnComplete(() =>
        {
            blinkUpper.DOLocalMoveY(initialPosUpper, blinkTime);
            blinkLower.DOLocalMoveY(-initialPosUpper, blinkTime).OnComplete(() =>
            {
                blinkUpper.DOLocalMoveY(targetedPosUpper, blinkTime);
                blinkLower.DOLocalMoveY(-targetedPosUpper, blinkTime).OnComplete(() =>
                   {
                       blinkUpper.DOLocalMoveY(initialPosUpper, blinkTime);
                       blinkLower.DOLocalMoveY(-initialPosUpper, blinkTime).OnComplete(() =>
                       {
                           blinkUpper.gameObject.SetActive(false);
                           blinkLower.gameObject.SetActive(false);
                           performingBlink = false;
                           action?.Invoke();
                       });
                   });
            });

        });
    }

    IEnumerator PerformBlinkCo()
    {
        yield return null;
    }
}