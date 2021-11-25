// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public struct OperandStringBuilder : ISignatureTypeProvider<string, object?>
{
    public static void AppendFieldDefinition(StringBuilder builder, MetadataReader metadataReader, FieldDefinition fieldDefinition)
    {
        var declaringType = metadataReader.GetTypeDefinition(fieldDefinition.GetDeclaringType());
        AppendTypeDefinition(builder, metadataReader, declaringType);

        _ = builder.Append('.');
        AppendName(builder, metadataReader, fieldDefinition.Name);

        var signatureTypeProvider = new OperandStringBuilder();
        _ = fieldDefinition.DecodeSignature(signatureTypeProvider, genericContext: fieldDefinition);
    }

    public static void AppendGenericParameter(StringBuilder builder, MetadataReader metadataReader, GenericParameterHandle genericParameterHandle)
    {
        var genericParameter = metadataReader.GetGenericParameter(genericParameterHandle);
        AppendName(builder, metadataReader, genericParameter.Name);
    }

    public static void AppendGenericParameters(StringBuilder builder, MetadataReader metadataReader, IReadOnlyCollection<GenericParameterHandle> genericParameterHandles)
    {
        var enumerator = genericParameterHandles.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return;
        }

        _ = builder.Append('<');

        AppendGenericParameter(builder, metadataReader, enumerator.Current);

        while (enumerator.MoveNext())
        {
            _ = builder.Append(", ");
            AppendGenericParameter(builder, metadataReader, enumerator.Current);
        }

        _ = builder.Append('>');
    }

    public static void AppendMethodDefinition(StringBuilder builder, MetadataReader metadataReader, MethodDefinition methodDefinition)
    {
        if (builder.Length == 0)
        {
            var declaringType = metadataReader.GetTypeDefinition(methodDefinition.GetDeclaringType());
            AppendTypeDefinition(builder, metadataReader, declaringType);
        }

        _ = builder.Append('.');
        AppendName(builder, metadataReader, methodDefinition.Name);

        var signatureTypeProvider = new OperandStringBuilder();
        var methodSignature = methodDefinition.DecodeSignature(signatureTypeProvider, genericContext: methodDefinition);

        var parameterHandles = methodDefinition.GetParameters();
        var genericParameterHandles = methodDefinition.GetGenericParameters();

        AppendMethodSignature(builder, metadataReader, methodSignature, genericParameterHandles, parameterHandles);
    }

    public static void AppendMethodSignature(StringBuilder builder, MetadataReader metadataReader, MethodSignature<string> methodSignature, IReadOnlyCollection<GenericParameterHandle> genericParameterHandles, IReadOnlyCollection<ParameterHandle> parameterHandles)
    {
        if (methodSignature.GenericParameterCount != 0)
        {
            _ = builder.Append('`');
            _ = builder.Append(methodSignature.GenericParameterCount);
        }

        AppendGenericParameters(builder, metadataReader, genericParameterHandles);

        _ = builder.Append('(');

        var typeEnumerator = methodSignature.ParameterTypes.GetEnumerator();
        var nameEnumerator = parameterHandles.GetEnumerator();

        if (typeEnumerator.MoveNext())
        {
            _ = builder.Append(typeEnumerator.Current);

            if (nameEnumerator.MoveNext())
            {
                _ = builder.Append(' ');
                var parameter = metadataReader.GetParameter(nameEnumerator.Current);
                AppendName(builder, metadataReader, parameter.Name);
            }
        }

        while (typeEnumerator.MoveNext())
        {
            _ = builder.Append(", ");
            _ = builder.Append(typeEnumerator.Current);

            if (nameEnumerator.MoveNext())
            {
                _ = builder.Append(' ');
                var parameter = metadataReader.GetParameter(nameEnumerator.Current);
                AppendName(builder, metadataReader, parameter.Name);
            }
        }

        _ = builder.Append(')');
        _ = builder.Append(':');

        _ = builder.Append(methodSignature.ReturnType);
    }

    public static void AppendMemberReference(StringBuilder builder, MetadataReader metadataReader, MemberReference memberReference)
    {
        var parent = memberReference.Parent;

        switch (parent.Kind)
        {
            case HandleKind.MethodDefinition:
            {
                var methodDefinition = metadataReader.GetMethodDefinition((MethodDefinitionHandle)parent);
                AppendMethodDefinition(builder, metadataReader, methodDefinition);
                break;
            }

            case HandleKind.ModuleReference:
            {
                var moduleReference = metadataReader.GetModuleReference((ModuleReferenceHandle)parent);
                AppendModuleReference(builder, metadataReader, moduleReference);
                break;
            }

            case HandleKind.TypeDefinition:
            {
                var typeDefinition = metadataReader.GetTypeDefinition((TypeDefinitionHandle)parent);
                AppendTypeDefinition(builder, metadataReader, typeDefinition);
                break;
            }

            case HandleKind.TypeReference:
            {
                var typeReference = metadataReader.GetTypeReference((TypeReferenceHandle)parent);
                AppendTypeReference(builder, metadataReader, typeReference);
                break;
            }

            case HandleKind.TypeSpecification:
            {
                var typeSpecification = metadataReader.GetTypeSpecification((TypeSpecificationHandle)parent);
                AppendTypeSpecification(builder, typeSpecification);
                break;
            }

            default:
            {
                throw new ArgumentOutOfRangeException(nameof(memberReference));
            }
        }

        _ = builder.Append('.');
        AppendName(builder, metadataReader, memberReference.Name);

        switch (memberReference.GetKind())
        {
            case MemberReferenceKind.Method:
            {
                var signatureTypeProvider = new OperandStringBuilder();
                var methodSignature = memberReference.DecodeMethodSignature(signatureTypeProvider, genericContext: memberReference);

                AppendMethodSignature(builder, metadataReader, methodSignature, Array.Empty<GenericParameterHandle>(), Array.Empty<ParameterHandle>());
                break;
            }

            case MemberReferenceKind.Field:
            {
                var signatureTypeProvider = new OperandStringBuilder();
                _ = memberReference.DecodeFieldSignature(signatureTypeProvider, genericContext: memberReference);
                break;
            }

            default:
            {
                throw new ArgumentOutOfRangeException(nameof(memberReference));
            }
        }
    }

    public static void AppendModuleReference(StringBuilder builder, MetadataReader metadataReader, ModuleReference moduleReference)
    {
        AppendName(builder, metadataReader, moduleReference.Name);
    }

    public static void AppendName(StringBuilder builder, MetadataReader metadataReader, StringHandle nameHandle, bool isQualifiedName = false)
    {
        var name = metadataReader.GetString(nameHandle);
        var lastPartStart = isQualifiedName ? -1 : name.LastIndexOf('.');

        if ((lastPartStart == -1) || (name == ".ctor") || (name == ".cctor"))
        {
            _ = builder.Append(name);
        }
        else
        {
            _ = builder.Append(name.AsSpan(lastPartStart + 1));
        }
    }

    public static void AppendOffset(StringBuilder builder, Instruction instruction)
    {
        _ = builder.Append("IL_");
        var offset = instruction.GetOffset();
        _ = builder.Append(offset.ToString("X4"));
    }

    public static void AppendQualifiedName(StringBuilder builder, MetadataReader metadataReader, StringHandle namespaceNameHandle, StringHandle nameHandle)
    {
        if (!namespaceNameHandle.IsNil)
        {
            AppendName(builder, metadataReader, namespaceNameHandle, isQualifiedName: true);
            _ = builder.Append('.');
        }

        AppendName(builder, metadataReader, nameHandle);
    }

    public static void AppendStandaloneSignature(StringBuilder builder, MetadataReader metadataReader, StandaloneSignature standaloneSignature)
    {
        switch (standaloneSignature.GetKind())
        {
            case StandaloneSignatureKind.Method:
            {
                var signatureTypeProvider = new OperandStringBuilder();
                var methodSignature = standaloneSignature.DecodeMethodSignature(signatureTypeProvider, genericContext: standaloneSignature);

                AppendMethodSignature(builder, metadataReader, methodSignature, Array.Empty<GenericParameterHandle>(), Array.Empty<ParameterHandle>());
                break;
            }

            case StandaloneSignatureKind.LocalVariables:
            {
                var signatureTypeProvider = new OperandStringBuilder();
                _ = standaloneSignature.DecodeLocalSignature(signatureTypeProvider, genericContext: standaloneSignature);
                break;
            }

            default:
            {
                throw new ArgumentOutOfRangeException(nameof(standaloneSignature));
            }
        }
    }

    public static void AppendTargetInstruction(StringBuilder builder, Instruction targetInstruction)
    {
        AppendOffset(builder, targetInstruction);
    }

    public static void AppendTargetInstructions(StringBuilder builder, ImmutableArray<Instruction> targetInstructions)
    {
        _ = builder.Append('(');
        var lastIndex = targetInstructions.Length - 1;

        for (var i = 0; i < lastIndex; i++)
        {
            AppendTargetInstruction(builder, targetInstructions[i]);
            _ = builder.Append(',');
            _ = builder.Append(' ');
        }

        AppendTargetInstruction(builder, targetInstructions[lastIndex]);
        _ = builder.Append(')');
    }

    public static void AppendTypeDefinition(StringBuilder builder, MetadataReader metadataReader, TypeDefinition typeDefinition)
    {
        if (typeDefinition.IsNested)
        {
            var declaringType = metadataReader.GetTypeDefinition(typeDefinition.GetDeclaringType());
            AppendTypeDefinition(builder, metadataReader, declaringType);

            _ = builder.Append('.');
            AppendName(builder, metadataReader, typeDefinition.Name);
        }
        else
        {
            AppendQualifiedName(builder, metadataReader, typeDefinition.Namespace, typeDefinition.Name);
        }

        var genericParameterHandles = typeDefinition.GetGenericParameters();
        AppendGenericParameters(builder, metadataReader, genericParameterHandles);
    }

    public static void AppendTypeReference(StringBuilder builder, MetadataReader metadataReader, TypeReference typeReference)
    {
        AppendQualifiedName(builder, metadataReader, typeReference.Namespace, typeReference.Name);
    }

    public static void AppendTypeSpecification(StringBuilder builder, TypeSpecification typeSpecification)
    {
        var signatureTypeProvider = new OperandStringBuilder();
        var signature = typeSpecification.DecodeSignature(signatureTypeProvider, genericContext: typeSpecification);
        _ = builder.Append(signature);
    }

    public string GetArrayType(string elementType, ArrayShape arrayShape)
    {
        var builder = new StringBuilder();
        _ = builder.Append('[');

        var wasSimple = AppendRank(builder, arrayShape, 0);

        for (var i = 1; i < arrayShape.Rank; i++)
        {
            _ = builder.Append(',');

            if (!wasSimple)
            {
                _ = builder.Append(' ');
            }
            wasSimple = AppendRank(builder, arrayShape, i);
        }

        _ = builder.Append(']');
        return builder.ToString();

        static bool AppendRank(StringBuilder builder, ArrayShape arrayShape, int index)
        {
            var wasSimple = true;

            var lowerBound = GetValue(arrayShape.LowerBounds, index);
            var size = GetValue(arrayShape.Sizes, index);

            if (lowerBound == 0)
            {
                if (size != 0)
                {
                    _ = builder.Append(size);
                    wasSimple = false;
                }
            }
            else
            {
                _ = builder.Append(lowerBound);
                wasSimple = false;

                _ = builder.Append("...");

                if (size != 0)
                {
                    _ = builder.Append(size - lowerBound);
                }
            }

            return wasSimple;
        }

        static int GetValue(ImmutableArray<int> array, int index)
        {
            return index < array.Length ? array[index] : 0;
        }
    }

    public string GetByReferenceType(string elementType) => elementType + '&';

    public string GetFunctionPointerType(MethodSignature<string> signature)
    {
        var builder = new StringBuilder();

        _ = builder.Append("delegate*");

        if (signature.Header.CallingConvention != SignatureCallingConvention.Default)
        {
            _ = builder.Append(" unmanaged");
        }

        switch (signature.Header.CallingConvention)
        {
            case SignatureCallingConvention.Default:
            {
                break;
            }

            case SignatureCallingConvention.CDecl:
            {
                _ = builder.Append("[Cdecl]");
                break;
            }

            case SignatureCallingConvention.StdCall:
            {
                _ = builder.Append("[Stdcall]");
                break;
            }

            case SignatureCallingConvention.ThisCall:
            {
                _ = builder.Append("[Thiscall]");
                break;
            }

            case SignatureCallingConvention.FastCall:
            {
                _ = builder.Append("[Fastcall]");
                break;
            }

            case SignatureCallingConvention.VarArgs:
            {
                _ = builder.Append("[Varargs]");
                break;
            }

            case SignatureCallingConvention.Unmanaged:
            {
                break;
            }

            default:
            {
                throw new ArgumentOutOfRangeException(nameof(signature));
            }
        }

        var parameterTypes = signature.ParameterTypes;
        _ = builder.Append('<');

        for (var i = 0; i < parameterTypes.Length; i++)
        {
            _ = builder.Append(parameterTypes[i]);
            _ = builder.Append(", ");
        }

        _ = builder.Append(signature.ReturnType);
        _ = builder.Append('>');

        return builder.ToString();
    }

    public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
    {
        if (genericType.EndsWith('>'))
        {
            return genericType;
        }

        var builder = new StringBuilder();

        _ = builder.Append(genericType);
        var enumerator = typeArguments.GetEnumerator();

        _ = enumerator.MoveNext();
        _ = builder.Append('<');

        _ = builder.Append(enumerator.Current);

        while (enumerator.MoveNext())
        {
            _ = builder.Append(", ");
            _ = builder.Append(enumerator.Current);
        }

        _ = builder.Append('>');

        return builder.ToString();
    }

    public string GetGenericMethodParameter(object? genericContext, int index)
    {
        if (genericContext is MemberReference or MethodDefinition)
        {
            return "!!" + index.ToString();
        }
        else if (genericContext is TypeSpecification)
        {
            return '!' + index.ToString();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(genericContext));
        }
    }

    public string GetGenericTypeParameter(object? genericContext, int index)
    {
        if (genericContext is MemberReference or MethodDefinition)
        {
            return "!!" + index.ToString();
        }
        else if (genericContext is TypeSpecification)
        {
            return '!' + index.ToString();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(genericContext));
        }
    }

    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
    {
        var builder = new StringBuilder();

        _ = builder.Append(modifier);
        _ = builder.Append(' ');
        _ = builder.Append(unmodifiedType);

        return builder.ToString();
    }

    public string GetPinnedType(string elementType) => "pinned " + elementType;

    public string GetPointerType(string elementType) => elementType + '*';

    public string GetPrimitiveType(PrimitiveTypeCode primitiveTypeCode)
    {
        switch (primitiveTypeCode)
        {
            case PrimitiveTypeCode.Void:
            {
                return "void";
            }

            case PrimitiveTypeCode.Boolean:
            {
                return "bool";
            }

            case PrimitiveTypeCode.Char:
            {
                return "char";
            }

            case PrimitiveTypeCode.SByte:
            {
                return "sbyte";
            }

            case PrimitiveTypeCode.Byte:
            {
                return "byte";
            }

            case PrimitiveTypeCode.Int16:
            {
                return "short";
            }

            case PrimitiveTypeCode.UInt16:
            {
                return "ushort";
            }

            case PrimitiveTypeCode.Int32:
            {
                return "int";
            }

            case PrimitiveTypeCode.UInt32:
            {
                return "uint";
            }

            case PrimitiveTypeCode.Int64:
            {
                return "long";
            }

            case PrimitiveTypeCode.UInt64:
            {
                return "ulong";
            }

            case PrimitiveTypeCode.Single:
            {
                return "float";
            }

            case PrimitiveTypeCode.Double:
            {
                return "double";
            }

            case PrimitiveTypeCode.String:
            {
                return "string";
            }

            case PrimitiveTypeCode.TypedReference:
            {
                return "TypedReference";
            }

            case PrimitiveTypeCode.IntPtr:
            {
                return "nint";
            }

            case PrimitiveTypeCode.UIntPtr:
            {
                return "nuint";
            }

            case PrimitiveTypeCode.Object:
            {
                return "object";
            }

            default:
            {
                throw new ArgumentOutOfRangeException(nameof(primitiveTypeCode));
            }
        }
    }

    public string GetSZArrayType(string elementType) => elementType + "[]";

    public string GetTypeFromDefinition(MetadataReader metadataReader, TypeDefinitionHandle typeDefinitionHandle, byte rawTypeKind)
    {
        var builder = new StringBuilder();

        var typeDefinition = metadataReader.GetTypeDefinition(typeDefinitionHandle);
        AppendTypeDefinition(builder, metadataReader, typeDefinition);

        return builder.ToString();
    }

    public string GetTypeFromReference(MetadataReader metadataReader, TypeReferenceHandle typeReferenceHandle, byte rawTypeKind)
    {
        var builder = new StringBuilder();

        var typeReference = metadataReader.GetTypeReference(typeReferenceHandle);
        AppendTypeReference(builder, metadataReader, typeReference);

        return builder.ToString();
    }

    public string GetTypeFromSpecification(MetadataReader metadataReader, object? genericContext, TypeSpecificationHandle typeSpecificationHandle, byte rawTypeKind)
    {
        var builder = new StringBuilder();

        var typeSpecification = metadataReader.GetTypeSpecification(typeSpecificationHandle);
        AppendTypeSpecification(builder, typeSpecification);

        return builder.ToString();
    }
}
