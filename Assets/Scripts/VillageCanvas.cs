using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class VillageCanvas : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text mikanItemCount;
    public Text nickNameText;

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }

    public void SetItemNum(int itemNum)
    {
        mikanItemCount.text = itemNum.ToString();
    }

    public void SetNickName(string nickName)
    {
        nickNameText.text = nickName;
    }

}
