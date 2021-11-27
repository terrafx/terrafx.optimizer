// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodDefinitionInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly MethodDefinition _methodDefinition;

    private CustomAttributeInfoCollection? _customAttributes;
    private DeclarativeSecurityAttributeInfoCollection? _declarativeSecurityAttributes;
    private TypeDefinitionInfo? _declaringType;
    private GenericParameterInfoCollection? _genericParameters;
    private string? _name;
    private ParameterInfoCollection? _parameters;

    private MethodImport _import;
    private MethodSignature<MetadataInfo> _signature;
    private bool _isMethodImportInitialized;
    private bool _isSignatureInitialized;

    private MethodDefinitionInfo(MethodDefinition methodDefinition, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _methodDefinition = methodDefinition;
    }

    public MethodAttributes Attributes => MethodDefinition.Attributes;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(MethodDefinition.GetCustomAttributes(), MetadataReader);
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
                declarativeSecurityAttributes = DeclarativeSecurityAttributeInfoCollection.Create(MethodDefinition.GetDeclarativeSecurityAttributes(), MetadataReader);
                _declarativeSecurityAttributes = declarativeSecurityAttributes;
            }

            return declarativeSecurityAttributes;
        }
    }

    public TypeDefinitionInfo DeclaringType
    {
        get
        {
            var declaringType = _declaringType;

            if (declaringType is null)
            {
                declaringType = CompilerInfo.Instance.Resolve(MethodDefinition.GetDeclaringType(), MetadataReader);
                Debug.Assert(declaringType is not null);
                _declaringType = declaringType;
            }

            return declaringType;
        }
    }

    public GenericParameterInfoCollection GenericParameters
    {
        get
        {
            var genericParameters = _genericParameters;

            if (genericParameters is null)
            {
                genericParameters = GenericParameterInfoCollection.Create(MethodDefinition.GetGenericParameters(), MetadataReader);
                _genericParameters = genericParameters;
            }

            return genericParameters;
        }
    }

    public MethodImplAttributes ImplAttributes => MethodDefinition.ImplAttributes;

    public ref readonly MethodImport Import
    {
        get
        {
            if (!_isMethodImportInitialized)
            {
                _import = MethodDefinition.GetImport();
                _isMethodImportInitialized = true;
            }

            return ref _import;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public ref readonly MethodDefinition MethodDefinition => ref _methodDefinition;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(MethodDefinition.Name);
                _name = name;
            }

            return name;
        }
    }

    public ParameterInfoCollection Parameters
    {
        get
        {
            var parameters = _parameters;

            if (parameters is null)
            {
                parameters = ParameterInfoCollection.Create(MethodDefinition.GetParameters(), MetadataReader);
                _parameters = parameters;
            }

            return parameters;
        }
    }

    public string QualifiedName => DisplayString;

    public int RelativeVirtualAddress => MethodDefinition.RelativeVirtualAddress;

    public ref readonly MethodSignature<MetadataInfo> Signature
    {
        get
        {
            if (!_isSignatureInitialized)
            {
                _signature = MethodDefinition.DecodeSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                _isSignatureInitialized = true;
            }

            return ref _signature;
        }
    }

    public static MethodDefinitionInfo Create(MethodDefinitionHandle methodDefinitionHandle, MetadataReader metadataReader)
    {
        var methodDefinition = metadataReader.GetMethodDefinition(methodDefinitionHandle);
        return new MethodDefinitionInfo(methodDefinition, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        var signature = Signature;
        var isInstance = signature.Header.IsInstance;

        if (isInstance)
        {
            _ = builder.Append("instance ");
        }

        _ = builder.Append(signature.ReturnType);
        _ = builder.Append(' ');
        _ = builder.Append(DeclaringType);
        _ = builder.Append("::");
        _ = builder.Append(Name);

        var genericParameters = GenericParameters;
        var genericParameterCount = signature.GenericParameterCount;

        if (genericParameterCount != 0)
        {
            _ = builder.Append('`');
            _ = builder.Append(genericParameterCount);
        }

        _ = AppendGenericParameters(builder, genericParameters);

        var parameterTypes = signature.ParameterTypes;
        var parameters = Parameters;

        if (parameterTypes.Length == 0)
        {
            _ = builder.Append("()");
        }
        else if (parameters.Count == 0)
        {
            _ = AppendParameters(builder, parameterTypes);
        }
        else
        {
            _ = AppendParameters(builder, isInstance, parameterTypes, parameters);
        }

        return builder.ToString();
    }
}
