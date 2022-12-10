// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class AssemblyReferenceInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly AssemblyReference _assemblyReference;

    private AssemblyName? _assemblyName;
    private string? _culture;
    private CustomAttributeInfoCollection? _customAttributes;
    private string? _name;

    private AssemblyReferenceInfo(AssemblyReference assemblyReference, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _assemblyReference = assemblyReference;
    }

    public AssemblyName AssemblyName
    {
        get
        {
            var assemblyName = _assemblyName;

            if (assemblyName is null)
            {
                assemblyName = AssemblyReference.GetAssemblyName();
                _assemblyName = assemblyName;
            }

            return assemblyName;
        }
    }

    public ref readonly AssemblyReference AssemblyReference => ref _assemblyReference;

    public string Culture
    {
        get
        {
            var culture = _culture;

            if (culture is null)
            {
                culture = MetadataReader.GetString(AssemblyReference.Culture);
                _culture = culture;
            }

            return culture;
        }
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(AssemblyReference.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public AssemblyFlags Flags => AssemblyReference.Flags;

    // TODO: Handle HashValue

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(AssemblyReference.Name);
                _name = name;
            }

            return name;
        }
    }

    // TODO: Handle PublicKeyOrToken

    public Version Version => AssemblyReference.Version;

    public static AssemblyReferenceInfo Create(AssemblyReferenceHandle assemblyReferenceHandle, MetadataReader metadataReader)
    {
        var assemblyReference = metadataReader.GetAssemblyReference(assemblyReferenceHandle);
        return new AssemblyReferenceInfo(assemblyReference, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append('[');
        _ = builder.Append(Name);
        _ = builder.Append(']');

        return builder.ToString();
    }
}
