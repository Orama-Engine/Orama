// Attributes.slang
// Provides definitions for custom engine shader attributes.

module Attributes;

[__AttributeUsage(_AttributeTargets.Struct)]
public struct ShaderPassAttribute
{
    string passName;
};