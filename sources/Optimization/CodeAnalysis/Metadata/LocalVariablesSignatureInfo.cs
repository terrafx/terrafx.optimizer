// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class LocalVariablesSignatureInfo : StandaloneSignatureInfo
{
    private ImmutableArray<MetadataInfo> _signature;

    internal LocalVariablesSignatureInfo(StandaloneSignature standaloneSignature, MetadataReader metadataReader)
        : base(standaloneSignature, metadataReader)
    {
        Debug.Assert(standaloneSignature.GetKind() == StandaloneSignatureKind.LocalVariables);
    }

    public override StandaloneSignatureKind Kind => StandaloneSignatureKind.LocalVariables;

    public ImmutableArray<MetadataInfo> Signature
    {
        get
        {
            if (_signature.IsDefault)
            {
                _signature = StandaloneSignature.DecodeLocalSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
            }

            return _signature;
        }
    }

    protected override string ResolveDisplayString() => throw new NotImplementedException();
}
