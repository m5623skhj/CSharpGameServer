namespace CSharpGameServer.etc;

public static class DictionaryExtensions
{
    public static TValue? GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue?> dictionary, TKey key) where TValue : class where TKey : notnull
    {
        return dictionary.GetValueOrDefault(key);
    }
}