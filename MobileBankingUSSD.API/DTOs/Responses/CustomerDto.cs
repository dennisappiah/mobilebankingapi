namespace MobileBankingUSSD.API.DTOs.Responses;


public class CustomerDto
{
        public Guid Id { get; set; }
        public string Msisdn { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }

}

