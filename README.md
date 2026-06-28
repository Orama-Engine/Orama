# Orama
![PreRelease](https://img.shields.io/badge/Version-Pre--Release-orange) ![MIT](https://img.shields.io/badge/License-MIT-blue)

The .NET 10 C# 14 Virtual-Reality Game Engine.

> [!WARNING]
> Orama is a Pre-Release engine. It's API will have **breaking changes**.

## Features
### Virtual Reality
Orama was made specifically for Virtual Reality with no workarounds and no retrofitting.

### Extreme Modularity
All engine subsystems have their lifecycle hooked into the `Module` system. Because of this, subsystems can be arbitrarily disabled, profiled, or singled out for debugging.

```csharp
if (Input.IsKeyPressed(Key.E))
  if (ModuleManager.GetModule<PhysicsModule>()?.World.TryRaycast(Transform.Position, Transform.Forward, 1f, out RaycastResult result) == true)
    EngineOutput.Log(result.Body.Owner?.Name ?? "null");
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

### ShaderLang
```hlsl
#vertex VertexEntryPoint
#fragment FragmentEntryPoint

Name = ""Default/White""
Pass = ""Opaque""

Properties
{
    float4 Color;
}

Source
{
    struct VSInput
    {
        float3 Position : POSITION;
        float3 Normal   : NORMAL;
        float2 UV       : TEXCOORD0;
    };

    struct VSOutput
    {
        float4 pos : SV_POSITION;
    };

    VSOutput VertexEntryPoint(VSInput input)
    {
        VSOutput output;

        float4 localPos = float4(input.Position, 1.0);

        float4 worldPos = mul(ObjectMatrix, localPos);
        float4 viewPos = mul(ViewMatrix, worldPos);
        float4 clipPos = mul(ProjectionMatrix, viewPos);

        output.pos = clipPos;

        return output;
    }


    float4 FragmentEntryPoint(VSOutput input) : SV_TARGET
    {
        return Color;
    }
}
```
### Modern C#
Orama has been written to be C# 14-first. Extensive use of properties, nullability, attributes, and abstraction means code can be more explicit and less based in guess work.
