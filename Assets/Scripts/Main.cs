using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Main : MonoBehaviour
{
    public ISceneManager currentSceneManager;

    public SceneLoader sceneLoder;
    public static Main Instance;
    public Canvas loadingCanvas;
    public LoginManager loginManager;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        sceneLoder.LoadScene("Title", SoloveReferences);
    }

    public void LoadingCanvas(bool value)
    {
        loadingCanvas.enabled = value;
    }

    public void LoadTitle()
    {
        currentSceneManager.OnEnd();
        sceneLoder.UnloadScene("Village");
        sceneLoder.LoadScene("Title", SoloveReferences);
    }

    public void LoadVillage()
    {
        currentSceneManager.OnEnd();
        sceneLoder.UnloadScene("Title");
        sceneLoder.LoadScene("Village", SoloveReferences);
    }

    private void SoloveReferences()
    {
        currentSceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ISceneManager>();

        currentSceneManager.SetLoginManager(loginManager);

        currentSceneManager.OnStart();
        loadingCanvas.enabled = false;
    }
}