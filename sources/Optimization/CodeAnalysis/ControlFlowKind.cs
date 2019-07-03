// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace TerraFX.Optimization.CodeAnalysis
{
    /// <summary>Describes the control flow implications of an instruction.</summary>
    public enum ControlFlowKind
    {
        /// <summary>Unconditional branch.</summary>
        Branch,

        Break,

        /// <summary>Method call.</summary>
        Call,

        /// <summary>Conditional branch.</summary>
        Cond_branch,

        /// <summary>Unused operation or prefix code.</summary>
        Meta,

        /// <summary>Control flow unaltered ("fall through").</summary>
        Next,

        /// <summary>Return from method.</summary>
        Return,

        /// <summary>Throw or rethrow an exception.</summary>
        Throw,
    }
}
