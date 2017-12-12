using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName, UnityAction endCallback, UnityAction<int> loadProgressAction = null)
    {
        StartCoroutine(LoadAsyncAdditive(sceneName, endCallback, loadProgressAction));

        //Scene.UnloadにはUnloadUnusedAssetsは含まれない
        //Awakeのタイミングでは正常に動かないっぽい
        //Activeなシーンの切替は最小限に留める
        //Activeなシーンは、「オブジェクトを生成するシーン」LighmtapやNavmesh等、シーンに紐づく設定が選択される要因

        //Single	現在読み込まれているシーンをすべて閉じてからシーンを読み込みます
        //Additive 現在読み込まれているシーンに新たなシーンを追加します
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneAsync(sceneName));
    }

    public IEnumerator UnloadSceneAsync(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);

        Resources.UnloadUnusedAssets();
    }

    private IEnumerator LoadAsyncAdditive(string sceneName, UnityAction endCallback, UnityAction<int> laodProgressAction)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        do
        {
            yield return new WaitForEndOfFrame();
            //Debug.Log("progress: " + async.progress + "isDone: " + async.isDone);
            if (laodProgressAction != null)
            {
                laodProgressAction((int)(async.progress * 100));
            }
        }
        while (async.isDone == false);

        SetLoadedSceneActive(sceneName);

        endCallback();
    }

    public bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }

    public void SetLoadedSceneActive(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid())
        {
            SceneManager.SetActiveScene(scene);
        }
    }
}