// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis
{
    public sealed class Instruction
    {
        private Instruction? _next;
        private Instruction? _previous;

        internal Instruction(MetadataReader metadataReader, Opcode opcode, Operand operand)
        {
            MetadataReader = metadataReader;
            Opcode = opcode;
            Operand = operand;
        }

        public Instruction? Next => _next;

        public int Length => Opcode.EncodingLength + Operand.Size;

        public MetadataReader MetadataReader { get; }

        public Opcode Opcode { get; }

        public Operand Operand { get; }

        public Instruction? Previous => _previous;

        public int GetIndex()
        {
            int index = 0;
            var previous = _previous;

            while (previous != null)
            {
                index++;
            }
            return index;
        }

        public int GetOffset()
        {
            int offset = 0;
            var previous = _previous;

            while (previous != null)
            {
                offset += previous.Length;
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

            builder.Append("IL_");
            var offset = GetOffset();
            builder.Append(offset.ToString("X4"));

            builder.Append(':');
            builder.Append(' ', 2);

            var opcodeName = Opcode.Name;
            builder.Append(opcodeName);

            var operand = Operand.ToString();

            if (operand != string.Empty)
            {
                builder.Append(' ', 16 - opcodeName.Length);
                builder.Append(operand);
            }

            return builder.ToString();
        }
    }
}
