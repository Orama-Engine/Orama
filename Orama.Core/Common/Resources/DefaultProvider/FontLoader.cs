using SixLabors.Fonts;
using Font = Orama.Core.Modules.GUI.Resources.Font;


namespace Orama.Core.Common.Resources.DefaultProvider;

[ResourceLoader]
internal class FontLoader : ResourceLoader<Font>
{
    /// <inheritdoc/>
    public override Font? LoadResource(byte[] data)
    {
        var collection = new FontCollection();

        using var ms = new MemoryStream(data);
        FontFamily family = collection.Add(ms);

        // Default to 16pt
        const int defaultSize = 16;

        var sixFont = family.CreateFont(defaultSize);

        return new Font(sixFont, defaultSize);
    }
}
