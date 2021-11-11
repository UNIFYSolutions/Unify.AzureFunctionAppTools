#if NETCOREAPP3_1
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Missing class in .NET CORE 3.1. Needed for using c# 9 init property.
    /// </summary>
    public class IsExternalInit { }
}
#endif