// Polyfill required for C# 9 init-only property setters and record types.
// Unity 6.x uses the .NET Standard 2.1 BCL but ships with the Roslyn compiler
// flag that enables C# 9 syntax. The runtime type IsExternalInit is expected by
// the compiler but is not included in .NET Standard 2.1 assemblies, so it must
// be declared manually in the consuming project.
// Without this file, any use of `record` or `init` accessors produces CS0518.
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Compiler-required marker type that enables C# 9 <c>init</c> accessors and
    /// <c>record</c> types in Unity projects targeting .NET Standard 2.1.
    /// </summary>
    /// <remarks>
    /// The C# 9 compiler emits a reference to this type for every <c>init</c>
    /// property setter it generates. Because <c>System.Runtime.CompilerServices.IsExternalInit</c>
    /// is not present in the .NET Standard 2.1 runtime shipped with Unity 6.x,
    /// the type must be declared here so compilation succeeds.
    /// This file has no runtime behaviour and can be safely ignored by tooling.
    /// </remarks>
    internal static class IsExternalInit { }
}