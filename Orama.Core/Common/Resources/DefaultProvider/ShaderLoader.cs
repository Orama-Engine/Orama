using Orama.Core.Modules.Rendering.Resources;

namespace Orama.Core.Common.Resources.DefaultProvider;

[ResourceLoader]
internal class ShaderLoader : ResourceLoader<Shader>
{
    /// <inheritdoc/>
    public override Shader? LoadResource(byte[] data)
    {
        string source = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        return new Shader(source);
    }
}
