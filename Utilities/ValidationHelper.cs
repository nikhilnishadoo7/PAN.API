// Utilities/ValidationHelper.cs
using System.Text.RegularExpressions;

namespace PAN.API.Utilities;

public static class ValidationHelper
{
    public static void ValidatePan(string pan)
    {
        if (!Regex.IsMatch(pan, "^[A-Z]{5}[0-9]{4}[A-Z]$"))
            throw new Exception("Invalid PAN");
    }
}