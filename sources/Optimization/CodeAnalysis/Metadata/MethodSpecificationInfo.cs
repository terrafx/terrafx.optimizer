// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodSpecificationInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly MethodSpecification _methodSpecification;

    private CustomAttributeInfoCollection? _customAttributes;
    private MetadataInfo? _method;

    private MethodSignature<MetadataInfo> _signature;
    private ImmutableArray<MetadataInfo> _typeArguments;
    private bool _isSignatureInitialized;

    private MethodSpecificationInfo(MethodSpecification methodSpecification, MetadataReader metadataReader)
    {
        ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _methodSpecification = methodSpecification;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(MethodSpecification.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public MetadataInfo Method
    {
        get
        {
            var method = _method;

            if (method is null)
            {
                method = CompilerInfo.Instance.Resolve(MethodSpecification.Method, MetadataReader);
                Debug.Assert(method is not null);
                _method = method;
            }

            return method;
        }
    }

    public ref readonly MethodSpecification MethodSpecification => ref _methodSpecification;

    public ref readonly MethodSignature<MetadataInfo> Signature
    {
        get
        {
            if (!_isSignatureInitialized)
            {
                var method = Method;

                if (method is MethodDefinitionInfo methodDefinitionInfo)
                {
                    _signature = methodDefinitionInfo.MethodDefinition.DecodeSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                }
                else if (method is MethodReferenceInfo methodReferenceInfo)
                {
                    _signature = methodReferenceInfo.MemberReference.DecodeMethodSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                }
                else
                {
                    ThrowUnreachableException();
                }

                _isSignatureInitialized = true;
            }

            return ref _signature;
        }
    }

    public ImmutableArray<MetadataInfo> TypeArguments
    {
        get
        {
            if (_typeArguments.IsDefault)
            {
                _typeArguments = MethodSpecification.DecodeSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
            }

            return _typeArguments;
        }
    }

    public static MethodSpecificationInfo Create(MethodSpecificationHandle methodSpecificationHandle, MetadataReader metadataReader)
    {
        var methodSpecification = metadataReader.GetMethodSpecification(methodSpecificationHandle);
        return new MethodSpecificationInfo(methodSpecification, metadataReader);
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

        var method = Method;
        var parameters = ParameterInfoCollection.Empty;

        if (method is MethodDefinitionInfo methodDefinitionInfo)
        {
            _ = builder.Append(methodDefinitionInfo.DeclaringType);
            _ = builder.Append("::");
            _ = builder.Append(methodDefinitionInfo.Name);

            parameters = methodDefinitionInfo.Parameters;
        }
        else if (method is MethodReferenceInfo methodReferenceInfo)
        {
            _ = builder.Append(methodReferenceInfo.Parent);
            _ = builder.Append("::");
            _ = builder.Append(methodReferenceInfo.Name);
        }
        else
        {
            ThrowUnreachableException();
        }

        var genericParameterCount = signature.GenericParameterCount;

        if (genericParameterCount != 0)
        {
            _ = builder.Append('`');
            _ = builder.Append(genericParameterCount);
        }

        var typeArguments = TypeArguments;

        if (typeArguments.Length != 0)
        {
            _ = builder.Append('<');
            _ = builder.Append(typeArguments[0]);

            for (var i = 1; i < typeArguments.Length; i++)
            {
                _ = builder.Append(", ");
                _ = builder.Append(typeArguments[i]);
            }

            _ = builder.Append('>');
        }

        var parameterTypes = signature.ParameterTypes;

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
