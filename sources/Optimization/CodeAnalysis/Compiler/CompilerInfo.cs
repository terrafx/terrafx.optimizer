// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Concurrent;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed partial class CompilerInfo
{
    public static readonly CompilerInfo Instance = new CompilerInfo();

    private readonly ConcurrentDictionary<AssemblyFileHandle, AssemblyFileInfo> _assemblyFiles;
    private readonly ConcurrentDictionary<AssemblyReferenceHandle, AssemblyReferenceInfo> _assemblyReferences;
    private readonly ConcurrentDictionary<ConstantHandle, ConstantInfo> _constants;
    private readonly ConcurrentDictionary<CustomAttributeHandle, CustomAttributeInfo> _customAttributes;
    private readonly ConcurrentDictionary<DeclarativeSecurityAttributeHandle, DeclarativeSecurityAttributeInfo> _declarativeSecurityAttributes;
    private readonly ConcurrentDictionary<EventDefinitionHandle, EventDefinitionInfo> _eventDefinitions;
    private readonly ConcurrentDictionary<ExportedTypeHandle, ExportedTypeInfo> _exportedTypes;
    private readonly ConcurrentDictionary<FieldDefinitionHandle, FieldDefinitionInfo> _fieldDefinitions;
    private readonly ConcurrentDictionary<GenericParameterConstraintHandle, GenericParameterConstraintInfo> _genericParameterConstraints;
    private readonly ConcurrentDictionary<GenericParameterHandle, GenericParameterInfo> _genericParameters;
    private readonly ConcurrentDictionary<InterfaceImplementationHandle, InterfaceImplementationInfo> _interfaceImplementations;
    private readonly ConcurrentDictionary<MemberReferenceHandle, MemberReferenceInfo> _memberReferences;
    private readonly ConcurrentDictionary<MethodDefinitionHandle, MethodDefinitionInfo> _methodDefinitions;
    private readonly ConcurrentDictionary<MethodImplementationHandle, MethodImplementationInfo> _methodImplementations;
    private readonly ConcurrentDictionary<MethodSpecificationHandle, MethodSpecificationInfo> _methodSpecifications;
    private readonly ConcurrentDictionary<NamespaceDefinitionHandle, NamespaceDefinitionInfo> _namespaceDefinitions;
    private readonly ConcurrentDictionary<ParameterHandle, ParameterInfo> _parameters;
    private readonly ConcurrentDictionary<PropertyDefinitionHandle, PropertyDefinitionInfo> _propertyDefinitions;
    private readonly ConcurrentDictionary<StandaloneSignatureHandle, StandaloneSignatureInfo> _standaloneSignatures;
    private readonly ConcurrentDictionary<TypeDefinitionHandle, TypeDefinitionInfo> _typeDefinitions;
    private readonly ConcurrentDictionary<TypeReferenceHandle, TypeReferenceInfo> _typeReferences;
    private readonly ConcurrentDictionary<TypeSpecificationHandle, TypeSpecificationInfo> _typeSpecifications;

    public CompilerInfo()
    {
        _assemblyFiles = new ConcurrentDictionary<AssemblyFileHandle, AssemblyFileInfo>();
        _assemblyReferences = new ConcurrentDictionary<AssemblyReferenceHandle, AssemblyReferenceInfo>();
        _constants = new ConcurrentDictionary<ConstantHandle, ConstantInfo>();
        _customAttributes = new ConcurrentDictionary<CustomAttributeHandle, CustomAttributeInfo>();
        _declarativeSecurityAttributes = new ConcurrentDictionary<DeclarativeSecurityAttributeHandle, DeclarativeSecurityAttributeInfo>();
        _eventDefinitions = new ConcurrentDictionary<EventDefinitionHandle, EventDefinitionInfo>();
        _exportedTypes = new ConcurrentDictionary<ExportedTypeHandle, ExportedTypeInfo>();
        _fieldDefinitions = new ConcurrentDictionary<FieldDefinitionHandle, FieldDefinitionInfo>();
        _genericParameterConstraints = new ConcurrentDictionary<GenericParameterConstraintHandle, GenericParameterConstraintInfo>();
        _genericParameters = new ConcurrentDictionary<GenericParameterHandle, GenericParameterInfo>();
        _interfaceImplementations = new ConcurrentDictionary<InterfaceImplementationHandle, InterfaceImplementationInfo>();
        _memberReferences = new ConcurrentDictionary<MemberReferenceHandle, MemberReferenceInfo>();
        _methodDefinitions = new ConcurrentDictionary<MethodDefinitionHandle, MethodDefinitionInfo>();
        _methodImplementations = new ConcurrentDictionary<MethodImplementationHandle, MethodImplementationInfo>();
        _methodSpecifications = new ConcurrentDictionary<MethodSpecificationHandle, MethodSpecificationInfo>();
        _namespaceDefinitions = new ConcurrentDictionary<NamespaceDefinitionHandle, NamespaceDefinitionInfo>();
        _parameters = new ConcurrentDictionary<ParameterHandle, ParameterInfo>();
        _propertyDefinitions = new ConcurrentDictionary<PropertyDefinitionHandle, PropertyDefinitionInfo>();
        _standaloneSignatures = new ConcurrentDictionary<StandaloneSignatureHandle, StandaloneSignatureInfo>();
        _typeDefinitions = new ConcurrentDictionary<TypeDefinitionHandle, TypeDefinitionInfo>();
        _typeReferences = new ConcurrentDictionary<TypeReferenceHandle, TypeReferenceInfo>();
        _typeSpecifications = new ConcurrentDictionary<TypeSpecificationHandle, TypeSpecificationInfo>();
    }

    public MetadataInfo? Resolve(EntityHandle entityHandle, MetadataReader metadataReader)
    {
        if (entityHandle.IsNil)
        {
            return null;
        }
        return entityHandle.Kind switch {
            HandleKind.ModuleDefinition => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.TypeReference => Resolve((TypeReferenceHandle)entityHandle, metadataReader),
            HandleKind.TypeDefinition => Resolve((TypeDefinitionHandle)entityHandle, metadataReader),
            HandleKind.FieldDefinition => Resolve((FieldDefinitionHandle)entityHandle, metadataReader),
            HandleKind.MethodDefinition => Resolve((MethodDefinitionHandle)entityHandle, metadataReader),
            HandleKind.Parameter => Resolve((ParameterHandle)entityHandle, metadataReader),
            HandleKind.InterfaceImplementation => Resolve((InterfaceImplementationHandle)entityHandle, metadataReader),
            HandleKind.MemberReference => Resolve((MemberReferenceHandle)entityHandle, metadataReader),
            HandleKind.Constant => Resolve((ConstantHandle)entityHandle, metadataReader),
            HandleKind.CustomAttribute => Resolve((CustomAttributeHandle)entityHandle, metadataReader),
            HandleKind.DeclarativeSecurityAttribute => Resolve((DeclarativeSecurityAttributeHandle)entityHandle, metadataReader),
            HandleKind.StandaloneSignature => Resolve((StandaloneSignatureHandle)entityHandle, metadataReader),
            HandleKind.EventDefinition => Resolve((EventDefinitionHandle)entityHandle, metadataReader),
            HandleKind.PropertyDefinition => Resolve((PropertyDefinitionHandle)entityHandle, metadataReader),
            HandleKind.MethodImplementation => Resolve((MethodImplementationHandle)entityHandle, metadataReader),
            HandleKind.ModuleReference => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.TypeSpecification => Resolve((TypeSpecificationHandle)entityHandle, metadataReader),
            HandleKind.AssemblyDefinition => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.AssemblyReference => Resolve((AssemblyReferenceHandle)entityHandle, metadataReader),
            HandleKind.AssemblyFile => Resolve((AssemblyFileHandle)entityHandle, metadataReader),
            HandleKind.ExportedType => Resolve((ExportedTypeHandle)entityHandle, metadataReader),
            HandleKind.ManifestResource => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.GenericParameter => Resolve((GenericParameterHandle)entityHandle, metadataReader),
            HandleKind.MethodSpecification => Resolve((MethodSpecificationHandle)entityHandle, metadataReader),
            HandleKind.GenericParameterConstraint => Resolve((GenericParameterConstraintHandle)entityHandle, metadataReader),
            HandleKind.Document => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.MethodDebugInformation => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.LocalScope => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.LocalVariable => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.LocalConstant => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.ImportScope => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.CustomDebugInformation => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.UserString => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.Blob => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.Guid => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.String => ThrowNotImplementedException<MetadataInfo>(),
            HandleKind.NamespaceDefinition => ThrowNotImplementedException<MetadataInfo>(),
            _ => ThrowForInvalidKind<HandleKind, MetadataInfo?>(entityHandle.Kind),
        };
    }

    public AssemblyFileInfo? Resolve(AssemblyFileHandle assemblyFileHandle, MetadataReader metadataReader)
    {
        return assemblyFileHandle.IsNil
             ? null
             : _assemblyFiles.GetOrAdd(assemblyFileHandle, AssemblyFileInfo.Create, metadataReader);
    }

    public AssemblyReferenceInfo? Resolve(AssemblyReferenceHandle assemblyReferenceHandle, MetadataReader metadataReader)
    {
        return assemblyReferenceHandle.IsNil
             ? null
             : _assemblyReferences.GetOrAdd(assemblyReferenceHandle, AssemblyReferenceInfo.Create, metadataReader);
    }

    public ConstantInfo? Resolve(ConstantHandle constantHandle, MetadataReader metadataReader)
    {
        return constantHandle.IsNil
             ? null
             : _constants.GetOrAdd(constantHandle, ConstantInfo.Create, metadataReader);
    }

    public CustomAttributeInfo? Resolve(CustomAttributeHandle customAttributeHandle, MetadataReader metadataReader)
    {
        return customAttributeHandle.IsNil
             ? null
             : _customAttributes.GetOrAdd(customAttributeHandle, CustomAttributeInfo.Create, metadataReader);
    }

    public DeclarativeSecurityAttributeInfo? Resolve(DeclarativeSecurityAttributeHandle declarativeSecurityAttributeHandle, MetadataReader metadataReader)
    {
        return declarativeSecurityAttributeHandle.IsNil
             ? null
             : _declarativeSecurityAttributes.GetOrAdd(declarativeSecurityAttributeHandle, DeclarativeSecurityAttributeInfo.Create, metadataReader);
    }

    public EventDefinitionInfo? Resolve(EventDefinitionHandle eventDefinitionHandle, MetadataReader metadataReader)
    {
        return eventDefinitionHandle.IsNil
             ? null
             : _eventDefinitions.GetOrAdd(eventDefinitionHandle, EventDefinitionInfo.Create, metadataReader);
    }

    public FieldDefinitionInfo? Resolve(FieldDefinitionHandle fieldDefinitionHandle, MetadataReader metadataReader)
    {
        return fieldDefinitionHandle.IsNil
             ? null
             : _fieldDefinitions.GetOrAdd(fieldDefinitionHandle, FieldDefinitionInfo.Create, metadataReader);
    }

    public GenericParameterConstraintInfo? Resolve(GenericParameterConstraintHandle genericParameterConstraintHandle, MetadataReader metadataReader)
    {
        return genericParameterConstraintHandle.IsNil
             ? null
             : _genericParameterConstraints.GetOrAdd(genericParameterConstraintHandle, GenericParameterConstraintInfo.Create, metadataReader);
    }

    public GenericParameterInfo? Resolve(GenericParameterHandle genericParameterHandle, MetadataReader metadataReader)
    {
        return genericParameterHandle.IsNil
             ? null
             : _genericParameters.GetOrAdd(genericParameterHandle, GenericParameterInfo.Create, metadataReader);
    }

    public ExportedTypeInfo? Resolve(ExportedTypeHandle exportedTypeHandle, MetadataReader metadataReader)
    {
        return exportedTypeHandle.IsNil
             ? null
             : _exportedTypes.GetOrAdd(exportedTypeHandle, ExportedTypeInfo.Create, metadataReader);
    }

    public InterfaceImplementationInfo? Resolve(InterfaceImplementationHandle interfaceImplementationHandle, MetadataReader metadataReader)
    {
        return interfaceImplementationHandle.IsNil
             ? null
             : _interfaceImplementations.GetOrAdd(interfaceImplementationHandle, InterfaceImplementationInfo.Create, metadataReader);
    }

    public MemberReferenceInfo? Resolve(MemberReferenceHandle memberReferenceHandle, MetadataReader metadataReader)
    {
        return memberReferenceHandle.IsNil
             ? null
             : _memberReferences.GetOrAdd(memberReferenceHandle, MemberReferenceInfo.Create, metadataReader);
    }

    public MethodDefinitionInfo? Resolve(MethodDefinitionHandle methodDefinitionHandle, MetadataReader metadataReader)
    {
        return methodDefinitionHandle.IsNil
             ? null
             : _methodDefinitions.GetOrAdd(methodDefinitionHandle, MethodDefinitionInfo.Create, metadataReader);
    }

    public MethodImplementationInfo? Resolve(MethodImplementationHandle methodImplementationHandle, MetadataReader metadataReader)
    {
        return methodImplementationHandle.IsNil
             ? null
             : _methodImplementations.GetOrAdd(methodImplementationHandle, MethodImplementationInfo.Create, metadataReader);
    }

    public MethodSpecificationInfo? Resolve(MethodSpecificationHandle methodSpecificationHandle, MetadataReader metadataReader)
    {
        return methodSpecificationHandle.IsNil
             ? null
             : _methodSpecifications.GetOrAdd(methodSpecificationHandle, MethodSpecificationInfo.Create, metadataReader);
    }

    public NamespaceDefinitionInfo? Resolve(NamespaceDefinitionHandle namespaceDefinitionHandle, MetadataReader metadataReader)
    {
        return namespaceDefinitionHandle.IsNil
             ? null
             : _namespaceDefinitions.GetOrAdd(namespaceDefinitionHandle, NamespaceDefinitionInfo.Create, metadataReader);
    }

    public ParameterInfo? Resolve(ParameterHandle parameterHandle, MetadataReader metadataReader)
    {
        return parameterHandle.IsNil
             ? null
             : _parameters.GetOrAdd(parameterHandle, ParameterInfo.Create, metadataReader);
    }

    public PropertyDefinitionInfo? Resolve(PropertyDefinitionHandle propertyDefinitionHandle, MetadataReader metadataReader)
    {
        return propertyDefinitionHandle.IsNil
             ? null
             : _propertyDefinitions.GetOrAdd(propertyDefinitionHandle, PropertyDefinitionInfo.Create, metadataReader);
    }

    public StandaloneSignatureInfo? Resolve(StandaloneSignatureHandle standaloneSignatureHandle, MetadataReader metadataReader)
    {
        return standaloneSignatureHandle.IsNil
             ? null
             : _standaloneSignatures.GetOrAdd(standaloneSignatureHandle, StandaloneSignatureInfo.Create, metadataReader);
    }

    public TypeDefinitionInfo? Resolve(TypeDefinitionHandle typeDefinitionHandle, MetadataReader metadataReader)
    {
        return typeDefinitionHandle.IsNil
             ? null
             : _typeDefinitions.GetOrAdd(typeDefinitionHandle, TypeDefinitionInfo.Create, metadataReader);
    }

    public TypeReferenceInfo? Resolve(TypeReferenceHandle typeReferenceHandle, MetadataReader metadataReader)
    {
        return typeReferenceHandle.IsNil
             ? null
             : _typeReferences.GetOrAdd(typeReferenceHandle, TypeReferenceInfo.Create, metadataReader);
    }

    public TypeSpecificationInfo? Resolve(TypeSpecificationHandle typeSpecificationHandle, MetadataReader metadataReader)
    {
        return typeSpecificationHandle.IsNil
             ? null
             : _typeSpecifications.GetOrAdd(typeSpecificationHandle, TypeSpecificationInfo.Create, metadataReader);
    }
}
