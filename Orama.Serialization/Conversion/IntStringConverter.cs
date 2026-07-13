// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization.Conversion;

[StringConverter(typeof(int))]
internal class IntStringConverter : StringConverter<int>
{
	/// <inheritdoc/>
	public override int ConvertFromString(string value) => int.Parse(value);

	/// <inheritdoc/>
	public override string ConvertToString(int value) => value.ToString();
}
