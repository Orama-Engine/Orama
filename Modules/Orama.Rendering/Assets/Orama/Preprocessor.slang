// Orama.Preprocessor.slang
// Provides macros and definitions that can be included by Orama shaders.

module Preprocessor;

// -----------------------------------------------------------------------------
// Macros.
// -----------------------------------------------------------------------------

// Defines a parameter block that can be read from the engine.
#define SHADER_PARAMETERS(params)                                                                                           \
    struct ShaderParameters                                                                                            \
    {                                                                                                                  \
        params                                                                                                              \
    };                                                                                                                 \
    ParameterBlock<ShaderParameters> Parameters;

