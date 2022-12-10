// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodReferenceInfo : MemberReferenceInfo
{
    private MethodSignature<MetadataInfo> _methodSignature;
    private bool _isMethodSignatureInitialized;

    internal MethodReferenceInfo(MemberReference memberReference, MetadataReader metadataReader)
        : base(memberReference, metadataReader)
    {
        Debug.Assert(memberReference.GetKind() == MemberReferenceKind.Method);
    }

    public override MemberReferenceKind Kind => MemberReferenceKind.Method;

    public ref readonly MethodSignature<MetadataInfo> Signature
    {
        get
        {
            if (!_isMethodSignatureInitialized)
            {
                _methodSignature = MemberReference.DecodeMethodSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                _isMethodSignatureInitialized = true;
            }

            return ref _methodSignature;
        }
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
        _ = builder.Append(Parent);
        _ = builder.Append("::");
        _ = builder.Append(Name);

        var genericParameterCount = signature.GenericParameterCount;

        if (genericParameterCount != 0)
        {
            _ = builder.Append('`');
            _ = builder.Append(genericParameterCount);
        }

        var parameterTypes = signature.ParameterTypes;

        if (parameterTypes.Length == 0)
        {
            _ = builder.Append("()");
        }
        else
        {
            _ = AppendParameters(builder, parameterTypes);
        }

        return builder.ToString();
    }
}
