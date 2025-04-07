namespace MobileBankingUSSD.API.Helpers;

public static class Validators
{
    public static bool IsValidMsisdn(string msisdn)
    {
        return !string.IsNullOrWhiteSpace(msisdn) &&
               msisdn.Length == 12 &&
               msisdn.StartsWith("233") &&
               msisdn.Substring(3).All(char.IsDigit);
    }

    public static bool IsValidPin(string pin)
    {
        var blockedPins = new HashSet<string>
        {
            "1234", "2345", "3456", "4567", "5678",
            "6789", "0123", "0000", "1111", "2222",
            "3333", "4444", "5555", "6666", "7777",
            "8888", "9999"
        };

        return !string.IsNullOrWhiteSpace(pin) &&
               pin.Length == 4 &&
               pin.All(char.IsDigit) &&
               !blockedPins.Contains(pin);
    }
}