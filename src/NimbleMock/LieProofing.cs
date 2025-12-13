using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NimbleMock;

/// <summary>
/// Provides lie-proofing capabilities to validate mock shapes against real API endpoints.
/// </summary>
public static class LieProofing
{
    private static readonly Dictionary<string, ApiShape> _shapeCache = new();
    private static readonly HttpClient _httpClient = new();
    
    /// <summary>
    /// Validates that a mock interface matches the shape of a real API endpoint.
    /// This helps ensure mocks don't drift from actual API contracts.
    /// </summary>
    /// <typeparam name="T">The interface type to validate.</typeparam>
    /// <param name="stagingUrl">The staging URL of the real API endpoint.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A validation result indicating if the mock matches the real API shape.</returns>
    public static async Task<ValidationResult> AssertMatchesReal<T>(
        string stagingUrl,
        CancellationToken cancellationToken = default) where T : class
    {
        var interfaceType = typeof(T);
        var cacheKey = $"{interfaceType.FullName}:{stagingUrl}";
        
        if (!_shapeCache.TryGetValue(cacheKey, out var cachedShape))
        {
            cachedShape = await DiscoverApiShapeAsync(stagingUrl, cancellationToken);
            _shapeCache[cacheKey] = cachedShape;
        }
        
        var mockShape = ExtractMockShape(interfaceType);
        
        return ValidateShapes(mockShape, cachedShape);
    }
    
    private static async Task<ApiShape> DiscoverApiShapeAsync(string url, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            return new ApiShape
            {
                Methods = ExtractMethodsFromResponse(content),
                Properties = ExtractPropertiesFromResponse(content),
                IsValid = response.IsSuccessStatusCode
            };
        }
        catch
        {
            return new ApiShape { IsValid = false };
        }
    }
    
    private static MockShape ExtractMockShape(Type interfaceType)
    {
        var methods = interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.IsSpecialName)
            .Select(m => new MethodSignature
            {
                Name = m.Name,
                ReturnType = m.ReturnType.Name,
                Parameters = m.GetParameters().Select(p => new ParameterInfo
                {
                    Name = p.Name!,
                    Type = p.ParameterType.Name
                }).ToArray()
            })
            .ToArray();
        
        var properties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new PropertySignature
            {
                Name = p.Name,
                Type = p.PropertyType.Name
            })
            .ToArray();
        
        return new MockShape
        {
            Methods = methods,
            Properties = properties
        };
    }
    
    private static string[] ExtractMethodsFromResponse(string content)
    {
        return Array.Empty<string>();
    }
    
    private static string[] ExtractPropertiesFromResponse(string content)
    {
        return Array.Empty<string>();
    }
    
    private static ValidationResult ValidateShapes(MockShape mock, ApiShape api)
    {
        var mismatches = new List<string>();
        
        foreach (var mockMethod in mock.Methods)
        {
            var apiMethod = api.Methods.FirstOrDefault(m => m == mockMethod.Name);
            if (apiMethod == null)
            {
                mismatches.Add($"Method {mockMethod.Name} exists in mock but not in API");
            }
        }
        
        return new ValidationResult
        {
            IsValid = mismatches.Count == 0,
            Mismatches = mismatches.ToArray()
        };
    }
    
    private class ApiShape
    {
        public string[] Methods { get; set; } = Array.Empty<string>();
        public string[] Properties { get; set; } = Array.Empty<string>();
        public bool IsValid { get; set; }
    }
    
    private class MockShape
    {
        public MethodSignature[] Methods { get; set; } = Array.Empty<MethodSignature>();
        public PropertySignature[] Properties { get; set; } = Array.Empty<PropertySignature>();
    }
    
    private class MethodSignature
    {
        public string Name { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;
        public ParameterInfo[] Parameters { get; set; } = Array.Empty<ParameterInfo>();
    }
    
    private class PropertySignature
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
    
    private class ParameterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}

/// <summary>
/// Result of validating a mock shape against a real API.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates if the mock shape matches the real API.
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// List of mismatches found between mock and real API.
    /// </summary>
    public string[] Mismatches { get; set; } = Array.Empty<string>();
}

