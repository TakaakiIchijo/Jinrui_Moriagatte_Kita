using System.IO;
using UnityEditor;
using UnityEngine;

public class DebugMenu
{
    [MenuItem("Debug/DeleteLocalIDPass")]
    private static void DeleteLocalIDPass()
    {
        PlayerPrefs.DeleteKey("NCMBUserName");
        PlayerPrefs.DeleteKey("NCMBPassWord");
        Debug.Log("DeleteLocalIDPass");
    }
}