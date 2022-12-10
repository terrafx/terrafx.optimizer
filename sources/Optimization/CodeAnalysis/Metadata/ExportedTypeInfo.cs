// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ExportedTypeInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly ExportedType _exportedType;

    private CustomAttributeInfoCollection? _customAttributes;
    private MetadataInfo? _implementation;
    private string? _name;
    private string? _namespace;
    private NamespaceDefinitionInfo? _namespaceDefinition;

    private ExportedTypeInfo(ExportedType exportedType, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _exportedType = exportedType;
    }

    public TypeAttributes Attributes => ExportedType.Attributes;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(ExportedType.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public ref readonly ExportedType ExportedType => ref _exportedType;

    public MetadataInfo Implementation
    {
        get
        {
            var implementation = _implementation;

            if (implementation is null)
            {
                var metadataReader = MetadataReader;
                var implementationHandle = ExportedType.Implementation;

                if (implementationHandle.Kind == HandleKind.AssemblyFile)
                {
                    var assemblyFileHandle = (AssemblyFileHandle)implementationHandle;
                    implementation = CompilerInfo.Instance.Resolve(assemblyFileHandle, metadataReader);
                }
                else if (implementationHandle.Kind == HandleKind.AssemblyReference)
                {
                    var assemblyReferenceHandle = (AssemblyReferenceHandle)implementationHandle;
                    implementation = CompilerInfo.Instance.Resolve(assemblyReferenceHandle, metadataReader);
                }
                else if (implementationHandle.Kind == HandleKind.ExportedType)
                {
                    var exportedTypeHandle = (ExportedTypeHandle)implementationHandle;
                    implementation = CompilerInfo.Instance.Resolve(exportedTypeHandle, metadataReader);
                }
                else
                {
                    throw new NotSupportedException();
                }

                Debug.Assert(implementation is not null);
                _implementation = implementation;
            }

            return implementation;
        }
    }

    public bool IsForwarder => ExportedType.IsForwarder;

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(ExportedType.Name);
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
                @namespace = MetadataReader.GetString(ExportedType.Namespace);
                _namespace = @namespace;
            }

            return @namespace;
        }
    }

    public NamespaceDefinitionInfo? NamespaceDefinition
    {
        get
        {
            var namespaceDefinition = _namespaceDefinition;

            if (namespaceDefinition is null)
            {
                namespaceDefinition = CompilerInfo.Instance.Resolve(ExportedType.NamespaceDefinition, MetadataReader);
                _namespaceDefinition = namespaceDefinition;
            }

            return namespaceDefinition;
        }
    }

    public string QualifiedName => DisplayString;

    public static ExportedTypeInfo Create(ExportedTypeHandle exportedTypeHandle, MetadataReader metadataReader)
    {
        var exportedType = metadataReader.GetExportedType(exportedTypeHandle);
        return new ExportedTypeInfo(exportedType, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        if (Implementation is MetadataInfo implementation)
        {
            _ = builder.Append(implementation);

            if (implementation is ExportedTypeInfo)
            {
                Debug.Assert(Namespace is null);
                _ = builder.Append('.');
            }
        }

        if (Namespace is string @namespace)
        {
            Debug.Assert(Implementation is not ExportedTypeInfo);
            _ = builder.Append(@namespace);
            _ = builder.Append('.'); 
        }

        _ = builder.Append(Name);
        return builder.ToString();
    }
}
