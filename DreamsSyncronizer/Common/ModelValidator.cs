using System.Text.RegularExpressions;

namespace DreamsSyncronizer.Common;

public static class ModelValidator
{
    public static bool CheckEmptyCondition(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    public static bool CheckLengthCondition(string input, int length)
    {
        return !string.IsNullOrWhiteSpace(input) && input.Length >= length;
    }

    public static bool CheckEmailFormat(string input)
    {
        return !string.IsNullOrWhiteSpace(input) && IsValidEmail(input);
    }


    #region Private Methods

    private static bool IsValidEmail(string email)
    {
        const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (string.IsNullOrEmpty(email))
            return false;

        var regex = new Regex(emailPattern);
        return regex.IsMatch(email);
    }

    #endregion
}