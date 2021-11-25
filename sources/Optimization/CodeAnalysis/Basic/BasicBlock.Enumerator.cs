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
            if (firstInstruction is null)
            {
                throw new ArgumentNullException(nameof(firstInstruction));
            }

            _firstInstruction = firstInstruction;
            _lastInstruction = lastInstruction;

            _currentInstruction = null;
        }

        public Instruction Current => _currentInstruction!;

        object? IEnumerator.Current => Current;

        public void Dispose()
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
