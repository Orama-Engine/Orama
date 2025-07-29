using Orama.Resources.ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orama.Resources;

internal class TextAsset : IResource<TextAsset>
{
	public string Content { get; set; } = "";

	public TextAsset Deserialize(Stream stream)
	{
		using var reader = new StreamReader(stream);
		return new TextAsset
		{
			Content = reader.ReadToEnd()
		};
	}

	public void Serialize(Stream stream)
	{
		using var writer = new StreamWriter(stream);
		writer.Write(Content);
		writer.Flush();
	}
}
