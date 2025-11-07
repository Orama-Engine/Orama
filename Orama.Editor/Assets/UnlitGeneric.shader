#vertex VertexEntryPoint
#fragment FragmentEntryPoint

Name = "Default/White"
Pass = "Opaque"

Properties
{
    float4x4 u_MVP;
}

Source
{
    struct VSInput
    {
        float3 pos : POSITION;
    };

    struct VSOutput
    {
        float4 pos : SV_POSITION;
    };

    VSOutput VertexEntryPoint(VSInput input)
    {
        VSOutput output;
        output.pos = mul(u_MVP, float4(input.pos, 1.0));
        return output;
    }

    float4 FragmentEntryPoint(VSOutput input) : SV_TARGET
    {
        return float4(1.0, 1.0, 1.0, 1.0);
    }
}