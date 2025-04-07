using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Repositories;

public class RedisUssdSessionRepository(ICacheService cache) : IUssdSessionRepository
{
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(5);
    private const string SessionPrefix = "ussd_session_";

    public async Task<UssdSession> GetOrCreateSession(string sessionId, string msisdn)
    {
        var session = await cache.GetAsync<UssdSession>(GetSessionKey(sessionId));
            
        if (session != null)
        {
            session.LastActivity = DateTime.UtcNow;
            await UpdateSession(session);
            return session;
        }

        var newSession = new UssdSession
        {
            SessionId = sessionId,
            Msisdn = msisdn,
            CurrentMenu = string.Empty,
            LastActivity = DateTime.UtcNow,
            SessionData = new Dictionary<string, string>()
        };

        await cache.SetAsync(GetSessionKey(sessionId), newSession, _sessionTimeout);
        return newSession;
    }

    public async Task UpdateSession(UssdSession session)
    {
        session.LastActivity = DateTime.UtcNow;
        await cache.SetAsync(GetSessionKey(session.SessionId), session, _sessionTimeout);
    }

    public async Task RemoveSession(string sessionId)
    {
        await cache.RemoveAsync(GetSessionKey(sessionId));
    }

    private static string GetSessionKey(string sessionId) => $"{SessionPrefix}{sessionId}";
}