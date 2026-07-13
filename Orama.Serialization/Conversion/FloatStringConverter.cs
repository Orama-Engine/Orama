// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization.Conversion;

[StringConverter(typeof(float))]
internal class FloatStringConverter : StringConverter<float>
{
	/// <inheritdoc/>
	public override float ConvertFromString(string value) => float.Parse(value);

	/// <inheritdoc/>
	public override string ConvertToString(float value) => value.ToString();
}
