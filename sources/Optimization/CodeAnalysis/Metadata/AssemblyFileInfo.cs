// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class AssemblyFileInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly AssemblyFile _assemblyFile;

    private CustomAttributeInfoCollection? _customAttributes;
    private string? _name;

    private AssemblyFileInfo(AssemblyFile assemblyFile, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _assemblyFile = assemblyFile;
    }

    public ref readonly AssemblyFile AssemblyFile => ref _assemblyFile;

    public bool ContainsMetadata => AssemblyFile.ContainsMetadata;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(AssemblyFile.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    // TODO: Handle HashValue

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(AssemblyFile.Name);
                _name = name;
            }

            return name;
        }
    }

    public static AssemblyFileInfo Create(AssemblyFileHandle assemblyFileHandle, MetadataReader metadataReader)
    {
        var assemblyFile = metadataReader.GetAssemblyFile(assemblyFileHandle);
        return new AssemblyFileInfo(assemblyFile, metadataReader);
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
