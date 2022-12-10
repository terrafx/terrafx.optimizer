// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class FieldReferenceInfo : MemberReferenceInfo
{
    private MetadataInfo? _signature;

    internal FieldReferenceInfo(MemberReference memberReference, MetadataReader metadataReader)
        : base(memberReference, metadataReader)
    {
        Debug.Assert(memberReference.GetKind() == MemberReferenceKind.Field);
    }

    public override MemberReferenceKind Kind => MemberReferenceKind.Field;

    public MetadataInfo? Signature
    {
        get
        {
            var signature = _signature;

            if (signature is null)
            {
                signature = MemberReference.DecodeFieldSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                _signature = signature;
            }

            return signature;
        }
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(Signature);;
        _ = builder.Append(' ');
        _ = builder.Append(Parent);
        _ = builder.Append("::");
        _ = builder.Append(Name);

        return builder.ToString();
    }
}
