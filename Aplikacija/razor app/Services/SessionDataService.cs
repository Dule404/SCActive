using System;
using System.Linq;
using System.Threading.Tasks;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.Services
{
    public class SessionDataService : ISessionDataService
    {
        /// <summary>
        /// Key for session data
        /// </summary>
        private const string SessionStorageName = "MyApp";

        private SessionData RetrieveSessionData(ISession session)
        {
            SessionData data = new SessionData();
            
            // if it does not exists, create it
            if (session.Keys.Contains(SessionStorageName))
            {
                data = JsonConvert.DeserializeObject<SessionData>(session.GetString(SessionStorageName));
            }
            else
            {
                string asd = "";
            }
            return data;
        }

        public async Task<SessionData> GetSessionDataAsync(ISession session)
        {
            return await Task.Run(() => RetrieveSessionData(session));
        }
        public SessionData GetSessionData(ISession session)
        {
            return  RetrieveSessionData(session);
        }
        
        public void SetSessionData(ISession session, SessionData dataSession)
        {
            SessionData data = new SessionData(dataSession);
            session.SetString(SessionStorageName, JsonConvert.SerializeObject(data));
        }
        
        public async Task SetSessionDataAsync(ISession session, SessionData dataSession)
        {
            await Task.Run(() => SetSessionData(session,dataSession));
        }

        public void ClearSessionData(ISession session)
        {
            session.Remove(SessionStorageName);
        }

        public async Task ClearSessionDataAsync(ISession session)
        {
            await Task.Run(() => ClearSessionData(session));
        }
        
        
        
        //! caching 
        
    }
}