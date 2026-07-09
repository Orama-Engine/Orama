import Orama;

ParameterBlock<GlobalData> Globals;

[shader("vertex")]
VSOutput Vertex(VSInput i)
{
    VSOutput o;

    float4 world = mul(Globals.ObjectMatrix, float4(i.Position, 1));
    float4 view = mul(Globals.ViewMatrix, world);

    o.Position = mul(Globals.ProjectionMatrix, view);

    return o;
}

[shader("fragment")]
float4 Fragment(VSOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}