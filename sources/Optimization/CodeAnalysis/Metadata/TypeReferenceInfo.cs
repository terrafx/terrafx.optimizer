// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class TypeReferenceInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly TypeReference _typeReference;

    private string? _name;
    private string? _namespace;
    private MetadataInfo? _resolutionScope;

    private TypeReferenceInfo(TypeReference typeReference, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _typeReference = typeReference;
    }

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(TypeReference.Name);
                _name = name;
            }

            return name;
        }
    }

    public string? Namespace
    {
        get
        {
            var @namespace = _namespace;

            if (@namespace is null)
            {
                @namespace = MetadataReader.GetString(TypeReference.Namespace);
                _namespace = @namespace;
            }

            return @namespace;
        }
    }

    public MetadataInfo ResolutionScope
    {
        get
        {
            var resolutionScope = _resolutionScope;

            if (resolutionScope is null)
            {
                resolutionScope = CompilerInfo.Instance.Resolve(TypeReference.ResolutionScope, MetadataReader);
                Debug.Assert(resolutionScope is not null);
                _resolutionScope = resolutionScope;
            }

            return resolutionScope;
        }
    }

    public string QualifiedName => DisplayString;

    public ref readonly TypeReference TypeReference => ref _typeReference;

    public static TypeReferenceInfo Create(TypeReferenceHandle typeReferenceHandle, MetadataReader metadataReader)
    {
        var typeReference = metadataReader.GetTypeReference(typeReferenceHandle);
        return new TypeReferenceInfo(typeReference, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(ResolutionScope);

        if (Namespace is string @namespace)
        {
            _ = builder.Append(@namespace);
            _ = builder.Append('.');
        }

        _ = builder.Append(Name);
        return builder.ToString();
    }
}
