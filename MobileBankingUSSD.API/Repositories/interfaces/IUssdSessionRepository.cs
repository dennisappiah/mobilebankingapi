using MobileBankingUSSD.API.DTOs.Requests;

namespace MobileBankingUSSD.API.Repositories.interfaces;

public interface IUssdSessionRepository
{
    Task<UssdSession> GetOrCreateSession(string sessionId, string msisdn);
    Task UpdateSession(UssdSession session);
    Task RemoveSession(string sessionId);
}