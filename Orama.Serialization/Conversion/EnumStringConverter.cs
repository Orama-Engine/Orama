// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization.Conversion;

[StringConverter(typeof(Enum))]
internal sealed class EnumStringConverter<T> : StringConverter<T> where T : struct, Enum
{
	/// <inheritdoc/>
	public override T ConvertFromString(string value) => (T)Enum.Parse(typeof(T), value);

	/// <inheritdoc/>
	public override string ConvertToString(T value) => value.ToString();
}
