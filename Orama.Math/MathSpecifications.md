# Math Library Specifications
## Vectors
The math library provides two main categories of vector structs: **floating-point vectors** (default) and **integer vectors** (denoted with an `Int` suffix).

Each vector type implements the following interfaces:
  - `IEquatable<Vector<T>>`
  - `IReadOnlyList<T>`
  - `IFormattable`
  - `IComparable<Vector<T>>`
  - `IComparable`

Each vector exposes named [properties](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties) (`X`, `Y`, `Z`, `W` as applicable), index-based access via this[int index],
and multiple constructors, including:
  -  Uniform value initialization (e.g., `new Vector2<T>(value)`)
  -  Per-component initialization (e.g., `new Vector2<T>(x, y)`)
  -  Partial initialization using lower-dimensional vectors (e.g., `new Vector3<T>(Vector2<T> xy, T z)`)

### Floating-Point Vectors
These vectors support any type `T` that implement `IFloatingPoint<T>`.

Supported types:
  - `Vector2<T>`
  - `Vector3<T>`
  - `Vector4<T>`

### Integer Vectors
These vectors support any type `T` that implements `IBinaryInteger<T>`.

Supported types:
  - `Vector2Int<T>`
  - `Vector3Int<T>`
  - `Vector4Int<T>`
