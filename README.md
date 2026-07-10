# Orama
![PreRelease](https://img.shields.io/badge/Version-Pre--Release-orange) ![MIT](https://img.shields.io/badge/License-MIT-blue)

The C# 14 Virtual-Reality Game Engine built around the modern .NET ecosystem.

> [!WARNING]
> Orama is a Pre-Release engine. It's API will have **breaking changes**.

## Features
### Virtual Reality
Orama was made specifically for Virtual Reality with no workarounds and no retrofitting.

### Extreme Modularity
All engine subsystems have their lifecycle hooked into the `Module` system. Because of this, subsystems can be arbitrarily disabled, profiled, or singled out for debugging.

```csharp
if (Input.PrimaryHandLeft.IsButtonPressed(VirtualRealityController.Button.ActionUp))
  if (ModuleManager.GetModule<PhysicsModule>()?.World.TryRaycast(Transform.Position, Transform.Forward, 1f, out RaycastResult result) == true)
    EngineConsole.Log(result.Body.Owner?.Name ?? "null");
```

### Entity System
Logic is built around Entities and Components. Components are the reusable building blocks of the engine whilst Entities orchestrate their attached components to make more specific logic occur. For instance, a Button Entity would consist of `MeshRenderer`, `RigidBody`, and `Collider` components and contain logic that fires an output when an interaction ray hits the `Collider`.

### Vulkan Renderer
Rendering is built on a Veldrid-based descriptor defined Vulkan-first engine with DirectX11 and OpenGL for compatibility. 'Descriptor Defined' means all CPU/GPU heterogeneous resources are built around a 'Descriptor' that defines their data and is then mapped to the GPU sided resource, allowing every resource to reuse buffers from eachother when possible.

```csharp

/// <summary> My Custom Pass. </summary>
public class CustomRenderPass : RenderPass
{
    /// <inheritdoc/>
    public override void Render(ref RenderFrame frame)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = Renderer.AllocateCommandBuffer();

        buffer.Begin();

        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);

        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
            {
                Matrix4x4 model = renderable.Transform;
                Matrix4x4 view = frame.Camera.ViewMatrix;
                Matrix4x4 proj = frame.Camera.ProjectionMatrix;

                GPUBuffer paramBuffer = new GPUBuffer();

                paramBuffer.AddMatrix4x4(model);
                paramBuffer.AddMatrix4x4(view);
                paramBuffer.AddMatrix4x4(proj);

                buffer.QueueGPUBuffer(paramBuffer, 0);

                buffer.DrawRenderable(renderable, model);
            }

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        buffer.Dispose();
    }
}
```

### Slang Shaders
```slang
import Orama.Core;
import Orama.Attributes;

#include "Orama/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
    float4x4 ObjectMatrix;
    float4x4 ViewMatrix;
    float4x4 ProjectionMatrix;
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Parameters.ObjectMatrix, float4(i.Position, 1));
    float4 view = mul(Parameters.ViewMatrix, world);

    o.Position = mul(Parameters.ProjectionMatrix, view);

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}
```

### Modern C#
Orama has been written to be C# 14-first. Extensive use of properties, nullability, attributes, and abstraction means code can be more explicit and less based in guess work.
