using System.Threading.Tasks;
using backend.Models.UserAuth;
using Microsoft.AspNetCore.Http;

namespace backend.Services.Interfaces
{
    public interface ISessionDataService
    {
        Task<SessionData> GetSessionDataAsync(ISession session);
        void SetSessionData(ISession session, SessionData sessionData);
        Task SetSessionDataAsync(ISession session, SessionData sessionData);
        void ClearSessionData(ISession session);
        Task ClearSessionDataAsync(ISession session);
        SessionData GetSessionData(ISession session);
    }
}