// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;

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
            int size;

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
                    throw new ArgumentOutOfRangeException(nameof(Kind));
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
                    argumentOutOfRange = value != null;
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
                            var memberReferenceHandle = (MemberReferenceHandle)entityHandle;
                            var memberReference = MetadataReader.GetMemberReference(memberReferenceHandle);

                            if (memberReference.GetKind() == MemberReferenceKind.Field)
                            {
                                value = memberReference;
                                argumentOutOfRange = false;
                            }
                        }
                        else if (entityHandle.Kind == HandleKind.FieldDefinition)
                        {
                            var fieldDefinitionHandle = (FieldDefinitionHandle)entityHandle;
                            value = MetadataReader.GetFieldDefinition(fieldDefinitionHandle);
                            argumentOutOfRange = false;
                        }
                    }
                    break;
                }

                case OperandKind.InlineI:
                {
                    argumentOutOfRange = value?.GetType() != typeof(int);
                    break;
                }

                case OperandKind.InlineI8:
                {
                    argumentOutOfRange = value?.GetType() != typeof(long);
                    break;
                }

                case OperandKind.InlineMethod:
                {
                    if (value is EntityHandle entityHandle)
                    {
                        if (entityHandle.Kind == HandleKind.MemberReference)
                        {
                            var memberReferenceHandle = (MemberReferenceHandle)entityHandle;
                            var memberReference = MetadataReader.GetMemberReference(memberReferenceHandle);

                            if (memberReference.GetKind() == MemberReferenceKind.Method)
                            {
                                value = memberReference;
                                argumentOutOfRange = false;
                            }
                        }
                        else if (entityHandle.Kind == HandleKind.MethodDefinition)
                        {
                            var methodDefinitionHandle = (MethodDefinitionHandle)entityHandle;
                            value = MetadataReader.GetMethodDefinition(methodDefinitionHandle);
                            argumentOutOfRange = false;
                        }
                        else if (entityHandle.Kind == HandleKind.MethodSpecification)
                        {
                            var methodSpecificationHandle = (MethodSpecificationHandle)entityHandle;
                            value = MetadataReader.GetMethodSpecification(methodSpecificationHandle);
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
                        value = MetadataReader.GetStandaloneSignature(standaloneSignatureHandle);
                        argumentOutOfRange = false;
                    }
                    break;
                }

                case OperandKind.InlineString:
                {
                    if (value is UserStringHandle userStringHandle)
                    {
                        value = MetadataReader.GetUserString(userStringHandle);
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
                    argumentOutOfRange = value?.GetType() != typeof(EntityHandle);
                    break;
                }

                case OperandKind.InlineType:
                {
                    if (value is EntityHandle entityHandle)
                    {
                        if (entityHandle.Kind == HandleKind.TypeDefinition)
                        {
                            var typeDefinitionHandle = (TypeDefinitionHandle)entityHandle;
                            value = MetadataReader.GetTypeDefinition(typeDefinitionHandle);
                            argumentOutOfRange = false;
                        }
                        else if (entityHandle.Kind == HandleKind.TypeReference)
                        {
                            var typeReferenceHandle = (TypeReferenceHandle)entityHandle;
                            value = MetadataReader.GetTypeReference(typeReferenceHandle);
                            argumentOutOfRange = false;
                        }
                        else if (entityHandle.Kind == HandleKind.TypeSpecification)
                        {
                            var typeSpecificationHandle = (TypeSpecificationHandle)entityHandle;
                            value = MetadataReader.GetTypeSpecification(typeSpecificationHandle);
                            argumentOutOfRange = false;
                        }
                    }
                    break;
                }

                case OperandKind.InlineVar:
                {
                    argumentOutOfRange = value?.GetType() != typeof(short);
                    break;
                }

                case OperandKind.ShortInlineI:
                case OperandKind.ShortInlineVar:
                {
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
                    throw new NotSupportedException(nameof(Kind));
                }
            }

            if (argumentOutOfRange)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _value = value;
        }
    }

    public override string ToString()
    {
        if (_value is not object value)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        if (value is Instruction targetInstruction)
        {
            OperandStringBuilder.AppendTargetInstruction(builder, targetInstruction);
        }
        else if (value is ImmutableArray<Instruction> targetInstructions)
        {
            OperandStringBuilder.AppendTargetInstructions(builder, targetInstructions);
        }
        else if (value is FieldDefinition fieldDefinition)
        {
            OperandStringBuilder.AppendFieldDefinition(builder, MetadataReader, fieldDefinition);
        }
        else if (value is MethodDefinition methodDefinition)
        {
            OperandStringBuilder.AppendMethodDefinition(builder, MetadataReader, methodDefinition);
        }
        else if (value is MemberReference memberReference)
        {
            OperandStringBuilder.AppendMemberReference(builder, MetadataReader, memberReference);
        }
        else if (value is StandaloneSignature standaloneSignature)
        {
            OperandStringBuilder.AppendStandaloneSignature(builder, MetadataReader, standaloneSignature);
        }
        else if (value is TypeDefinition typeDefinition)
        {
            OperandStringBuilder.AppendTypeDefinition(builder, MetadataReader, typeDefinition);
        }
        else if (value is TypeReference typeReference)
        {
            OperandStringBuilder.AppendTypeReference(builder, MetadataReader, typeReference);
        }
        else if (value is TypeSpecification typeSpecification)
        {
            OperandStringBuilder.AppendTypeSpecification(builder, typeSpecification);
        }
        else if (value is string)
        {
            _ = builder.Append('"');
            _ = builder.Append(value);
            _ = builder.Append('"');
        }
        else
        {
            _ = builder.Append(value);
        }

        return builder.ToString();
    }
}
