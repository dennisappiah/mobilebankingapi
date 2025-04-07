using MobileBankingUSSD.API.DTOs.Requests;
using MobileBankingUSSD.API.DTOs.Responses;

namespace MobileBankingUSSD.API.Services.interfaces;

public interface IUssdService
{
    Task<UssdResponse> ProcessRequest(UssdRequest request);
}