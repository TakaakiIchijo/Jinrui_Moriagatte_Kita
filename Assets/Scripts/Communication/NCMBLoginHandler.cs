using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Communication
{
    public class NCMBLoginHandler: IUserAuthSystem
    {
        public readonly int GENID_LENGTH = 4;

        public IEnumerator LogInCoroutine(string userName, string password, Action<CommunicationResult> communicationCallback)
        {
            bool isConnecting = true;
            CommunicationResult cr = new CommunicationResult();

            NCMBUser.LogInAsync(userName, password, (NCMBException e) =>
            {
                if (e != null)
                {
                    cr.ErroMessage = e.ErrorMessage;
                    cr.ErrorCode = e.ErrorCode;
                    cr.IsSuccess = false;
                }
                else
                {
                    cr.IsSuccess = true;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            if (communicationCallback != null) { communicationCallback(cr); }
        }

        public IEnumerator CreateNewUserCoroutine(Action<GeneratedIDPASSResult> communicationCallback)
        {
            bool repeat = false;
            GeneratedIDPASSResult gi = new GeneratedIDPASSResult();

            do
            {
                string userName = Utility.GenerateRandomAlphanumeric(GENID_LENGTH);
                string password = Utility.GenerateRandomAlphanumeric(GENID_LENGTH);

                IEnumerator coroutine =  NCMBSignInCoroutine(userName, password, (CommunicationResult cr) =>
                {
                    if (cr.IsSuccess)
                    {
                        repeat = false;
                        gi.IsSuccess = true;
                        gi.UserName = userName;
                        gi.Password = password;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(cr.objectId))
                        {
                            repeat = true;
                        }
                        else
                        {
                            repeat = false;
                            gi.IsSuccess = false;
                        }
                    }
                    
                });

                yield return coroutine;

            } while (repeat);

            communicationCallback(gi);
        }

        private IEnumerator NCMBSignInCoroutine(string userName, string password, Action<CommunicationResult> communicationResult)
        {
            bool isConnecting = true;
            CommunicationResult cr = new CommunicationResult();

            NCMBUser user = new NCMBUser()
            {
                UserName = userName,
                Password = password
            };

            user.SignUpAsync((NCMBException e) =>
            {
                if(e != null)
                {
                    cr.IsSuccess = false;
                    //重複エラーの場合のみの処理//
                    if (e.ErrorCode == NCMBException.DUPPLICATION_ERROR)
                    {
                        cr.objectId = "";
                    }
                }
                else
                {
                    cr.IsSuccess = true;
                    cr.objectId = user.ObjectId;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            communicationResult(cr);
        }

        public IEnumerator DeleteUserAsyncCoroutine(Action<CommunicationResult> communicationCallback)
        {
            CommunicationResult cr = new CommunicationResult();

            bool isConnecting = true;
            //NCMBからユーザーを削除//
            NCMBUser.CurrentUser.DeleteAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    cr.ErroMessage = e.ErrorMessage;
                    cr.ErrorCode = e.ErrorCode;
                    cr.IsSuccess = false;
                }

                isConnecting = false;
            });

            while (isConnecting) { yield return null; }
            
            communicationCallback(cr);
        }

        public string GetColumnValueByString(string key)
        {
            return (string)NCMBUser.CurrentUser[key];
        }

        public string GetColumnObjectIdByString(string key)
        {
            NCMBObject ncmbObject = (NCMBObject)NCMBUser.CurrentUser[key];
            return (string)ncmbObject.ObjectId;
        }

        public int GetColumnValueByInt(string key)
        {
            var count = 0;
            if (NCMBUser.CurrentUser.ContainsKey(key))
            {
                count = (int)(long)NCMBUser.CurrentUser[key];
            }

            return count;
        }

        public void IncrementValue(string key)
        {
            NCMBUser.CurrentUser.Increment(key);
        }

        public IEnumerator LogOutCoroutine(Action<CommunicationResult> communicationCallback)
        {
            bool isConnecting = true;
            CommunicationResult cr = new CommunicationResult();

            NCMBUser.LogOutAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    cr.ErroMessage = e.ErrorMessage;
                    cr.ErrorCode = e.ErrorCode;
                    cr.IsSuccess = false;
                }
                else
                {
                    cr.IsSuccess = true;
                }
                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            communicationCallback(cr);
        }

        public IEnumerator SaveUserData(Dictionary<string, object> data, Action<CommunicationResult> communicationCallback)
        {
            bool isConnecting = true;
            CommunicationResult cr = new CommunicationResult();

            if(data != null)
            {
                foreach (KeyValuePair<string, object> pair in data)
                {
                    NCMBUser.CurrentUser.Add(pair.Key, pair.Value);
                }
            }

            NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                    cr.ErroMessage = e.ErrorMessage;
                    cr.ErrorCode = e.ErrorCode;
                    cr.IsSuccess = false;
                }
                else
                {
                    cr.IsSuccess = true;
                }
                isConnecting = false;
            });

            while (isConnecting) { yield return null; }

            communicationCallback(cr);
        }
    }
}