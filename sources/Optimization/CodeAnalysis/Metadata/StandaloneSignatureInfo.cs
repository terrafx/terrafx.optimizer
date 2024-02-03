// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public abstract class StandaloneSignatureInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly StandaloneSignature _standaloneSignature;

    private CustomAttributeInfoCollection? _customAttributes;

    protected StandaloneSignatureInfo(StandaloneSignature standaloneSignature, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _standaloneSignature = standaloneSignature;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(StandaloneSignature.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public abstract StandaloneSignatureKind Kind { get; }

    public MetadataReader MetadataReader => _metadataReader;

    public ref readonly StandaloneSignature StandaloneSignature => ref _standaloneSignature;

    public static StandaloneSignatureInfo Create(StandaloneSignatureHandle standaloneSignatureHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var standaloneSignature = metadataReader.GetStandaloneSignature(standaloneSignatureHandle);

        return standaloneSignature.GetKind() switch {
            StandaloneSignatureKind.Method => new MethodSignatureInfo(standaloneSignature, metadataReader),
            StandaloneSignatureKind.LocalVariables => new LocalVariablesSignatureInfo(standaloneSignature, metadataReader),
            _ => ThrowForInvalidKind<StandaloneSignatureKind, StandaloneSignatureInfo>(standaloneSignature.GetKind()),
        };
    }
}
