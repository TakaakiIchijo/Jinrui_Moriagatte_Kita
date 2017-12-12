using UnityEngine;
using UnityEngine.Events;

public class TitleSceneManager : MonoBehaviour, ISceneManager
{
    public LoginManager loginManager;
    public TitleCanvas titleCanvas;

    public void SetLoginManager(LoginManager loginManager)
    {
        this.loginManager = loginManager;
    }

    public void OnStart()
    {
        titleCanvas.Show();

        if (loginManager.HasLocalUserName)
        {
            titleCanvas.SetStartButton("続きから", Login);
            titleCanvas.SetNameInput(false);
        }
        else
        {
            titleCanvas.SetStartButton("はじめから", SignIn);
            titleCanvas.SetNameInput(true);
        }

        Main.Instance.LoadingCanvas(false);
    }

    private void Login()
    {
        Main.Instance.LoadingCanvas(true);
        loginManager.LogIn(Main.Instance.LoadVillage);
    }

    private void SignIn()
    {
        string name = titleCanvas.GetPlayerName();

        Main.Instance.LoadingCanvas(true);
        loginManager.SignIn(name, Main.Instance.LoadVillage);
    }

    public void ShowID()
    {
        string id = loginManager.PPID;
        string pass = loginManager.PPPass;

        titleCanvas.ShowID(id, pass);
    }

    public void HideID()
    {
        titleCanvas.HideID();
    }

    public void ShowInputID()
    {
        titleCanvas.ShowInputID();
    }

    public void LoginWithInputID()
    {
        loginManager.LogInWithInputID(
                titleCanvas.InputId,
                titleCanvas.InputPass,

                () =>
                {
                    titleCanvas.HideInputID();

                    Main.Instance.LoadingCanvas(true);
                    Main.Instance.LoadVillage();
                }

        );
    }

    public void HideInputID()
    {
        titleCanvas.HideInputID();
    }

    public void DeleteAccount()
    {
        Main.Instance.LoadingCanvas(true);
        loginManager.DeleteAccount(OnStart);
    }

    public void OnEnd()
    {
        titleCanvas.Hide();
    }
}