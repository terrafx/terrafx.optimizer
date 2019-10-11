// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using TerraFX.Utilities;
using static TerraFX.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis
{
    public struct Operand
    {
        private object? _value;

        internal Operand(MetadataReader metadataReader, OperandKind kind, object? value)
        {
            Kind = kind;
            MetadataReader = metadataReader;
            _value = value;
        }

        public OperandKind Kind { get; }

        public MetadataReader MetadataReader { get; }

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
            get => _value;

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
                        throw new ArgumentOutOfRangeException(nameof(Kind));
                    }
                }

                if (argumentOutOfRange)
                {
                    ThrowArgumentOutOfRangeException(nameof(value), value!);
                }
                _value = value;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var value = _value;

            if (value != null)
            {
                if (value is Instruction targetInstruction)
                {
                    AppendTargetInstruction(builder, targetInstruction);
                }
                else if (value is ImmutableArray<Instruction> targetInstructions)
                {
                    AppendTargetInstructions(builder, targetInstructions);
                }
                else if (value is FieldDefinition fieldDefinition)
                {
                    AppendFieldDefinition(builder, MetadataReader, fieldDefinition);
                }
                else if (value is MethodDefinition methodDefinition)
                {
                    AppendMethodDefinition(builder, MetadataReader, methodDefinition);
                }
                else if (value is MemberReference memberReference)
                {
                    AppendMemberReference(builder, MetadataReader, memberReference);
                }
                else if (value is StandaloneSignature standaloneSignature)
                {
                    AppendStandaloneSignature(builder, MetadataReader, standaloneSignature);
                }
                else if (value is TypeDefinition typeDefinition)
                {
                    AppendTypeDefinition(builder, MetadataReader, typeDefinition);
                }
                else if (value is TypeReference typeReference)
                {
                    AppendTypeReference(builder, MetadataReader, typeReference);
                }
                else if (value is TypeSpecification typeSpecification)
                {
                    AppendTypeSpecification(builder, MetadataReader, typeSpecification);
                }
                else
                {
                    _ = builder.Append(value);
                }
            }

            return builder.ToString();

            static void AppendFieldDefinition(StringBuilder builder, MetadataReader metadataReader, FieldDefinition fieldDefinition)
            {
                var declaringType = metadataReader.GetTypeDefinition(fieldDefinition.GetDeclaringType());
                AppendQualifiedName(builder, metadataReader, declaringType.Namespace, declaringType.Name);

                var name = metadataReader.GetString(fieldDefinition.Name);
                _ = builder.Append(name);
            }

            static void AppendMethodDefinition(StringBuilder builder, MetadataReader metadataReader, MethodDefinition methodDefinition)
            {
                var declaringType = metadataReader.GetTypeDefinition(methodDefinition.GetDeclaringType());
                AppendQualifiedName(builder, metadataReader, declaringType.Namespace, declaringType.Name);

                _ = builder.Append('.');

                var name = metadataReader.GetString(methodDefinition.Name);
                _ = builder.Append(name);
            }

            static void AppendMemberReference(StringBuilder builder, MetadataReader metadataReader, MemberReference memberReference)
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
                        AppendTypeSpecification(builder, metadataReader, typeSpecification);
                        break;
                    }

                    default:
                    {
                        ThrowInvalidOperationException(nameof(parent.Kind), parent.Kind);
                        break;
                    }
                }

                _ = builder.Append('.');

                var name = metadataReader.GetString(memberReference.Name);
                _ = builder.Append(name);
            }

            static void AppendModuleReference(StringBuilder builder, MetadataReader metadataReader, ModuleReference moduleReference)
            {
                var name = metadataReader.GetString(moduleReference.Name);
                _ = builder.Append(name);
            }

            static void AppendOffset(StringBuilder builder, Instruction instruction)
            {
                _ = builder.Append("IL_");
                var offset = instruction.GetOffset();
                _ = builder.Append(offset.ToString("X4"));
            }

            static void AppendQualifiedName(StringBuilder builder, MetadataReader metadataReader, StringHandle namespaceNameHandle, StringHandle nameHandle)
            {
                var namespaceName = metadataReader.GetString(namespaceNameHandle);

                if (string.IsNullOrWhiteSpace(namespaceName) == false)
                {
                    _ = builder.Append(namespaceName);
                    _ = builder.Append('.');
                }

                var name = metadataReader.GetString(nameHandle);
                _ = builder.Append(name);
            }

            static void AppendStandaloneSignature(StringBuilder builder, MetadataReader metadataReader, StandaloneSignature standaloneSignature)
            {
                Debug.WriteLine("StandaloneSignatures are not supported.");
                _ = builder.Append(standaloneSignature);
            }

            static void AppendTargetInstruction(StringBuilder builder, Instruction targetInstruction)
            {
                AppendOffset(builder, targetInstruction);
            }

            static void AppendTargetInstructions(StringBuilder builder, ImmutableArray<Instruction> targetInstructions)
            {
                _ = builder.Append('(');
                var lastIndex = targetInstructions.Length - 1;

                for (int i = 0; i < lastIndex; i++)
                {
                    AppendTargetInstruction(builder, targetInstructions[i]);
                    _ = builder.Append(',');
                    _ = builder.Append(' ');
                }

                AppendTargetInstruction(builder, targetInstructions[lastIndex]);
                _ = builder.Append(')');
            }

            static void AppendTypeDefinition(StringBuilder builder, MetadataReader metadataReader, TypeDefinition typeDefinition)
            {
                AppendQualifiedName(builder, metadataReader, typeDefinition.Namespace, typeDefinition.Name);
            }

            static void AppendTypeReference(StringBuilder builder, MetadataReader metadataReader, TypeReference typeReference)
            {
                AppendQualifiedName(builder, metadataReader, typeReference.Namespace, typeReference.Name);
            }

            static void AppendTypeSpecification(StringBuilder builder, MetadataReader metadataReader, TypeSpecification typeSpecification)
            {
                Debug.WriteLine("TypeSpecifications are not supported.");
                _ = builder.Append(typeSpecification);
            }
        }
    }
}
