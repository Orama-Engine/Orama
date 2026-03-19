
namespace Orama.Serialization.Conversion;

[StringConverter(typeof(Enum))]
internal class EnumStringConverter<T> : StringConverter<T> where T : struct, Enum
{
    /// <inheritdoc/>
    public override T ConvertFromString(string value) => (T)Enum.Parse(typeof(T), value);

    /// <inheritdoc/>
    public override string ConvertToString(T value) => value.ToString();
}
