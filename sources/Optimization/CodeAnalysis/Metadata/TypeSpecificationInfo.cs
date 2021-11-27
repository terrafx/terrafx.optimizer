
// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class TypeSpecificationInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly TypeSpecification _typeSpecification;

    private CustomAttributeInfoCollection? _customAttributes;
    private MetadataInfo? _signature;

    private TypeSpecificationInfo(TypeSpecification typeSpecification, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _typeSpecification = typeSpecification;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(TypeSpecification.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public MetadataInfo Signature
    {
        get
        {
            var signature = _signature;

            if (signature is null)
            {
                signature = TypeSpecification.DecodeSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                Debug.Assert(signature is not null);
                _signature = signature;
            }

            return signature;
        }
    }

    public ref readonly TypeSpecification TypeSpecification => ref _typeSpecification;

    public static TypeSpecificationInfo Create(TypeSpecificationHandle typeSpecificationHandle, MetadataReader metadataReader)
    {
        var typeSpecification = metadataReader.GetTypeSpecification(typeSpecificationHandle);
        return new TypeSpecificationInfo(typeSpecification, metadataReader);
    }

    protected override string ResolveDisplayString() => Signature.DisplayString;
}
