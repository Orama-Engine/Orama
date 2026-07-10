// Attributes.slang
// Provides definitions for custom engine shader attributes.

module Attributes;

// Defines which rendering pass this shader will be used in.
[__AttributeUsage(_AttributeTargets.Struct)]
public struct ShaderPassAttribute
{
    string passName;
};

// -----------------------------------------------------------------------------
// Defaults.
// -----------------------------------------------------------------------------
[__AttributeUsage(_AttributeTargets.Var)]
public struct DefaultFloatAttribute
{
	float value;
};

[__AttributeUsage(_AttributeTargets.Var)]
public struct DefaultIntAttribute
{
	int value;
};

[__AttributeUsage(_AttributeTargets.Var)]
public struct DefaultFloat2Attribute
{
    float x, y;
};

[__AttributeUsage(_AttributeTargets.Var)]
public struct DefaultFloat3Attribute
{
    float x, y, z;
};

[__AttributeUsage(_AttributeTargets.Var)]
public struct DefaultFloat4Attribute
{
    float x, y, z, w;
};

