import Orama;

#include "Orama.Preprocessor.slang"

SHADER_PARAMETERS(
    float4x4 ObjectMatrix;
    float4x4 ViewMatrix;
    float4x4 ProjectionMatrix;
)

[shader("vertex")]
VSOutput Vertex(VSInput i)
{
    VSOutput o;

    float4 world = mul(Parameters.ObjectMatrix, float4(i.Position, 1));
    float4 view = mul(Parameters.ViewMatrix, world);

    o.Position = mul(Parameters.ProjectionMatrix, view);

    return o;
}

[shader("fragment")]
float4 Fragment(VSOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}