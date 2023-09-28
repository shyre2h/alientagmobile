using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CanvasManager : MonoBehaviour
{

    public static CanvasManager instance;
    [SerializeField] TextMeshProUGUI noticeText;
    private void Awake()
    {
        instance = this;
    }

    public void UpdateNoticeText(string newText)
    {
        noticeText.text = newText;
    }
}
