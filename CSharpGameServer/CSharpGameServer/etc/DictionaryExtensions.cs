public static class DictionaryExtensions
{
    public static TValue? GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
    {
        if (dictionary.TryGetValue(key, out TValue value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }
}