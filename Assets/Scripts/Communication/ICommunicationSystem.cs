using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Communication
{
    public interface IUserAuthSystem
    {
        string GetColumnValueByString(string key);
        int GetColumnValueByInt(string key);
        string GetColumnObjectIdByString(string key);

        void IncrementValue(string key);

        IEnumerator LogInCoroutine(string userName, string password, Action<CommunicationResult> communicationCallback);
        IEnumerator LogOutCoroutine(Action<CommunicationResult> communicationCallback);
        IEnumerator CreateNewUserCoroutine(Action<GeneratedIDPASSResult> communicationCallback);
        IEnumerator SaveUserData(Dictionary<string, object> datas = null, Action<CommunicationResult> communicationCallback=null);
        IEnumerator DeleteUserAsyncCoroutine(Action<CommunicationResult> communicationCallback);
    }

    public interface IDatabaseSystem
    {
        IEnumerator SaveCoroutine(string tableName, Dictionary<string, object> data, Action<CommunicationResult> communicationCallback, bool addUserRelation);
        IEnumerator FindCoroutine(string tableName, int amount, Action<FindDataResult> communicationCallback);
        IEnumerator FetchCoroutine(string tableName, string objectId, Action<FetchDataResult> communicationCallback);
        IEnumerator DeleteCoroutine(string tableName, string objectId, Action<CommunicationResult> communicationCallback);
    }

    public class GeneratedIDPASSResult: CommunicationResult
    {
        public string UserName = "";
        public string Password = "";
    }

    public class FindDataResult : CommunicationResult
    {
        public List<Dictionary<string, object>> DatdaList = new List<Dictionary<string, object>>();
    }

    public class FetchDataResult : CommunicationResult
    {
        public Dictionary<string, object> Data;
    }

    public class CommunicationResult
    {
        public bool IsSuccess = false;
        public string ErroMessage = "";
        public string ErrorCode;
        public string objectId;
    }

}
