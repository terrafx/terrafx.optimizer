// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis
{
    public sealed class Instruction
    {
        private readonly Opcode _opcode;

        private Instruction? _next;
        private Operand _operand;
        private Instruction? _previous;

        private Instruction(Opcode opcode, Operand operand)
        {
            _opcode = opcode;
            _operand = operand;
        }

        public Instruction? Next => _next;

        public int Length => Opcode.EncodingLength + Operand.Size;

        public Opcode Opcode => _opcode;

        public Operand Operand => _operand;

        public Instruction? Previous => _previous;

        public static Instruction Decode(MetadataReader metadataReader, MethodBodyBlock methodBody)
        {
            if (methodBody is null)
            {
                throw new ArgumentNullException(nameof(methodBody));
            }

            var ilReader = methodBody.GetILReader();
            var rootInstruction = DecodeNext(metadataReader, ref ilReader);

            var instructionMap = new Dictionary<int, Instruction> {
                [0] = rootInstruction,
            };

            var currentOffset = rootInstruction.Length;
            var previousInstruction = rootInstruction;

            while (ilReader.RemainingBytes != 0)
            {
                var instruction = DecodeNext(metadataReader, ref ilReader);
                instruction.InsertAfter(previousInstruction);

                instructionMap.Add(currentOffset, instruction);
                currentOffset += instruction.Length;

                previousInstruction = instruction;
            }

            foreach (var kvp in instructionMap)
            {
                var offset = kvp.Key;
                var instruction = kvp.Value;

                var operandValue = instruction.Operand.Value;

                switch (instruction.Operand.Kind)
                {
                    case OperandKind.InlineBrTarget:
                    {
                        Debug.Assert(operandValue is int, "Expected a 4-byte signed branch target.");
                        var targetOffset = offset + instruction.Length + (int)operandValue!;
                        operandValue = instructionMap[targetOffset];
                        break;
                    }

                    case OperandKind.InlineSwitch:
                    {
                        Debug.Assert(operandValue is int[], "Expected an array of 4-byte signed branch targets.");

                        var targets = (int[])operandValue!;
                        var targetCount = targets.Length;

                        var baseOffset = offset + instruction.Length;
                        var targetInstructions = ImmutableArray.CreateBuilder<Instruction>(targetCount);

                        for (var i = 0; i < targets.Length; i++)
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
                        Debug.Assert(operandValue is sbyte, "Expected a 1-byte signed branch target.");
                        var targetOffset = offset + instruction.Length + (sbyte)operandValue!;
                        operandValue = instructionMap[targetOffset];
                        break;
                    }
                }

                // This triggers the validation that the value is correct for the operand kind.
                instruction._operand.Value = operandValue;
            }

            return rootInstruction;
        }

        private static Instruction DecodeNext(MetadataReader metadataReader, ref BlobReader ilReader)
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

                    for (var i = 0; i < count; i++)
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
                    throw new NotSupportedException(nameof(opcode.OperandKind));
                }
            }

            var operand = new Operand(metadataReader, operandKind, operandValue);
            return new Instruction(opcode, operand);
        }

        public int GetIndex()
        {
            var index = 0;
            var previous = _previous;

            while (previous != null)
            {
                index++;
            }
            return index;
        }

        public int GetOffset()
        {
            var offset = 0;

            for (var current = _previous; current != null; current = current.Previous)
            {
                offset += current.Length;

            }
            return offset;
        }

        public void InsertAfter(Instruction instruction)
        {
            var next = instruction._next;

            if (next != null)
            {
                next._previous = this;
                _next = next;
            }
            instruction._next = this;
            _previous = instruction;
        }

        public void InsertBefore(Instruction instruction)
        {
            var previous = instruction._previous;

            if (previous != null)
            {
                previous._next = this;
                _previous = previous;
            }
            instruction._previous = this;
            _next = instruction;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            _ = builder.Append("IL_");
            var offset = GetOffset();
            _ = builder.Append(offset.ToString("X4"));

            _ = builder.Append(':');
            _ = builder.Append(' ', 2);

            var opcodeName = Opcode.Name;
            _ = builder.Append(opcodeName);

            var operand = Operand.ToString();

            if (operand != string.Empty)
            {
                _ = builder.Append(' ', 16 - opcodeName.Length);
                _ = builder.Append(operand);
            }

            return builder.ToString();
        }
    }
}
