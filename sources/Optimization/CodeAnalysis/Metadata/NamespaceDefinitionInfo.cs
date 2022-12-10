// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class NamespaceDefinitionInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly NamespaceDefinition _namespaceDefinition;

    private ExportedTypeInfoCollection? _exportedTypes;
    private string? _name;
    private NamespaceDefinitionInfoCollection? _namespaceDefinitions;
    private NamespaceDefinitionInfo? _parent;
    private TypeDefinitionInfoCollection? _typeDefinitions;

    private NamespaceDefinitionInfo(NamespaceDefinition namespaceDefinition, MetadataReader metadataReader)
    {
        ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _namespaceDefinition = namespaceDefinition;
    }

    public ExportedTypeInfoCollection ExportedTypes
    {
        get
        {
            var exportedTypes = _exportedTypes;

            if (exportedTypes is null)
            {
                exportedTypes = ExportedTypeInfoCollection.Create(NamespaceDefinition.ExportedTypes, MetadataReader);
                _exportedTypes = exportedTypes;
            }

            return exportedTypes;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(NamespaceDefinition.Name);
                _name = name;
            }

            return name;
        }
    }

    public ref readonly NamespaceDefinition NamespaceDefinition => ref _namespaceDefinition;

    public NamespaceDefinitionInfoCollection NamespaceDefinitions
    {
        get
        {
            var namespaceDefinitions = _namespaceDefinitions;

            if (namespaceDefinitions is null)
            {
                namespaceDefinitions = NamespaceDefinitionInfoCollection.Create(NamespaceDefinition.NamespaceDefinitions, MetadataReader);
                _namespaceDefinitions = namespaceDefinitions;
            }

            return namespaceDefinitions;
        }
    }

    public NamespaceDefinitionInfo? Parent
    {
        get
        {
            var parent = _parent;

            if (parent is null)
            {
                parent = CompilerInfo.Instance.Resolve(NamespaceDefinition.Parent, MetadataReader);
                _parent = parent;
            }

            return parent;
        }
    }

    public string QualifiedName => DisplayString;

    public TypeDefinitionInfoCollection TypeDefinitions
    {
        get
        {
            var typeDefinitions = _typeDefinitions;

            if (typeDefinitions is null)
            {
                typeDefinitions = TypeDefinitionInfoCollection.Create(NamespaceDefinition.TypeDefinitions, MetadataReader);
                _typeDefinitions = typeDefinitions;
            }

            return typeDefinitions;
        }
    }

    public static NamespaceDefinitionInfo Create(NamespaceDefinitionHandle namespaceDefinitionHandle, MetadataReader metadataReader)
    {
        var namespaceDefinition = metadataReader.GetNamespaceDefinition(namespaceDefinitionHandle);
        return new NamespaceDefinitionInfo(namespaceDefinition, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();
        var parent = Parent;

        if (parent is not null)
        {
            _ = builder.Append(parent);
            _ = builder.Append('.');
        }

        _ = builder.Append(Name);
        return builder.ToString();
    }
}
