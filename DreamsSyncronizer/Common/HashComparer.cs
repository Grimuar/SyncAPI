namespace DreamsSyncronizer.Common;
/// <summary>
/// Сравниватель хешей, два входных значения, на выходе получем лигическое утверждение
/// </summary>
public static class HashComparer
{
    public static bool IsEqual(string hashOne, string hashTwo)
    {
        if (hashOne.Length == hashTwo.Length)
        { 
            int i = 0;
        while ((i < hashTwo.Length) && (hashTwo[i] == hashOne[i]))
        {
            i += 1;
        }

        if (i == hashTwo.Length)
        {
            return true;
        }
        
        }

        return false;
    }
}