using System.ComponentModel.DataAnnotations;

namespace MobileBankingUSSD.API.DTOs.Requests;

public class CustomerRegistrationRequest
{
    [Required]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must contain only digits")]
    [MinLength(8, ErrorMessage = "Phone number must be at least 8 digits")]
    public string Msisdn { get; set; } = string.Empty;
    
    [Required]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN must be exactly 4 digits")]
    [RegularExpression(@"^(?!(\d)\1{3}$)\d{4}$", ErrorMessage = "PIN cannot be all the same digit")]
    public string Pin { get; set; } = string.Empty;
    
    [Required]
    public string FullName { get; set; } = string.Empty;
}