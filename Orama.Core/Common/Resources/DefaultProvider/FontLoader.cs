using Orama.Core.Modules.GUI.Resources;
using SixLabors.Fonts;
using Font = Orama.Core.Modules.GUI.Resources.Font;


namespace Orama.Core.Common.Resources.DefaultProvider;

[ResourceLoader]
internal class FontLoader : ResourceLoader<Font>
{
    /// <inheritdoc/>
    public override Font? LoadResource(byte[] data)
    {
        try
        {
            var collection = new FontCollection();

            using var ms = new MemoryStream(data);
            FontFamily family = collection.Add(ms);

            // Default to 20pt
            const int defaultSize = 19;

            var sixFont = family.CreateFont(defaultSize);

            return new Font(sixFont, defaultSize);
        }
        catch
        {
            return null;
        }
    }
}
