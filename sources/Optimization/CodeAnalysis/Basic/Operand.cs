// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public struct Operand
{
    private readonly OperandKind _kind;
    private readonly MetadataReader _metadataReader;
    private object? _value;

    internal Operand(MetadataReader metadataReader, OperandKind kind, object? value)
    {
        _kind = kind;
        _metadataReader = metadataReader;
        _value = value;
    }

    public OperandKind Kind => _kind;

    public MetadataReader MetadataReader => _metadataReader;

    public int Size
    {
        get
        {
            var size = -1;

            switch (Kind)
            {
                case OperandKind.InlineNone:
                {
                    size = 0;
                    break;
                }

                case OperandKind.InlineBrTarget:
                case OperandKind.InlineField:
                case OperandKind.InlineI:
                case OperandKind.InlineMethod:
                case OperandKind.InlineSig:
                case OperandKind.InlineString:
                case OperandKind.InlineTok:
                case OperandKind.InlineType:
                case OperandKind.ShortInlineR:
                {
                    size = 4;
                    break;
                }

                case OperandKind.InlineI8:
                case OperandKind.InlineR:
                {
                    size = 8;
                    break;
                }


                case OperandKind.InlineSwitch:
                {
                    var count = (Value is ImmutableArray<Instruction> immutableTargets) ? immutableTargets.Length : ((int[])Value!).Length;
                    size = 4 + (count * 4);
                    break;
                }

                case OperandKind.InlineVar:
                {
                    size = 2;
                    break;
                }

                case OperandKind.ShortInlineBrTarget:
                case OperandKind.ShortInlineI:
                case OperandKind.ShortInlineVar:
                {
                    size = 1;
                    break;
                }

                default:
                {
                    ThrowForInvalidKind(Kind);
                    break;
                }
            }

            return size;
        }
    }

    public object? Value
    {
        get
        {
            return _value;
        }

        set
        {
            var argumentOutOfRange = true;

            switch (Kind)
            {
                case OperandKind.InlineNone:
                {
                    argumentOutOfRange = value is not null;
                    break;
                }

                case OperandKind.InlineBrTarget:
                case OperandKind.ShortInlineBrTarget:
                {
                    argumentOutOfRange = value?.GetType() != typeof(Instruction);
                    break;
                }

                case OperandKind.InlineField:
                {
                    if (value is EntityHandle entityHandle)
                    {
                        if (entityHandle.Kind == HandleKind.MemberReference)
                        {
                            var memberReferenceInfo = CompilerInfo.Instance.Resolve((MemberReferenceHandle)entityHandle, MetadataReader);

                            if (memberReferenceInfo is FieldReferenceInfo fieldReferenceInfo)
                            {
                                value = fieldReferenceInfo;
                                argumentOutOfRange = false;
                            }
                        }
                        else if (entityHandle.Kind == HandleKind.FieldDefinition)
                        {
                            value = CompilerInfo.Instance.Resolve((FieldDefinitionHandle)entityHandle, MetadataReader);
                            argumentOutOfRange = false;
                        }
                    }
                    break;
                }

                case OperandKind.InlineI:
                {
                    if (value?.GetType() == typeof(uint))
                    {
                        value = (int)(uint)value;
                    }
                    argumentOutOfRange = value?.GetType() != typeof(int);
                    break;
                }

                case OperandKind.InlineI8:
                {
                    if (value?.GetType() == typeof(ulong))
                    {
                        value = (long)(ulong)value;
                    }
                    argumentOutOfRange = value?.GetType() != typeof(long);
                    break;
                }

                case OperandKind.InlineMethod:
                {
                    if (value is EntityHandle entityHandle)
                    {
                        if (entityHandle.Kind == HandleKind.MemberReference)
                        {
                            var memberReferenceInfo = CompilerInfo.Instance.Resolve((MemberReferenceHandle)entityHandle, MetadataReader);

                            if (memberReferenceInfo is MethodReferenceInfo methodReferenceInfo)
                            {
                                value = methodReferenceInfo;
                                argumentOutOfRange = false;
                            }
                        }
                        else if (entityHandle.Kind == HandleKind.MethodDefinition)
                        {
                            value = CompilerInfo.Instance.Resolve((MethodDefinitionHandle)entityHandle, MetadataReader);
                            argumentOutOfRange = false;
                        }
                        else if (entityHandle.Kind == HandleKind.MethodSpecification)
                        {
                            value = CompilerInfo.Instance.Resolve((MethodSpecificationHandle)entityHandle, MetadataReader);
                            argumentOutOfRange = false;
                        }
                    }
                    break;
                }

                case OperandKind.InlineR:
                {
                    argumentOutOfRange = value?.GetType() != typeof(double);
                    break;
                }

                case OperandKind.InlineSig:
                {
                    if (value is StandaloneSignatureHandle standaloneSignatureHandle)
                    {
                        value = CompilerInfo.Instance.Resolve(standaloneSignatureHandle, MetadataReader);
                        argumentOutOfRange = false;
                    }
                    break;
                }

                case OperandKind.InlineString:
                {
                    if (value is UserStringHandle userStringHandle)
                    {
                        var userString = MetadataReader.GetUserString(userStringHandle);
                        value = userString;
                        argumentOutOfRange = false;
                    }
                    break;
                }

                case OperandKind.InlineSwitch:
                {
                    argumentOutOfRange = value?.GetType() != typeof(ImmutableArray<Instruction>);
                    break;
                }

                case OperandKind.InlineTok:
                {
                    if (value is EntityHandle entityHandle)
                    {
                        value = CompilerInfo.Instance.Resolve(entityHandle, MetadataReader);
                        argumentOutOfRange = false;
                    }
                    break;
                }

                case OperandKind.InlineType:
                {
                    if (value is EntityHandle entityHandle)
                    {
                        if (entityHandle.Kind == HandleKind.TypeDefinition)
                        {
                            value = CompilerInfo.Instance.Resolve((TypeDefinitionHandle)entityHandle, MetadataReader);
                            argumentOutOfRange = false;
                        }
                        else if (entityHandle.Kind == HandleKind.TypeReference)
                        {
                            value = CompilerInfo.Instance.Resolve((TypeReferenceHandle)entityHandle, MetadataReader);
                            argumentOutOfRange = false;
                        }
                        else if (entityHandle.Kind == HandleKind.TypeSpecification)
                        {
                            value = CompilerInfo.Instance.Resolve((TypeSpecificationHandle)entityHandle, MetadataReader);
                            argumentOutOfRange = false;
                        }
                    }
                    break;
                }

                case OperandKind.InlineVar:
                {
                    if (value?.GetType() == typeof(ushort))
                    {
                        value = (short)(ushort)value;
                    }
                    argumentOutOfRange = value?.GetType() != typeof(short);
                    break;
                }

                case OperandKind.ShortInlineI:
                case OperandKind.ShortInlineVar:
                {
                    if (value?.GetType() == typeof(byte))
                    {
                        value = (sbyte)(byte)value;
                    }
                    argumentOutOfRange = value?.GetType() != typeof(sbyte);
                    break;
                }

                case OperandKind.ShortInlineR:
                {
                    argumentOutOfRange = value?.GetType() != typeof(float);
                    break;
                }

                default:
                {
                    ThrowForInvalidKind(Kind);
                    break;
                }
            }

            if (argumentOutOfRange)
            {
                ThrowForUnsupportedValue(value);
            }
            _value = value;
        }
    }

    public double AsDouble() => (double)Value!;

    public FieldDefinitionInfo AsFieldDefinitionInfo() => (FieldDefinitionInfo)Value!;
    
    public Instruction AsInstruction() => (Instruction)Value!;

    public ImmutableArray<Instruction> AsInstructions() => (ImmutableArray<Instruction>)Value!;

    public sbyte AsInt8() => (sbyte)Value!;

    public short AsInt16() => (short)Value!;

    public int AsInt32() => (int)Value!;

    public long AsInt64() => (long)Value!;

    public MemberReferenceInfo AsMemberReferenceInfo() => (MemberReferenceInfo)Value!;

    public MetadataInfo AsMetadataInfo() => (MetadataInfo)Value!;

    public MethodDefinitionInfo AsMethodDefinitionInfo() => (MethodDefinitionInfo)Value!;

    public MethodSpecificationInfo AsMethodSpecificationInfo() => (MethodSpecificationInfo)Value!;

    public float AsSingle() => (float)Value!;

    public StandaloneSignatureInfo AsStandaloneSignatureInfo() => (StandaloneSignatureInfo)Value!;

    public string AsString() => (string)Value!;

    public TypeDefinitionInfo AsTypeDefinitionInfo() => (TypeDefinitionInfo)Value!;

    public TypeReferenceInfo AsTypeReferenceInfo() => (TypeReferenceInfo)Value!;

    public TypeSpecificationInfo AsTypeSpecificationInfo() => (TypeSpecificationInfo)Value!;

    public byte AsUInt8() => (byte)(sbyte)Value!;

    public ushort AsUInt16() => (ushort)(short)Value!;

    public uint AsUInt32() => (uint)(int)Value!;

    public ulong AsUInt64() => (ulong)(long)Value!;

    public override string ToString()
    {
        var value = _value;

        if (_value is null)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        if (value is Instruction targetInstruction)
        {
            _ = AppendOffset(builder, targetInstruction);
        }
        else if (value is ImmutableArray<Instruction> targetInstructions)
        {
            _ = builder.Append('(');
            var lastIndex = targetInstructions.Length - 1;

            for (var i = 0; i < lastIndex; i++)
            {
                _ = AppendOffset(builder, targetInstructions[i]);
                _ = builder.Append(',');
                _ = builder.Append(' ');
            }

            _ = AppendOffset(builder, targetInstructions[lastIndex]);
            _ = builder.Append(')');
        }
        else if (value is FieldDefinitionInfo fieldDefinitionInfo)
        {
            _ = builder.Append(fieldDefinitionInfo);
        }
        else if (value is MethodDefinitionInfo methodDefinitionInfo)
        {
            _ = builder.Append(methodDefinitionInfo);
        }
        else if (value is MemberReferenceInfo memberReferenceInfo)
        {
            _ = builder.Append(memberReferenceInfo);
        }
        else if (value is MethodSpecificationInfo methodSpecificationInfo)
        {
            _ = builder.Append(methodSpecificationInfo);
        }
        else if (value is StandaloneSignatureInfo standaloneSignatureInfo)
        {
            _ = builder.Append(standaloneSignatureInfo);
        }
        else if (value is TypeDefinitionInfo typeDefinitionInfo)
        {
            _ = builder.Append(typeDefinitionInfo);
        }
        else if (value is TypeReferenceInfo typeReferenceInfo)
        {
            _ = builder.Append(typeReferenceInfo);
        }
        else if (value is TypeSpecificationInfo typeSpecificationInfo)
        {
            _ = builder.Append(typeSpecificationInfo);
        }
        else if (value is float float32Value)
        {
            _ = builder.Append(float32Value);
        }
        else if (value is double float64Value)
        {
            _ = builder.Append(float64Value);
        }
        else if (value is sbyte int8Value)
        {
            _ = builder.Append(int8Value);
        }
        else if (value is short int16Value)
        {
            _ = builder.Append(int16Value);
        }
        else if (value is int int32Value)
        {
            _ = builder.Append(int32Value);
        }
        else if (value is long int64Value)
        {
            _ = builder.Append(int64Value);
        }
        else if (value is string stringValue)
        {
            _ = builder.Append('"');
            _ = builder.Append(stringValue);
            _ = builder.Append('"');
        }
        else
        {
            ThrowUnreachableException();
        }

        return builder.ToString();

        static StringBuilder AppendOffset(StringBuilder builder, Instruction instruction)
        {
            _ = builder.Append("IL_");
            var offset = instruction.Offset;
            return builder.Append(offset.ToString("X4"));
        }
    }
}
