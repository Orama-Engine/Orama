// DummyLit.slang
// An example shader that uses fake lighting data.

import Orama.Core;
import Orama.Attributes;

#include "Orama/Preprocessor.slang"

SHADER_ATTRIBUTES(
	[ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
    [DefaultFloat3(1, 1, 1)]
    float3 Color;

    [DefaultFloat3(0.2f, 0.95f, 0.15f)]
    float3 SunDirection;

    [DefaultFloat(1)]
    float SunIntensity;
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Object.ModelMatrix, float4(i.Position, 1));
    o.Position = mul(Camera.ProjectionMatrix, mul(Camera.ViewMatrix, world));

    o.Normal = i.Normal;

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    float3 normal = normalize(i.Normal);

    return float4(Parameters.Color, 1);
}