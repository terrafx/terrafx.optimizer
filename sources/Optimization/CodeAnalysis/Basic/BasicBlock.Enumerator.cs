// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TerraFX.Optimization.CodeAnalysis;

public partial class BasicBlock
{
    public struct Enumerator : IEnumerator<Instruction>, IEnumerator
    {
        private readonly Instruction _firstInstruction;
        private readonly Instruction? _lastInstruction;

        private Instruction? _currentInstruction;

        public Enumerator(Instruction firstInstruction, Instruction? lastInstruction)
        {
            ArgumentNullException.ThrowIfNull(firstInstruction);

            _firstInstruction = firstInstruction;
            _lastInstruction = lastInstruction;

            _currentInstruction = null;
        }

        public readonly Instruction Current => _currentInstruction!;

        readonly object? IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            var currentInstruction = _currentInstruction;

            if (currentInstruction is not null)
            {
                currentInstruction = (currentInstruction != _lastInstruction) ? currentInstruction.Next : null;
            }
            else
            {
                currentInstruction = _firstInstruction;
            }

            if (currentInstruction is not null)
            {
                _currentInstruction = currentInstruction;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _currentInstruction = null;
        }
    }
}
