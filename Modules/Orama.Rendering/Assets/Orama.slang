public struct GlobalData
{
    public float4x4 ObjectMatrix;
    public float4x4 ViewMatrix;
    public float4x4 ProjectionMatrix;
};

public struct VSInput
{
    public float3 Position : POSITION;
    public float3 Normal : NORMAL;
    public float2 UV : TEXCOORD0;
};

public struct VSOutput
{
    public float4 Position : SV_Position;
};