using System.Collections;
using System.Collections.Generic;
using NCMB;
using System;

namespace Communication
{
    public class NCMBDataStoreHandler : IDatabaseSystem
    {
        /// <summary>
        /// データストアにセーブ
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        /// <param name="communicationCallback"></param>
        /// <param name="addUserRelation">NCMBUserにリレーションを付与</param>
        /// <returns></returns>
        public IEnumerator SaveCoroutine(string tableName, Dictionary<string, object> data, Action<CommunicationResult> communicationCallback, bool addUserRelation = false)
        {
            bool isConnecting = true;
            CommunicationResult cr = new CommunicationResult();
            NCMBObject ncmbObject = new NCMBObject(tableName);

            foreach (KeyValuePair<string, object> pair in data)
            {
                ncmbObject.Add(pair.Key, pair.Value);
            }

            ncmbObject.SaveAsync((NCMBException e) =>
            {
                isConnecting = false;

                if (e != null)
                {
                    cr.IsSuccess = false;
                    cr.ErroMessage = e.ErrorMessage;
                    cr.ErrorCode = e.ErrorCode;
                }
                else
                {
                    cr.IsSuccess = true;

                    if (addUserRelation)
                    {
                        NCMBUser.CurrentUser.Add(tableName, ncmbObject);
                    }
                    cr.objectId = ncmbObject.ObjectId;
                }
            });

            while (isConnecting) { yield return null; }

            communicationCallback(cr);
        }

        public IEnumerator FetchCoroutine(string tableName, string objectId, Action<FetchDataResult> communicationCallback)
        {
            bool isConnecting = true;
            FetchDataResult fr = new FetchDataResult();

            NCMBObject ncmbObject = new NCMBObject(tableName);
            ncmbObject.ObjectId = objectId;
            ncmbObject.FetchAsync((NCMBException e) =>
            {
                isConnecting = false;

                if (e != null)
                {
                    fr.IsSuccess = false;
                    fr.ErroMessage = e.ErrorMessage;
                    fr.ErrorCode = e.ErrorCode;
                }
                else
                {
                    fr.IsSuccess = true;
                    fr.Data = MapDictFromNCMBObject(ncmbObject);
                }
            });

            while (isConnecting) { yield return null; }

            communicationCallback(fr);
        }

        private Dictionary<string, object> MapDictFromNCMBObject(NCMBObject ncmbObject)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            IEnumerator<string> enumerator = ncmbObject.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                data.Add(enumerator.Current, ncmbObject[enumerator.Current]);
            }

            return data;
        }

        public IEnumerator FindCoroutine(string tableName, int amount, Action<FindDataResult> communicationCallback)
        {
            bool isConnecting = true;
            FindDataResult fr = new FindDataResult();

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(tableName);
            query.Limit = amount;

            query.FindAsync((List<NCMBObject> ncmbObjectList, NCMBException e) =>
            {
                isConnecting = false;

                if (e != null)
                {
                    fr.IsSuccess = false;
                    fr.ErroMessage = e.ErrorMessage;
                    fr.ErrorCode = e.ErrorCode;
                }
                else
                {
                    fr.IsSuccess = true;

                    foreach (NCMBObject obj in ncmbObjectList)
                    {
                        fr.DatdaList.Add(MapDictFromNCMBObject(obj));
                    }
                }
            });

            while (isConnecting) { yield return null; }

            communicationCallback(fr);
        }

        public IEnumerator DeleteCoroutine(string tableName, string objectId, Action<CommunicationResult> communicationCallback)
        {
            bool isConnecting = true;
            CommunicationResult cr = new CommunicationResult();

            NCMBObject ncmbObject = new NCMBObject(tableName);
            ncmbObject.ObjectId = objectId;
            ncmbObject.DeleteAsync((NCMBException e) =>
            {
                isConnecting = false;

                if (e != null)
                {
                    cr.IsSuccess = false;
                    cr.ErroMessage = e.ErrorMessage;
                    cr.ErrorCode = e.ErrorCode;
                }
                else
                {
                    cr.IsSuccess = true;
                }
            });

            while (isConnecting) { yield return null; }

            communicationCallback(cr);
        }
    }
}