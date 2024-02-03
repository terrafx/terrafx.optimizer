// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class TypeDefinitionInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly TypeDefinition _typeDefinition;

    private MetadataInfo? _baseType;
    private CustomAttributeInfoCollection? _customAttributes;
    private DeclarativeSecurityAttributeInfoCollection? _declarativeSecurityAttributes;
    private TypeDefinitionInfo? _declaringType;
    private EventDefinitionInfoCollection? _events;
    private FieldDefinitionInfoCollection? _fields;
    private GenericParameterInfoCollection? _genericParameters;
    private InterfaceImplementationInfoCollection? _interfaceImplementations;
    private TypeLayout _layout;
    private MethodImplementationInfoCollection? _methodImplementations;
    private MethodDefinitionInfoCollection? _methods;
    private string? _name;
    private string? _namespace;
    private NamespaceDefinitionInfo? _namespaceDefinition;
    private TypeDefinitionInfoCollection? _nestedTypes;
    private PropertyDefinitionInfoCollection? _properties;

    private bool _isLayoutInitialized;

    private TypeDefinitionInfo(TypeDefinition typeDefinition, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _typeDefinition = typeDefinition;
    }

    public TypeAttributes Attributes => TypeDefinition.Attributes;

    public MetadataInfo BaseType
    {
        get
        {
            var baseType = _baseType;

            if (baseType is null)
            {
                var metadataReader = MetadataReader;
                var baseTypeHandle = TypeDefinition.BaseType;

                if (baseTypeHandle.Kind == HandleKind.TypeDefinition)
                {
                    var typeDefinitionHandle = (TypeDefinitionHandle)baseTypeHandle;
                    baseType = CompilerInfo.Instance.Resolve(typeDefinitionHandle, metadataReader);
                }
                else if (baseTypeHandle.Kind == HandleKind.TypeReference)
                {
                    var typeReferenceHandle = (TypeReferenceHandle)baseTypeHandle;
                    baseType = CompilerInfo.Instance.Resolve(typeReferenceHandle, metadataReader);
                }
                else if (baseTypeHandle.Kind == HandleKind.TypeSpecification)
                {
                    var typeSpecificationHandle = (TypeSpecificationHandle)baseTypeHandle;
                    baseType = CompilerInfo.Instance.Resolve(typeSpecificationHandle, metadataReader);
                }
                else
                {
                    ThrowForInvalidKind(baseTypeHandle.Kind);
                }

                Debug.Assert(baseType is not null);
                _baseType = baseType;
            }

            return baseType;
        }
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(TypeDefinition.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public DeclarativeSecurityAttributeInfoCollection DeclarativeSecurityAttributes
    {
        get
        {
            var declarativeSecurityAttributes = _declarativeSecurityAttributes;

            if (declarativeSecurityAttributes is null)
            {
                declarativeSecurityAttributes = DeclarativeSecurityAttributeInfoCollection.Create(TypeDefinition.GetDeclarativeSecurityAttributes(), MetadataReader);
                _declarativeSecurityAttributes = declarativeSecurityAttributes;
            }

            return declarativeSecurityAttributes;
        }
    }

    public TypeDefinitionInfo? DeclaringType
    {
        get
        {
            var declaringType = _declaringType;

            if (declaringType is null)
            {
                declaringType = CompilerInfo.Instance.Resolve(TypeDefinition.GetDeclaringType(), MetadataReader);
                _declaringType = declaringType;
            }

            return declaringType;
        }
    }

    public EventDefinitionInfoCollection Events
    {
        get
        {
            var events = _events;

            if (events is null)
            {
                events = EventDefinitionInfoCollection.Create(TypeDefinition.GetEvents(), MetadataReader);
                _events = events;
            }

            return events;
        }
    }

    public FieldDefinitionInfoCollection Fields
    {
        get
        {
            var fields = _fields;

            if (fields is null)
            {
                fields = FieldDefinitionInfoCollection.Create(TypeDefinition.GetFields(), MetadataReader);
                _fields = fields;
            }

            return fields;
        }
    }

    public GenericParameterInfoCollection GenericParameters
    {
        get
        {
            var genericParameters = _genericParameters;

            if (genericParameters is null)
            {
                genericParameters = GenericParameterInfoCollection.Create(TypeDefinition.GetGenericParameters(), MetadataReader);
                _genericParameters = genericParameters;
            }

            return genericParameters;
        }
    }

    public InterfaceImplementationInfoCollection InterfaceImplementations
    {
        get
        {
            var interfaceImplementations = _interfaceImplementations;

            if (interfaceImplementations is null)
            {
                interfaceImplementations = InterfaceImplementationInfoCollection.Create(TypeDefinition.GetInterfaceImplementations(), MetadataReader);
                _interfaceImplementations = interfaceImplementations;
            }

            return interfaceImplementations;
        }
    }

    public bool IsNested => TypeDefinition.IsNested;

    public ref readonly TypeLayout Layout
    {
        get
        {
            if (!_isLayoutInitialized)
            {
                _layout = TypeDefinition.GetLayout();
                _isLayoutInitialized = true;
            }

            return ref _layout;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public MethodImplementationInfoCollection MethodImplementations
    {
        get
        {
            var methodImplementations = _methodImplementations;

            if (methodImplementations is null)
            {
                methodImplementations = MethodImplementationInfoCollection.Create(TypeDefinition.GetMethodImplementations(), MetadataReader);
                _methodImplementations = methodImplementations;
            }

            return methodImplementations;
        }
    }

    public MethodDefinitionInfoCollection Methods
    {
        get
        {
            var methods = _methods;

            if (methods is null)
            {
                methods = MethodDefinitionInfoCollection.Create(TypeDefinition.GetMethods(), MetadataReader);
                _methods = methods;
            }

            return methods;
        }
    }

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(TypeDefinition.Name);
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
                @namespace = MetadataReader.GetString(TypeDefinition.Namespace);
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
                namespaceDefinition = CompilerInfo.Instance.Resolve(TypeDefinition.NamespaceDefinition, MetadataReader);
                _namespaceDefinition = namespaceDefinition;
            }

            return namespaceDefinition;
        }
    }

    public TypeDefinitionInfoCollection NestedTypes
    {
        get
        {
            var nestedTypes = _nestedTypes;

            if (nestedTypes is null)
            {
                nestedTypes = TypeDefinitionInfoCollection.Create(TypeDefinition.GetNestedTypes(), MetadataReader);
                _nestedTypes = nestedTypes;
            }

            return nestedTypes;
        }
    }

    public PropertyDefinitionInfoCollection Properties
    {
        get
        {
            var properties = _properties;

            if (properties is null)
            {
                properties = PropertyDefinitionInfoCollection.Create(TypeDefinition.GetProperties(), MetadataReader);
                _properties = properties;
            }

            return properties;
        }
    }

    public string QualifiedName => DisplayString;

    public ref readonly TypeDefinition TypeDefinition => ref _typeDefinition;

    public static TypeDefinitionInfo Create(TypeDefinitionHandle typeDefinitionHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var typeDefinition = metadataReader.GetTypeDefinition(typeDefinitionHandle);
        return new TypeDefinitionInfo(typeDefinition, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        if (DeclaringType is TypeDefinitionInfo declaringType)
        {
            _ = builder.Append(declaringType);
            _ = builder.Append('/');
        }
        else if (Namespace is string @namespace)
        {
            _ = builder.Append(@namespace);
            _ = builder.Append('.');
        }

        _ = builder.Append(Name);
        _ = AppendGenericParameters(builder, GenericParameters);

        return builder.ToString();
    }
}
