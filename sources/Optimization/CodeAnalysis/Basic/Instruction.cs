// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed partial class Instruction(Opcode opcode, Operand operand) : IComparable, IComparable<Instruction>, IEquatable<Instruction>
{
    private readonly Opcode _opcode = opcode;

    private Instruction? _next;
    private Operand _operand = operand;
    private Instruction? _previous;

    private int _index = -1;
    private int _offset = -1;

    private bool _isReadonly;

    public int Index
    {
        get
        {
            var index = _index;

            if (index == -1)
            {
                var previous = _previous;
                index = 0;

                if (previous is not null)
                {
                    index = previous.Index + 1;
                }

                if (IsReadOnly)
                {
                    _index = index;
                }
            }

            return index;
        }
    }

    public bool IsReadOnly => _isReadonly;

    public int Length => Opcode.EncodingLength + Operand.Size;

    public Instruction? Next => _next;

    public int Offset
    {
        get
        {
            var offset = _offset;

            if (offset == -1)
            {
                var previous = _previous;
                offset = 0;

                if (previous is not null)
                {
                    offset = previous.Offset + previous.Length;
                }

                if (IsReadOnly)
                {
                    _offset = offset;
                }
            }

            return offset;
        }
    }

    public Opcode Opcode => _opcode;

    public Operand Operand => _operand;

    public Instruction? Previous => _previous;

    public static bool operator ==(Instruction? left, Instruction? right) => ReferenceEquals(left, right);

    public static bool operator !=(Instruction? left, Instruction? right) => !(left == right);

    public static bool operator <(Instruction? left, Instruction? right)
        => (left is null) ? (right is not null) : (left.CompareTo(right) < 0);

    public static bool operator <=(Instruction? left, Instruction? right)
        => (left is null) || (left.CompareTo(right) <= 0);

    public static bool operator >(Instruction? left, Instruction? right)
        => (left is not null) && (left.CompareTo(right) > 0);

    public static bool operator >=(Instruction? left, Instruction? right)
        => (left is null) ? (right is null) : (left.CompareTo(right) >= 0);

    public static Instruction Decode(MetadataReader metadataReader, MethodBodyBlock methodBody)
    {
        ArgumentNullException.ThrowIfNull(methodBody);

        var ilReader = methodBody.GetILReader();
        var rootInstruction = DecodeNext(metadataReader, ref ilReader);

        var instructions = new SortedDictionary<int, Instruction> {
            [0] = rootInstruction,
        };

        var previousInstruction = rootInstruction;

        while (ilReader.RemainingBytes != 0)
        {
            var instruction = DecodeNext(metadataReader, ref ilReader);
            instruction.InsertAfter(previousInstruction);
            previousInstruction.Freeze();

            instructions.Add(instruction.Offset, instruction);
            previousInstruction = instruction;
        }
        previousInstruction.Freeze();

        foreach (var kvp in instructions)
        {
            var offset = kvp.Key;
            var instruction = kvp.Value;

            Debug.Assert(offset == instruction.Offset);
            var operandValue = instruction.Operand.Value;

            switch (instruction.Operand.Kind)
            {
                case OperandKind.InlineBrTarget:
                {
                    Debug.Assert(operandValue is int, "Expected a 4-byte signed branch target.");
                    var targetOffset = offset + instruction.Length + (int)operandValue!;
                    operandValue = instructions[targetOffset];
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
                        var targetInstruction = instructions[targetOffset];
                        targetInstructions.Add(targetInstruction);
                    }

                    operandValue = targetInstructions.ToImmutable();
                    break;
                }

                case OperandKind.ShortInlineBrTarget:
                {
                    Debug.Assert(operandValue is sbyte, "Expected a 1-byte signed branch target.");
                    var targetOffset = offset + instruction.Length + (sbyte)operandValue!;
                    operandValue = instructions[targetOffset];
                    break;
                }
            }

            // This triggers the validation that the value is correct for the operand kind.
            instruction._operand.Value = operandValue;
        }

        return rootInstruction;

        static Instruction DecodeNext(MetadataReader metadataReader, ref BlobReader ilReader)
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
                    ThrowForInvalidKind(opcode.OperandKind);
                    break;
                }
            }

            var operand = new Operand(metadataReader, operandKind, operandValue);
            return new Instruction(opcode, operand);
        }
    }

    public int CompareTo(Instruction? other)
    {
        if (this == other)
        {
            return 0;
        }
        else if (other is not null)
        {
            return Index.CompareTo(other.Index);
        }
        return 1;
    }

    public int CompareTo(object? obj)
    {
        if (obj is Instruction other)
        {
            return CompareTo(other);
        }
        else if (obj is not null)
        {
            ThrowForInvalidType(obj.GetType(), typeof(Instruction));
        }
        return 1;
    }

    public bool Equals(Instruction? other) => this == other;

    public override bool Equals(object? obj) => (obj is Instruction other) && Equals(other);

    public override int GetHashCode() => base.GetHashCode();

    public void Freeze()
    {
        _isReadonly = true;
    }

    public void InsertAfter(Instruction instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction);

        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(Instruction));
        }
        var next = instruction._next;

        if (next is not null)
        {
            next._previous = this;
            _next = next;
        }
        instruction._next = this;
        _previous = instruction;
    }

    public void InsertBefore(Instruction instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction);

        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(Instruction));
        }
        var previous = instruction._previous;

        if (previous is not null)
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
        _ = builder.Append(Offset.ToString("X4", CultureInfo.InvariantCulture));

        _ = builder.Append(':');
        _ = builder.Append(' ', 2);

        var opcodeName = Opcode.Name;
        _ = builder.Append(opcodeName);

        var operand = Operand.ToString();

        if (!string.IsNullOrEmpty(operand))
        {
            _ = builder.Append(' ', 16 - opcodeName.Length);
            _ = builder.Append(operand);
        }

        return builder.ToString();
    }
}
