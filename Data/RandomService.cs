namespace Linker.Data;


public class RandomService:IRandomService
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private readonly Random _random;
    public RandomService()
    {
        _random = new Random();
    }

    public string GetRandomUrl(int length = 10)
    {
        var x = Enumerable.Repeat(chars, length)
                      .Select(s => s[_random.Next(s.Length)]);
        var randomString = new string (
            Enumerable.Repeat(chars, length)
                      .Select(s => s[_random.Next(s.Length)])
                      .ToArray());

        return randomString;
    }
}
