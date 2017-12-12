using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageSceneManager : MonoBehaviour, ISceneManager
{
    public VillageCanvas villageCanvas;
    public PlayerBase playerBase;
    public LoginManager loginManager;

    public List<TreeBase> treeBaseList = new List<TreeBase>();

    public int currentItemNum = 0;

    public void SetLoginManager(LoginManager loginManager)
    {
        this.loginManager = loginManager;
    }

    private void Awake()
    {
        villageCanvas.Hide();
        treeBaseList.ForEach(t => t.SetPlayerItemTakeCallback(TakeItem));
    }

    public void TakeItem()
    {
        loginManager.IncrementItem();
        loginManager.SaveUserData();
        currentItemNum++;

        villageCanvas.SetItemNum(currentItemNum);
    }

    public void OnStart()
    {
        currentItemNum = loginManager.GetItemNum();
        villageCanvas.SetItemNum(currentItemNum);

        villageCanvas.SetNickName(loginManager.NickName);

        villageCanvas.Show();
        playerBase.SetActive(true);
    }

    public void BackToTitle()
    {
        Main.Instance.LoadingCanvas(true);
        loginManager.LogOut(Main.Instance.LoadTitle);
    }

    public void OnEnd()
    {
        villageCanvas.Hide();
        playerBase.SetActive(false);
    }
}