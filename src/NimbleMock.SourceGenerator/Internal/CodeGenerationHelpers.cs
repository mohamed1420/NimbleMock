using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NimbleMock.SourceGenerator.Internal;

internal static class CodeGenerationHelpers
{
    public static string GetDefaultArgs(IMethodSymbol method)
    {
        if (method.Parameters.Length == 0)
            return "";

        return string.Join(", ", method.Parameters.Select(p =>
        {
            if (p.Type.IsValueType)
                return "default";
            return "default!";
        }));
    }

    public static void GenerateMethodBody(
        StringBuilder sb,
        IMethodSymbol method,
        string interfaceTypeName)
    {
        var returnType = method.ReturnType.ToDisplayString();
        var methodName = method.Name;
        var parameters = string.Join(", ", method.Parameters.Select(p =>
            $"{p.Type.ToDisplayString()} {p.Name}"));
        var paramNames = string.Join(", ", method.Parameters.Select(p => p.Name));
        var paramCount = method.Parameters.Length;

        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public {returnType} {methodName}({parameters})");
        sb.AppendLine("    {");

        // Create method ID
        // Use simple type name since the proxy is in the same namespace as the interface
        // The proxy class can directly reference the interface type by its simple name
        var simpleTypeName = interfaceTypeName.Contains('.') 
            ? interfaceTypeName.Substring(interfaceTypeName.LastIndexOf('.') + 1)
            : interfaceTypeName;
        // Explicitly type the lambda parameter to help with delegate inference
        sb.AppendLine($"        var methodId = NimbleMock.Internal.MethodId.From<{simpleTypeName}>(");
        sb.AppendLine($"            ({simpleTypeName} x) => x.{methodName}({GetDefaultArgs(method)}));");
        sb.AppendLine();

        // Record call
        if (paramCount > 0)
        {
            sb.AppendLine($"        _instance.RecordCall(methodId, new object?[] {{ {paramNames} }});");
        }
        else
        {
            sb.AppendLine("        _instance.RecordCall(methodId, Array.Empty<object?>());");
        }
        sb.AppendLine();

        // Get setup
        sb.AppendLine("        if (!_instance.TryGetSetup(methodId, out var setup))");
        sb.AppendLine("        {");
        sb.AppendLine($"            if (_instance.IsPartial)");
        sb.AppendLine($"                throw new System.NotImplementedException($\"Method {methodName} is not mocked in partial mock.\");");
        if (method.ReturnsVoid)
        {
            sb.AppendLine("            return;");
        }
        else
        {
            sb.AppendLine($"            return default({returnType})!;");
        }
        sb.AppendLine("        }");
        sb.AppendLine();

        // Handle exceptions
        sb.AppendLine("        if (setup.IsException)");
        sb.AppendLine("            throw (Exception)setup.ReturnValue!;");
        sb.AppendLine();

        // Return value
        if (!method.ReturnsVoid)
        {
            sb.AppendLine($"        return ({returnType})setup.ReturnValue!;");
        }

        sb.AppendLine("    }");
        sb.AppendLine();
    }
}

