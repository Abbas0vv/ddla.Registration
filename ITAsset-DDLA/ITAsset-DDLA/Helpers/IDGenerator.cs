namespace ddla.ITApplication.Helpers;

public static class IDGenerator
{
    public static string GenerateId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 9)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }
}
