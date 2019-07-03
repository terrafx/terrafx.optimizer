// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using static TerraFX.Utilities.AssertionUtilities;
using static TerraFX.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis
{
    public sealed class InstructionFactory
    {
        public InstructionFactory(MetadataReader metadataReader)
        {
            MetadataReader = metadataReader;
        }

        public MetadataReader MetadataReader { get; }

        public Instruction Decode(MethodBodyBlock methodBody)
        {
            ThrowIfNull(methodBody, nameof(methodBody));

            var ilReader = methodBody.GetILReader();
            var rootInstruction = DecodeNext(ilReader);

            var instructionMap = new Dictionary<int, Instruction>();
            instructionMap.Add(0, rootInstruction);

            int currentOffset = rootInstruction.Length;
            var previousInstruction = rootInstruction;

            while (ilReader.RemainingBytes != 0)
            {
                var instruction = DecodeNext(ilReader);
                instruction.InsertAfter(previousInstruction);

                instructionMap.Add(currentOffset, instruction);
                currentOffset += instruction.Length;
            }

            foreach (var kvp in instructionMap)
            {
                int offset = kvp.Key;
                Instruction instruction = kvp.Value;

                var operand = instruction.Operand;
                var operandValue = operand.Value;

                switch (operand.Kind)
                {
                    case OperandKind.InlineBrTarget:
                    {
                        Assert(operandValue is int, "Expected a 4-byte signed branch target.");
                        var targetOffset = offset + instruction.Length + (int)operandValue!;
                        operandValue = instructionMap[targetOffset];
                        break;
                    }

                    case OperandKind.InlineSwitch:
                    {
                        Assert(operandValue is int[], "Expected an array of 4-byte signed branch targets.");

                        var targets = (int[])operandValue!;
                        var targetCount = targets.Length;

                        var baseOffset = offset + instruction.Length;
                        var targetInstructions = ImmutableArray.CreateBuilder<Instruction>(targetCount);

                        for (int i = 0; i < targets.Length; i++)
                        {
                            var targetOffset = baseOffset + targets[i];
                            var targetInstruction = instructionMap[targetOffset];
                            targetInstructions.Add(targetInstruction);
                        }

                        operandValue = targetInstructions.ToImmutable();
                        break;
                    }

                    case OperandKind.ShortInlineBrTarget:
                    {
                        Assert(operandValue is sbyte, "Expected a 1-byte signed branch target.");
                        var targetOffset = offset + instruction.Length + (sbyte)operandValue!;
                        operandValue = instructionMap[targetOffset];
                        break;
                    }
                }

                // This triggers the validation that the value is correct for the operand kind.
                operand.Value = operandValue;
            }

            return rootInstruction;
        }

        private Instruction DecodeNext(BlobReader ilReader)
        {
            int opcodeEncoding = ilReader.ReadByte();

            if (opcodeEncoding == 0xFE)
            {
                opcodeEncoding <<= 8;
                opcodeEncoding += ilReader.ReadByte();
            }

            var opcodeKind = (OpcodeKind)opcodeEncoding;
            var opcode = Opcode.Create(opcodeKind);

            var operandKind = opcode.OperandKind;
            var operandValue = (object?)null;

            switch (operandKind)
            {
                case OperandKind.InlineNone:
                {
                    break;
                }

                case OperandKind.InlineBrTarget:
                case OperandKind.InlineI:
                {
                    operandValue = ilReader.ReadInt32();
                    break;
                }

                case OperandKind.InlineField:
                case OperandKind.InlineMethod:
                case OperandKind.InlineTok:
                case OperandKind.InlineType:
                {
                    var token = ilReader.ReadInt32();
                    operandValue = MetadataTokens.EntityHandle(token);
                    break;
                }

                case OperandKind.InlineI8:
                {
                    operandValue = ilReader.ReadInt64();
                    break;
                }

                case OperandKind.InlineR:
                {
                    operandValue = ilReader.ReadDouble();
                    break;
                }

                case OperandKind.InlineSig:
                {
                    var rowNumber = ilReader.ReadInt32();
                    operandValue = MetadataTokens.StandaloneSignatureHandle(rowNumber);
                    break;
                }

                case OperandKind.InlineString:
                {
                    var offset = ilReader.ReadInt32();
                    operandValue = MetadataTokens.UserStringHandle(offset);
                    break;
                }

                case OperandKind.InlineSwitch:
                {
                    var count = ilReader.ReadUInt32();
                    var targets = new int[count];

                    for (int i = 0; i < count; i++)
                    {
                        targets[i] = ilReader.ReadInt32();
                    }

                    operandValue = targets;
                    break;
                }

                case OperandKind.InlineVar:
                {
                    operandValue = ilReader.ReadInt16();
                    break;
                }

                case OperandKind.ShortInlineBrTarget:
                case OperandKind.ShortInlineI:
                case OperandKind.ShortInlineVar:
                {
                    operandValue = ilReader.ReadSByte();
                    break;
                }

                case OperandKind.ShortInlineR:
                {
                    operandValue = ilReader.ReadSingle();
                    break;
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(opcode.OperandKind));
                }
            }

            var metadataReader = MetadataReader;
            var operand = new Operand(metadataReader, operandKind, operandValue);
            return new Instruction(metadataReader, opcode, operand);
        }
    }
}
