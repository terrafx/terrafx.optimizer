// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class FunctionPointerTypeInfo : MetadataInfo
{
    private readonly MethodSignature<MetadataInfo> _signature;

    public FunctionPointerTypeInfo(MethodSignature<MetadataInfo> signature)
    {
        _signature = signature;
    }

    public ref readonly MethodSignature<MetadataInfo> Signature => ref _signature;

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
