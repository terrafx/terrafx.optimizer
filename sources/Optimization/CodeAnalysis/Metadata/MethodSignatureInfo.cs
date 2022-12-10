// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodSignatureInfo : StandaloneSignatureInfo
{
    private MethodSignature<MetadataInfo> _signature;
    private bool _isSignatureInitialized;

    internal MethodSignatureInfo(StandaloneSignature standaloneSignature, MetadataReader metadataReader)
        : base(standaloneSignature, metadataReader)
    {
        Debug.Assert(standaloneSignature.GetKind() == StandaloneSignatureKind.Method);
    }

    public override StandaloneSignatureKind Kind => StandaloneSignatureKind.Method;

    public ref readonly MethodSignature<MetadataInfo> Signature
    {
        get
        {
            if (!_isSignatureInitialized)
            {
                _signature = StandaloneSignature.DecodeMethodSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                _isSignatureInitialized = true;
            }

            return ref _signature;
        }
    }


    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        var signature = Signature;
        _ = builder.Append("method ");

        switch (signature.Header.CallingConvention)
        {
            case SignatureCallingConvention.Default:
            {
                break;
            }

            case SignatureCallingConvention.CDecl:
            {
                _ = builder.Append("unmanaged cdecl ");
                break;
            }

            case SignatureCallingConvention.StdCall:
            {
                _ = builder.Append("unmanaged stdcall ");
                break;
            }

            case SignatureCallingConvention.ThisCall:
            {
                _ = builder.Append("unmanaged thiscall ");
                break;
            }

            case SignatureCallingConvention.FastCall:
            {
                _ = builder.Append("unmanaged fastcall ");
                break;
            }

            case SignatureCallingConvention.VarArgs:
            {
                _ = builder.Append("unmanaged varargs ");
                break;
            }

            case SignatureCallingConvention.Unmanaged:
            {
                _ = builder.Append("unmanaged ");
                break;
            }

            default:
            {
                ThrowForInvalidKind(signature.Header.CallingConvention);
                break;
            }
        }

        _ = builder.Append(signature.ReturnType);
        _ = builder.Append(" *");

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
