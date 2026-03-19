
namespace Orama.Serialization.Conversion;

[StringConverter(typeof(List<>))]
internal class ListStringConverter<T> : StringConverter<List<T>>
{
    private const char Separator = '|';

    /// <inheritdoc/>
    public override List<T> ConvertFromString(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        var converter = StringConverterAttribute.GetConverter(typeof(T));
        var method = converter.GetType().GetMethod("ConvertFromString")!;

        return value.Split(Separator).Select(s => (T)method.Invoke(converter, new object[] { s })!).ToList();
    }

    /// <inheritdoc/>
    public override string ConvertToString(List<T> value)
    {
        if (value.Count == 0)
            return string.Empty;

        var converter = StringConverterAttribute.GetConverter(typeof(T));
        var method = converter.GetType().GetMethod("ConvertToString")!;

        return string.Join(Separator, value.Select(item => (string)method.Invoke(converter, new object[] { item! })!));
    }
}