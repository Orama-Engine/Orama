
namespace Orama.Serialization.Conversion;

[StringConverter(typeof(bool))]
internal class BoolStringConverter : StringConverter<bool>
{
    /// <inheritdoc/>
    public override bool ConvertFromString(string value) => value == "true";

    /// <inheritdoc/>
    public override string ConvertToString(bool value) => value ? "true" : "false";
}
