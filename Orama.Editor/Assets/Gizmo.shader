#vertex VertexEntryPoint
#fragment FragmentEntryPoint

Name = "Default/Gizmo"
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
        float3 objPos : TEXCOORD0;
    };

    VSOutput VertexEntryPoint(VSInput input)
    {
        VSOutput o;
        o.pos = mul(u_MVP, float4(input.pos, 1.0));
        o.objPos = input.pos;
        return o;
    }

    float4 FragmentEntryPoint(VSOutput input) : SV_TARGET
    {
        float3 axisColor = abs(input.objPos);

        axisColor /= max(max(axisColor.x, axisColor.y), axisColor.z);

        return float4(axisColor, 1.0);
    }
}