// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

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
        ArgumentNullException.ThrowIfNull(metadataReader);

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

                switch (implementationHandle.Kind)
                {
                    case HandleKind.AssemblyFile:
                    {

                        var assemblyFileHandle = (AssemblyFileHandle)implementationHandle;
                        implementation = CompilerInfo.Instance.Resolve(assemblyFileHandle, metadataReader);
                        break;
                    }

                    case HandleKind.AssemblyReference:
                    {
                        var assemblyReferenceHandle = (AssemblyReferenceHandle)implementationHandle;
                        implementation = CompilerInfo.Instance.Resolve(assemblyReferenceHandle, metadataReader);
                        break;
                    }

                    case HandleKind.ExportedType:
                    {
                        var exportedTypeHandle = (ExportedTypeHandle)implementationHandle;
                        implementation = CompilerInfo.Instance.Resolve(exportedTypeHandle, metadataReader);
                        break;
                    }

                    case HandleKind.ModuleDefinition:
                    case HandleKind.TypeReference:
                    case HandleKind.TypeDefinition:
                    case HandleKind.FieldDefinition:
                    case HandleKind.MethodDefinition:
                    case HandleKind.Parameter:
                    case HandleKind.InterfaceImplementation:
                    case HandleKind.MemberReference:
                    case HandleKind.Constant:
                    case HandleKind.CustomAttribute:
                    case HandleKind.DeclarativeSecurityAttribute:
                    case HandleKind.StandaloneSignature:
                    case HandleKind.EventDefinition:
                    case HandleKind.PropertyDefinition:
                    case HandleKind.MethodImplementation:
                    case HandleKind.ModuleReference:
                    case HandleKind.TypeSpecification:
                    case HandleKind.AssemblyDefinition:
                    case HandleKind.ManifestResource:
                    case HandleKind.GenericParameter:
                    case HandleKind.MethodSpecification:
                    case HandleKind.GenericParameterConstraint:
                    case HandleKind.Document:
                    case HandleKind.MethodDebugInformation:
                    case HandleKind.LocalScope:
                    case HandleKind.LocalVariable:
                    case HandleKind.LocalConstant:
                    case HandleKind.ImportScope:
                    case HandleKind.CustomDebugInformation:
                    case HandleKind.UserString:
                    case HandleKind.Blob:
                    case HandleKind.Guid:
                    case HandleKind.String:
                    case HandleKind.NamespaceDefinition:
                    {
                        ThrowForInvalidKind(implementationHandle.Kind);
                        break;
                    }

                    default:
                    {
                        ThrowUnreachableException();
                        break;
                    }
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
        ArgumentNullException.ThrowIfNull(metadataReader);
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
