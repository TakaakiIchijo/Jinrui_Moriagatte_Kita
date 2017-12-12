using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Communication;

public class LoginManager : MonoBehaviour
{
    //PlayerPrefs処理はuserAuthSystemに渡さない//
    //Player Prefsに保存する生IDとパス 本番は暗号化必須//

    public readonly string PP_NCMB_USERNAME = "NCMBUserName";
    public readonly string PP_NCMB_PASSWORD = "NCMBPassWord";

    public readonly string KEY_ITEM_MIKAN = "mikanCount";
    public readonly string KEY_MONEY = "money";

    public readonly string TABLE_PUBLCPROFILE = "publicProfile";
    public readonly string KEY_NICKNAME = "nickName";
    public readonly string KEY_LEVEL = "level";//もりあがり度//
    public readonly string KEY_VILLAGEDATA = "villageData";//家具データ//

    private IUserAuthSystem userAuthSystem;
    private IDatabaseSystem databaseSystem;
    
    //データストア(Public Profile)から取得
    private string nickName;
    private int level;
    private List<Vector3> villageChairPositionList = new List<Vector3>();

    private void Awake()
    {
        userAuthSystem = new NCMBLoginHandler();
        databaseSystem = new NCMBDataStoreHandler();
    }

    public void LogIn(UnityAction endCallback)
    {
        StartCoroutine(LogInCoroutine(endCallback));
    }

    IEnumerator LogInCoroutine(UnityAction endCallback)
    {
        yield return userAuthSystem.LogInCoroutine(
            PlayerPrefs.GetString(PP_NCMB_USERNAME),
            PlayerPrefs.GetString(PP_NCMB_PASSWORD), 
            (cr) => CheckCommunicationResult(cr, null));

        string objectId = userAuthSystem.GetColumnObjectIdByString(TABLE_PUBLCPROFILE);
        //Debug.Log("objectID" + objectId);

        yield return databaseSystem.FetchCoroutine(TABLE_PUBLCPROFILE, objectId, (cr) =>
        {
            if (cr.IsSuccess)
            {
                nickName = (string)cr.Data[KEY_NICKNAME];
            }
        });

        endCallback();
    }

    public void LogInWithInputID(string userName, string password, UnityAction endCallback)
    {
        userName = userName.ToUpper();
        password = password.ToUpper();

        endCallback += () => {
            //ログイン成功したら端末保存分を書き換える//
            PlayerPrefs.SetString(PP_NCMB_USERNAME, userName);
            PlayerPrefs.SetString(PP_NCMB_PASSWORD, password);
        };

        StartCoroutine(userAuthSystem.LogInCoroutine(userName, password, (cr) => CheckCommunicationResult(cr, endCallback)));
    }

    private void CheckCommunicationResult(CommunicationResult cr, UnityAction callback)
    {
        if (cr.IsSuccess)
        {
            if(callback != null)
            {
                callback();
            }
        }
        else
        {
            Debug.Log("error: " + cr.ErroMessage);
        }
    }

    public void LogOut(UnityAction endCallback)
    {
        StartCoroutine(userAuthSystem.LogOutCoroutine((cr) => CheckCommunicationResult(cr, endCallback)));
    }

    public string PPID
    {
        get { return PlayerPrefs.HasKey(PP_NCMB_USERNAME) ? PlayerPrefs.GetString(PP_NCMB_USERNAME) : "none"; }
    }

    public string PPPass
    {
        get { return PlayerPrefs.HasKey(PP_NCMB_PASSWORD) ? PlayerPrefs.GetString(PP_NCMB_PASSWORD) : "none"; }
    }

    public string NickName
    {
        get { return nickName; }
    }

    public int GetItemNum()
    {
        return (int)userAuthSystem.GetColumnValueByInt(KEY_ITEM_MIKAN);
    }

    public void IncrementItem()
    {
        userAuthSystem.IncrementValue(KEY_ITEM_MIKAN);
    }

    public void SaveUserData(UnityAction endCallback = null)
    {
        StartCoroutine(userAuthSystem.SaveUserData(communicationCallback: (cr) => CheckCommunicationResult(cr, endCallback)));
    }

    public bool HasLocalUserName
    {
        get { return PlayerPrefs.HasKey(PP_NCMB_USERNAME); }
    }

    public string GetUserName()
    {
        return PlayerPrefs.HasKey(PP_NCMB_USERNAME) ? PlayerPrefs.GetString(PP_NCMB_USERNAME) : string.Empty;
    }

    public void SignIn(string nickName, UnityAction endCallback)
    {
        this.nickName = nickName;

        //サインイン手順//
        StartCoroutine(SignInSequence(endCallback));
    }

    private IEnumerator SignInSequence(UnityAction endCallback)
    {
        string name = "";
        //まずアカウント作成//
        yield return userAuthSystem.CreateNewUserCoroutine(((gi) =>
        {
                if (gi.IsSuccess)
                {
                    PlayerPrefs.SetString(PP_NCMB_USERNAME, gi.UserName);
                    PlayerPrefs.SetString(PP_NCMB_PASSWORD, gi.Password);
                }
                else
                {
                    Debug.Log("error: " + gi.ErroMessage);
                }
            })
        );

        Debug.Log("Name" + name);

        yield return SavePublicProfileData();

        Dictionary<string, object> userDatas = new Dictionary<string, object>() {
            { KEY_MONEY, (long)0 },
            { KEY_ITEM_MIKAN, (long)0}, //初期化をlongでやらないと初回取得の型が食い違う//
        };

        //パラメータ初期化//
        yield return userAuthSystem.SaveUserData(userDatas, (cr) => CheckCommunicationResult(cr, endCallback));
    }

    public IEnumerator SavePublicProfileData()
    {
        float[][] scores = { new float[]{ 0f, 0f, 0f }, new float[] { 0f, 0f, 0f } };
        
        //次に公開プロフィール(フレンドリスト用情報）の作成//
        Dictionary<string, object> publicProfileDatas = new Dictionary<string, object>() {
            { KEY_NICKNAME, nickName },
            { KEY_VILLAGEDATA, scores},
            { KEY_LEVEL, level }
        };

        //末尾にtrueが付いているとncmbuserへ自動的にリレーションを貼る//
        yield return databaseSystem.SaveCoroutine(TABLE_PUBLCPROFILE, publicProfileDatas, (cr) => CheckCommunicationResult(cr, null), true);
    }

    public void DeleteAccount(UnityAction endCallback)
    {
        if (PlayerPrefs.HasKey(PP_NCMB_USERNAME))
        {
            StartCoroutine(DeleteAccountCoroutine(endCallback));
        }
        else
        {
            endCallback();
        }
    }

    IEnumerator DeleteAccountCoroutine(UnityAction endCallback)
    {
        yield return userAuthSystem.LogInCoroutine(
            PlayerPrefs.GetString(PP_NCMB_USERNAME),
            PlayerPrefs.GetString(PP_NCMB_PASSWORD),
                (cr) =>
                {
                    if (!cr.IsSuccess)
                    {
                        Debug.Log("error: " + cr.ErroMessage);
                    }
                }
            );

        string objectId = userAuthSystem.GetColumnObjectIdByString(TABLE_PUBLCPROFILE);
        //Public Profileを削除//
        yield return databaseSystem.DeleteCoroutine(TABLE_PUBLCPROFILE, objectId, (cr) => CheckCommunicationResult(cr, null));

        yield return userAuthSystem.DeleteUserAsyncCoroutine((e) =>
        {
            PlayerPrefs.DeleteKey(PP_NCMB_USERNAME);
            PlayerPrefs.DeleteKey(PP_NCMB_PASSWORD);
        });

        //yield return userAuthSystem.LogOutCoroutine((cr) => CheckCommunicationResult(cr, null));

        endCallback();
    }
}