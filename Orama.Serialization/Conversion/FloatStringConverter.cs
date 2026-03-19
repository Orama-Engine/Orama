
namespace Orama.Serialization.Conversion;

[StringConverter(typeof(float))]
internal class FloatStringConverter : StringConverter<float>
{
    /// <inheritdoc/>
    public override float ConvertFromString(string value) => float.Parse(value);

    /// <inheritdoc/>
    public override string ConvertToString(float value) => value.ToString();
}
