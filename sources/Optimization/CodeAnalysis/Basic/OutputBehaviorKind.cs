// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace TerraFX.Optimization.CodeAnalysis;

/// <summary>Describes the amount and type of data pushed as the result of the an instruction.</summary>
public enum OutputBehaviorKind
{
    /// <summary>No output value.</summary>
    Push0,

    /// <summary>One output value, type defined by data flow.</summary>
    Push1,

    /// <summary>Two output values, type defined by data flow.</summary>
    Push1_Push1,

    /// <summary>Push one native integer or pointer.</summary>
    PushI,

    /// <summary>Push one 8-byte integer.</summary>
    PushI8,

    /// <summary>Push one 4-byte floating point number.</summary>
    PushR4,

    /// <summary>Push one 8-byte floating point number.</summary>
    PushR8,

    /// <summary>Push one object reference.</summary>
    PushRef,

    /// <summary>Variable number of items pushed.</summary>
    VarPush,
}
