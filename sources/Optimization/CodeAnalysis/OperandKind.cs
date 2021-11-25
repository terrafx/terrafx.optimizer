// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace TerraFX.Optimization.CodeAnalysis;

/// <summary>Describes the type of inline argument to an instruction.</summary>
public enum OperandKind
{
    /// <summary>No in-line argument.</summary>
    InlineNone,

    /// <summary>Branch target, represented as a 4-byte signed integer from the beginning of the instruction following the current instruction.</summary>
    InlineBrTarget,

    /// <summary>Metadata token (4 bytes) representing a FieldRef (i.e., a MemberRef to a field) or FieldDef.</summary>
    InlineField,

    /// <summary>4-byte integer.</summary>
    InlineI,

    /// <summary>8-byte integer.</summary>
    InlineI8,

    /// <summary>Metadata token (4 bytes) representing a MethodRef (i.e., a MemberRef to a method) or MethodDef point number.</summary>
    InlineMethod,

    /// <summary>8-byte floating point number.</summary>
    InlineR,

    /// <summary>Metadata token (4 bytes) representing a standalone signature.</summary>
    InlineSig,

    /// <summary>Metadata token (4 bytes) representing a UserString.</summary>
    InlineString,

    /// <summary>Special for the switch instructions.</summary>
    InlineSwitch,

    /// <summary>Arbitrary metadata token (4 bytes), used for ldtoken instruction.</summary>
    InlineTok,

    /// <summary>Metadata token (4 bytes) representing a TypeDef, TypeRef, or TypeSpec.</summary>
    InlineType,

    /// <summary>2-byte integer representing an argument or local variable.</summary>
    InlineVar,

    /// <summary>Short branch target, represented as 1 signed byte from the beginning of the instruction following the current instruction.</summary>
    ShortInlineBrTarget,

    /// <summary>1-byte integer, signed or unsigned depending on instruction.</summary>
    ShortInlineI,

    /// <summary>4-byte floating point number.</summary>
    ShortInlineR,

    /// <summary>1-byte integer representing an argument or local variable.</summary>
    ShortInlineVar,
}
