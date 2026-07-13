// Core.slang
// Provides abstractions and datatypes used for Orama shaders.

module Core;

// -----------------------------------------------------------------------------
// Shared Structures.
// -----------------------------------------------------------------------------

public struct CameraData
{
    public float4x4 ViewMatrix;
    public float4x4 ProjectionMatrix;
};

public ParameterBlock<CameraData> Camera;

public struct ObjectData
{
    public float4x4 ModelMatrix;
};

public ParameterBlock<ObjectData> Object;

public struct VertexInput
{
    public float3 Position : POSITION;
    public float3 Normal : NORMAL;
    public float2 UV : TEXCOORD0;
};

public struct VertexOutput
{
    public float4 Position : SV_Position;
    public float3 WorldPosition : TEXCOORD0;
    public float3 Normal : NORMAL;
};

