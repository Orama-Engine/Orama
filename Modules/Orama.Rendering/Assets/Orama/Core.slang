// Core.slang
// Provides abstractions and datatypes used for Orama shaders.

module Core;

// -----------------------------------------------------------------------------
// Shared Structures.
// -----------------------------------------------------------------------------
public struct GlobalData
{
    public float4x4 ObjectMatrix;
    public float4x4 ViewMatrix;
    public float4x4 ProjectionMatrix;
};

public ParameterBlock<GlobalData> Globals;

public struct VertexInput
{
    public float3 Position : POSITION;
    public float3 Normal : NORMAL;
    public float2 UV : TEXCOORD0;
};

public struct VertexOutput
{
    public float4 Position : SV_Position;
};

