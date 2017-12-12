using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class TitleCanvas : MonoBehaviour
{
    public Button gameStartButton;
    public Text gameStartButtonText;
    public CanvasGroup canvasGroup;
    public InputField playerNickNameFileld;

    public CanvasGroup showIDCanvas;
    public Text idText;
    public Text passText;

    public CanvasGroup inputIDCanvas;
    public InputField idInputField;
    public InputField passInputField;

    private void Start()
    {
        HideID();
        HideInputID();
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;

        HideInputID();
        HideID();
    }

    public void ShowID(string id, string pass)
    {
        idText.text = id;
        passText.text = pass;

        showIDCanvas.alpha = 1f;
        showIDCanvas.interactable = true;
        showIDCanvas.blocksRaycasts = true;
    }

    public void HideID()
    {
        showIDCanvas.alpha = 0f;
        showIDCanvas.interactable = false;
        showIDCanvas.blocksRaycasts = false;
    }


    public void ShowInputID()
    {
        inputIDCanvas.alpha = 1f;
        inputIDCanvas.interactable = true;
        inputIDCanvas.blocksRaycasts = true;
    }

    public string InputId
    {
        get { return idInputField.text; }
    }

    public string InputPass
    {
        get { return passInputField.text; }
    }


    public void HideInputID()
    {
        inputIDCanvas.alpha = 0f;
        inputIDCanvas.interactable = false;
        inputIDCanvas.blocksRaycasts = false;
    }

    public void SetNameInput(bool isEnable)
    {
        playerNickNameFileld.gameObject.SetActive(isEnable);
    }

    public void SetStartButton(string buttonName, UnityAction onButtonEvent)
    {
        gameStartButtonText.text = buttonName;

        gameStartButton.onClick.RemoveAllListeners();
        gameStartButton.onClick.AddListener(onButtonEvent);
    }
    
    public string GetPlayerName()
    {
        var name = playerNickNameFileld.text;
        return string.IsNullOrEmpty(name) ? "Unknown": name;
    }
}