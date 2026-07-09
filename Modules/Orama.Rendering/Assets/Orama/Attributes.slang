// Attributes.slang
// Provides definitions for custom engine shader attributes.

module Attributes;

// Defines which rendering pass this shader will be used in.
[__AttributeUsage(_AttributeTargets.Struct)]
public struct ShaderPassAttribute
{
    string passName;
};