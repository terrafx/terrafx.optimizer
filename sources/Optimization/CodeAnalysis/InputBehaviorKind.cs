// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace TerraFX.Optimization.CodeAnalysis
{
    /// <summary>Describes the data removed from the stack to compute an operations result.</summary>
    public enum InputBehaviorKind
    {
        /// <summary>No inputs</summary>
        Pop0,

        /// <summary>One value type specified by data flow.</summary>
        Pop1,

        /// <summary>Two input values, types specified by data flow.</summary>
        Pop1_Pop1,

        /// <summary>One machine-sized integer.</summary>
        PopI,

        /// <summary>Top of stack is described by data flow, next item is a native pointer.</summary>
        PopI_Pop1,

        /// <summary>Top two items on stack are integers (size can vary by instruction).</summary>
        PopI_PopI,

        PopI_PopI8,

        /// <summary>Top three items on stack are machine-sized integers.</summary>
        PopI_PopI_PopI,

        /// <summary>Top of stack is an 8-byte integer, next is a native pointer.</summary>
        PopI8_Pop8,

        /// <summary>Top of stack is a 4-byte floating point number, next is a native pointer.</summary>
        PopI_PopR4,

        /// <summary>Top of stack is an 8-byte floating point number, next is a native pointer.</summary>
        PopI_PopR8,

        /// <summary>Top of stack is an object reference.</summary>
        PopRef,

        PopRef_Pop1,

        /// <summary>Top of stack is an integer (size can vary by instruction), next is an object reference.</summary>
        PopRef_PopI,

        PopRef_PopI_Pop1,

        /// <summary>Top of stack has two integers (size can vary by instruction), next is an object reference.</summary>
        PopRef_PopI_PopI,

        /// <summary>Top of stack is an 8-byte integer, then a native-sized integer, then an object reference.</summary>
        PopRef_PopI_PopI8,

        /// <summary>Top of stack is an 4-byte floating point number, then a native-sized integer, then an object reference.</summary>
        PopRef_PopI_PopR4,

        /// <summary>Top of stack is an 8-byte floating point number, then a native-sized integer, then an object reference.</summary>
        PopRef_PopI_PopR8,

        PopRef_PopI_PopRef,

        /// <summary>Variable number of items used.</summary>
        VarPop,
    }
}
