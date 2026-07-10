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

    [DefaultFloat3(0.4, -1.0, 0.3)]
    float3 SunDirection;

    [DefaultFloat(0.8f)]
    float SunIntensity;
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Object.ModelMatrix, float4(i.Position, 1));
    o.Position = mul(Camera.ProjectionMatrix, mul(Camera.ViewMatrix, world));

    o.Normal = mul((float3x3)Object.ModelMatrix, i.Normal);

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    float3 normal = normalize(i.Normal);

    float3 sunDir = normalize(Parameters.SunDirection);

    float ambient = 0.1f;

    float diffuse = max(dot(normal, -sunDir), 0.0f) * Parameters.SunIntensity;

    float lighting = ambient + diffuse;

    return float4(Parameters.Color * lighting, 1);
}