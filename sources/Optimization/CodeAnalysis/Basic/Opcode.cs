// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection.Metadata;
using static TerraFX.Optimization.CodeAnalysis.ControlFlowKind;
using static TerraFX.Optimization.CodeAnalysis.InputBehaviorKind;
using static TerraFX.Optimization.CodeAnalysis.OpcodeKind;
using static TerraFX.Optimization.CodeAnalysis.OperandKind;
using static TerraFX.Optimization.CodeAnalysis.OutputBehaviorKind;

namespace TerraFX.Optimization.CodeAnalysis;

public readonly struct Opcode : IEquatable<Opcode>
{
    private Opcode(OpcodeKind kind, string name, InputBehaviorKind inputBehavior, OutputBehaviorKind outputBehavior, OperandKind operandKind, int encodingLength, int encoding, ControlFlowKind controlFlow)
    {
        Kind = kind;
        Name = name;
        InputBehavior = inputBehavior;
        OutputBehavior = outputBehavior;
        OperandKind = operandKind;
        EncodingLength = encodingLength;
        Encoding = encoding;
        ControlFlow = controlFlow;
    }

    public ControlFlowKind ControlFlow { get; }

    public int Encoding { get; }

    public int EncodingLength { get; }

    public InputBehaviorKind InputBehavior { get; }

    public OpcodeKind Kind { get; }

    public string Name { get; }

    public OperandKind OperandKind { get; }

    public OutputBehaviorKind OutputBehavior { get; }

    public static bool operator ==(Opcode left, Opcode right) => left.Kind == right.Kind;

    public static bool operator !=(Opcode left, Opcode right) => left.Kind != right.Kind;

    public static Opcode Create(OpcodeKind kind) => kind switch {
        Nop => new Opcode(Nop, "nop", Pop0, Push0, InlineNone, 1, 0x0000, Next),
        OpcodeKind.Break => new Opcode(OpcodeKind.Break, "break", Pop0, Push0, InlineNone, 1, 0x0001, ControlFlowKind.Break),
        Ldarg_0 => new Opcode(Ldarg_0, "ldarg.0", Pop0, Push1, InlineNone, 1, 0x0002, Next),
        Ldarg_1 => new Opcode(Ldarg_1, "ldarg.1", Pop0, Push1, InlineNone, 1, 0x0003, Next),
        Ldarg_2 => new Opcode(Ldarg_2, "ldarg.2", Pop0, Push1, InlineNone, 1, 0x0004, Next),
        Ldarg_3 => new Opcode(Ldarg_3, "ldarg.3", Pop0, Push1, InlineNone, 1, 0x0005, Next),
        Ldloc_0 => new Opcode(Ldloc_0, "ldloc.0", Pop0, Push1, InlineNone, 1, 0x0006, Next),
        Ldloc_1 => new Opcode(Ldloc_1, "ldloc.1", Pop0, Push1, InlineNone, 1, 0x0007, Next),
        Ldloc_2 => new Opcode(Ldloc_2, "ldloc.2", Pop0, Push1, InlineNone, 1, 0x0008, Next),
        Ldloc_3 => new Opcode(Ldloc_3, "ldloc.3", Pop0, Push1, InlineNone, 1, 0x0009, Next),
        Stloc_0 => new Opcode(Stloc_0, "stloc.0", Pop1, Push0, InlineNone, 1, 0x000A, Next),
        Stloc_1 => new Opcode(Stloc_1, "stloc.1", Pop1, Push0, InlineNone, 1, 0x000B, Next),
        Stloc_2 => new Opcode(Stloc_2, "stloc.2", Pop1, Push0, InlineNone, 1, 0x000C, Next),
        Stloc_3 => new Opcode(Stloc_3, "stloc.3", Pop1, Push0, InlineNone, 1, 0x000D, Next),
        Ldarg_s => new Opcode(Ldarg_s, "ldarg.s", Pop0, Push1, ShortInlineVar, 1, 0x000E, Next),
        Ldarga_s => new Opcode(Ldarga_s, "ldarga.s", Pop0, PushI, ShortInlineVar, 1, 0x000F, Next),
        Starg_s => new Opcode(Starg_s, "starg.s", Pop1, Push0, ShortInlineVar, 1, 0x0010, Next),
        Ldloc_s => new Opcode(Ldloc_s, "ldloc.s", Pop0, Push1, ShortInlineVar, 1, 0x0011, Next),
        Ldloca_s => new Opcode(Ldloca_s, "ldloca.s", Pop0, PushI, ShortInlineVar, 1, 0x0012, Next),
        Stloc_s => new Opcode(Stloc_s, "stloc.s", Pop1, Push0, ShortInlineVar, 1, 0x0013, Next),
        Ldnull => new Opcode(Ldnull, "ldnull", Pop0, PushRef, InlineNone, 1, 0x0014, Next),
        Ldc_i4_m1 => new Opcode(Ldc_i4_m1, "ldc.i4.m1", Pop0, PushI, InlineNone, 1, 0x0015, Next),
        Ldc_i4_0 => new Opcode(Ldc_i4_0, "ldc.i4.0", Pop0, PushI, InlineNone, 1, 0x0016, Next),
        Ldc_i4_1 => new Opcode(Ldc_i4_1, "ldc.i4.1", Pop0, PushI, InlineNone, 1, 0x0017, Next),
        Ldc_i4_2 => new Opcode(Ldc_i4_2, "ldc.i4.2", Pop0, PushI, InlineNone, 1, 0x0018, Next),
        Ldc_i4_3 => new Opcode(Ldc_i4_3, "ldc.i4.3", Pop0, PushI, InlineNone, 1, 0x0019, Next),
        Ldc_i4_4 => new Opcode(Ldc_i4_4, "ldc.i4.4", Pop0, PushI, InlineNone, 1, 0x001A, Next),
        Ldc_i4_5 => new Opcode(Ldc_i4_5, "ldc.i4.5", Pop0, PushI, InlineNone, 1, 0x001B, Next),
        Ldc_i4_6 => new Opcode(Ldc_i4_6, "ldc.i4.6", Pop0, PushI, InlineNone, 1, 0x001C, Next),
        Ldc_i4_7 => new Opcode(Ldc_i4_7, "ldc.i4.7", Pop0, PushI, InlineNone, 1, 0x001D, Next),
        Ldc_i4_8 => new Opcode(Ldc_i4_8, "ldc.i4.8", Pop0, PushI, InlineNone, 1, 0x001E, Next),
        Ldc_i4_s => new Opcode(Ldc_i4_s, "ldc.i4.s", Pop0, PushI, ShortInlineI, 1, 0x001F, Next),
        Ldc_i4 => new Opcode(Ldc_i4, "ldc.i4", Pop0, PushI, InlineI, 1, 0x0020, Next),
        Ldc_i8 => new Opcode(Ldc_i8, "ldc.i8", Pop0, PushI8, InlineI8, 1, 0x0021, Next),
        Ldc_r4 => new Opcode(Ldc_r4, "ldc.r4", Pop0, PushR4, ShortInlineR, 1, 0x0022, Next),
        Ldc_r8 => new Opcode(Ldc_r8, "ldc.r8", Pop0, PushR8, InlineR, 1, 0x0023, Next),
        // unused: 0x0024,
        Dup => new Opcode(Dup, "dup", Pop1, Push1_Push1, InlineNone, 1, 0x0025, Next),
        Pop => new Opcode(Pop, "pop", Pop1, Push0, InlineNone, 1, 0x0026, Next),
        Jmp => new Opcode(Jmp, "jmp", Pop0, Push0, InlineMethod, 1, 0x0027, ControlFlowKind.Call),
        OpcodeKind.Call => new Opcode(OpcodeKind.Call, "call", VarPop, VarPush, InlineMethod, 1, 0x0028, ControlFlowKind.Call),
        Calli => new Opcode(Calli, "calli", VarPop, VarPush, InlineSig, 1, 0x0029, ControlFlowKind.Call),
        Ret => new Opcode(Ret, "ret", VarPop, Push0, InlineNone, 1, 0x002A, Return),
        Br_s => new Opcode(Br_s, "br.s", Pop0, Push0, ShortInlineBrTarget, 1, 0x002B, Branch),
        Brfalse_s => new Opcode(Brfalse_s, "brfalse.s", PopI, Push0, ShortInlineBrTarget, 1, 0x002C, Cond_branch),
        Brtrue_s => new Opcode(Brtrue_s, "brtrue.s", PopI, Push0, ShortInlineBrTarget, 1, 0x002D, Cond_branch),
        Beq_s => new Opcode(Beq_s, "beq.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x002E, Cond_branch),
        Bge_s => new Opcode(Bge_s, "bge.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x002F, Cond_branch),
        Bgt_s => new Opcode(Bgt_s, "bgt.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0030, Cond_branch),
        Ble_s => new Opcode(Ble_s, "ble.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0031, Cond_branch),
        Blt_s => new Opcode(Blt_s, "blt.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0032, Cond_branch),
        Bne_un_s => new Opcode(Bne_un_s, "bne.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0033, Cond_branch),
        Bge_un_s => new Opcode(Bge_un_s, "bge.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0034, Cond_branch),
        Bgt_un_s => new Opcode(Bgt_un_s, "bgt.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0035, Cond_branch),
        Ble_un_s => new Opcode(Ble_un_s, "ble.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0036, Cond_branch),
        Blt_un_s => new Opcode(Blt_un_s, "blt.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0037, Cond_branch),
        Br => new Opcode(Br, "br", Pop0, Push0, InlineBrTarget, 1, 0x0038, Branch),
        Brfalse => new Opcode(Brfalse, "brfalse", PopI, Push0, InlineBrTarget, 1, 0x0039, Cond_branch),
        Brtrue => new Opcode(Brtrue, "brtrue", PopI, Push0, InlineBrTarget, 1, 0x003A, Cond_branch),
        Beq => new Opcode(Beq, "beq", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003B, Cond_branch),
        Bge => new Opcode(Bge, "bge", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003C, Cond_branch),
        Bgt => new Opcode(Bgt, "bgt", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003D, Cond_branch),
        Ble => new Opcode(Ble, "ble", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003E, Cond_branch),
        Blt => new Opcode(Blt, "blt", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003F, Cond_branch),
        Bne_un => new Opcode(Bne_un, "bne.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0040, Cond_branch),
        Bge_un => new Opcode(Bge_un, "bge.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0041, Cond_branch),
        Bgt_un => new Opcode(Bgt_un, "bgt.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0042, Cond_branch),
        Ble_un => new Opcode(Ble_un, "ble.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0043, Cond_branch),
        Blt_un => new Opcode(Blt_un, "blt.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0044, Cond_branch),
        Switch => new Opcode(Switch, "switch", PopI, Push0, InlineSwitch, 1, 0x0045, Cond_branch),
        Ldind_i1 => new Opcode(Ldind_i1, "ldind.i1", PopI, PushI, InlineNone, 1, 0x0046, Next),
        Ldind_u1 => new Opcode(Ldind_u1, "ldind.u1", PopI, PushI, InlineNone, 1, 0x0047, Next),
        Ldind_i2 => new Opcode(Ldind_i2, "ldind.i2", PopI, PushI, InlineNone, 1, 0x0048, Next),
        Ldind_u2 => new Opcode(Ldind_u2, "ldind.u2", PopI, PushI, InlineNone, 1, 0x0049, Next),
        Ldind_i4 => new Opcode(Ldind_i4, "ldind.i4", PopI, PushI, InlineNone, 1, 0x004A, Next),
        Ldind_u4 => new Opcode(Ldind_u4, "ldind.u4", PopI, PushI, InlineNone, 1, 0x004B, Next),
        Ldind_i8 => new Opcode(Ldind_i8, "ldind.i8", PopI, PushI8, InlineNone, 1, 0x004C, Next),
        Ldind_i => new Opcode(Ldind_i, "ldind.i", PopI, PushI, InlineNone, 1, 0x004D, Next),
        Ldind_r4 => new Opcode(Ldind_r4, "ldind.r4", PopI, PushR4, InlineNone, 1, 0x004E, Next),
        Ldind_r8 => new Opcode(Ldind_r8, "ldind.r8", PopI, PushR8, InlineNone, 1, 0x004F, Next),
        Ldind_ref => new Opcode(Ldind_ref, "ldind.ref", PopI, PushRef, InlineNone, 1, 0x0050, Next),
        Stind_ref => new Opcode(Stind_ref, "stind.ref", PopI_PopI, Push0, InlineNone, 1, 0x0051, Next),
        Stind_i1 => new Opcode(Stind_i1, "stind.i1", PopI_PopI, Push0, InlineNone, 1, 0x0052, Next),
        Stind_i2 => new Opcode(Stind_i2, "stind.i2", PopI_PopI, Push0, InlineNone, 1, 0x0053, Next),
        Stind_i4 => new Opcode(Stind_i4, "stind.i4", PopI_PopI, Push0, InlineNone, 1, 0x0054, Next),
        Stind_i8 => new Opcode(Stind_i8, "stind.i8", PopI_PopI8, Push0, InlineNone, 1, 0x0055, Next),
        Stind_r4 => new Opcode(Stind_r4, "stind.r4", PopI_PopR4, Push0, InlineNone, 1, 0x0056, Next),
        Stind_r8 => new Opcode(Stind_r8, "stind.r8", PopI_PopR8, Push0, InlineNone, 1, 0x0057, Next),
        Add => new Opcode(Add, "add", Pop1_Pop1, Push1, InlineNone, 1, 0x0058, Next),
        Sub => new Opcode(Sub, "sub", Pop1_Pop1, Push1, InlineNone, 1, 0x0059, Next),
        Mul => new Opcode(Mul, "mul", Pop1_Pop1, Push1, InlineNone, 1, 0x005A, Next),
        Div => new Opcode(Div, "div", Pop1_Pop1, Push1, InlineNone, 1, 0x005B, Next),
        Div_un => new Opcode(Div_un, "div.un", Pop1_Pop1, Push1, InlineNone, 1, 0x005C, Next),
        Rem => new Opcode(Rem, "rem", Pop1_Pop1, Push1, InlineNone, 1, 0x005D, Next),
        Rem_un => new Opcode(Rem_un, "rem.un", Pop1_Pop1, Push1, InlineNone, 1, 0x005E, Next),
        And => new Opcode(And, "and", Pop1_Pop1, Push1, InlineNone, 1, 0x005F, Next),
        Or => new Opcode(Or, "or", Pop1_Pop1, Push1, InlineNone, 1, 0x0060, Next),
        Xor => new Opcode(Xor, "xor", Pop1_Pop1, Push1, InlineNone, 1, 0x0061, Next),
        Shl => new Opcode(Shl, "shl", Pop1_Pop1, Push1, InlineNone, 1, 0x0062, Next),
        Shr => new Opcode(Shr, "shr", Pop1_Pop1, Push1, InlineNone, 1, 0x0063, Next),
        Shr_un => new Opcode(Shr_un, "shr.un", Pop1_Pop1, Push1, InlineNone, 1, 0x0064, Next),
        Neg => new Opcode(Neg, "neg", Pop1, Push1, InlineNone, 1, 0x0065, Next),
        Not => new Opcode(Not, "not", Pop1, Push1, InlineNone, 1, 0x0066, Next),
        Conv_i1 => new Opcode(Conv_i1, "conv.i1", Pop1, PushI, InlineNone, 1, 0x0067, Next),
        Conv_i2 => new Opcode(Conv_i2, "conv.i2", Pop1, PushI, InlineNone, 1, 0x0068, Next),
        Conv_i4 => new Opcode(Conv_i4, "conv.i4", Pop1, PushI, InlineNone, 1, 0x0069, Next),
        Conv_i8 => new Opcode(Conv_i8, "conv.i8", Pop1, PushI8, InlineNone, 1, 0x006A, Next),
        Conv_r4 => new Opcode(Conv_r4, "conv.r4", Pop1, PushR4, InlineNone, 1, 0x006B, Next),
        Conv_r8 => new Opcode(Conv_r8, "conv.r8", Pop1, PushR8, InlineNone, 1, 0x006C, Next),
        Conv_u4 => new Opcode(Conv_u4, "conv.u4", Pop1, PushI, InlineNone, 1, 0x006D, Next),
        Conv_u8 => new Opcode(Conv_u8, "conv.u8", Pop1, PushI8, InlineNone, 1, 0x006E, Next),
        Callvirt => new Opcode(Callvirt, "callvirt", VarPop, VarPush, InlineMethod, 1, 0x006F, ControlFlowKind.Call),
        Cpobj => new Opcode(Cpobj, "cpobj", PopI_PopI, Push0, InlineType, 1, 0x0070, Next),
        Ldobj => new Opcode(Ldobj, "ldobj", PopI, Push1, InlineType, 1, 0x0071, Next),
        Ldstr => new Opcode(Ldstr, "ldstr", Pop0, PushRef, InlineString, 1, 0x0072, Next),
        Newobj => new Opcode(Newobj, "newobj", VarPop, PushRef, InlineMethod, 1, 0x0073, ControlFlowKind.Call),
        Castclass => new Opcode(Castclass, "castclass", PopRef, PushRef, InlineType, 1, 0x0074, Next),
        Isinst => new Opcode(Isinst, "isinst", PopRef, PushI, InlineType, 1, 0x0075, Next),
        Conv_r_un => new Opcode(Conv_r_un, "conv.r.un", Pop1, PushR8, InlineNone, 1, 0x0076, Next),
        // unused: 0x0077,
        // unused: 0x0078,
        Unbox => new Opcode(Unbox, "unbox", PopRef, PushI, InlineType, 1, 0x0079, Next),
        OpcodeKind.Throw => new Opcode(OpcodeKind.Throw, "throw", PopRef, Push0, InlineNone, 1, 0x007A, ControlFlowKind.Throw),
        Ldfld => new Opcode(Ldfld, "ldfld", PopRef, Push1, InlineField, 1, 0x007B, Next),
        Ldflda => new Opcode(Ldflda, "ldflda", PopRef, PushI, InlineField, 1, 0x007C, Next),
        Stfld => new Opcode(Stfld, "stfld", PopRef_Pop1, Push0, InlineField, 1, 0x007D, Next),
        Ldsfld => new Opcode(Ldsfld, "ldsfld", Pop0, Push1, InlineField, 1, 0x007E, Next),
        Ldsflda => new Opcode(Ldsflda, "ldsflda", Pop0, PushI, InlineField, 1, 0x007F, Next),
        Stsfld => new Opcode(Stsfld, "stsfld", Pop1, Push0, InlineField, 1, 0x0080, Next),
        Stobj => new Opcode(Stobj, "stobj", PopI_Pop1, Push0, InlineType, 1, 0x0081, Next),
        Conv_ovf_i1_un => new Opcode(Conv_ovf_i1_un, "conv.ovf.i1.un", Pop1, PushI, InlineNone, 1, 0x0082, Next),
        Conv_ovf_i2_un => new Opcode(Conv_ovf_i2_un, "conv.ovf.i2.un", Pop1, PushI, InlineNone, 1, 0x0083, Next),
        Conv_ovf_i4_un => new Opcode(Conv_ovf_i4_un, "conv.ovf.i4.un", Pop1, PushI, InlineNone, 1, 0x0084, Next),
        Conv_ovf_i8_un => new Opcode(Conv_ovf_i8_un, "conv.ovf.i8.un", Pop1, PushI8, InlineNone, 1, 0x0085, Next),
        Conv_ovf_u1_un => new Opcode(Conv_ovf_u1_un, "conv.ovf.u1.un", Pop1, PushI, InlineNone, 1, 0x0086, Next),
        Conv_ovf_u2_un => new Opcode(Conv_ovf_u2_un, "conv.ovf.u2.un", Pop1, PushI, InlineNone, 1, 0x0087, Next),
        Conv_ovf_u4_un => new Opcode(Conv_ovf_u4_un, "conv.ovf.u4.un", Pop1, PushI, InlineNone, 1, 0x0088, Next),
        Conv_ovf_u8_un => new Opcode(Conv_ovf_u8_un, "conv.ovf.u8.un", Pop1, PushI8, InlineNone, 1, 0x0089, Next),
        Conv_ovf_i_un => new Opcode(Conv_ovf_i_un, "conv.ovf.i.un", Pop1, PushI, InlineNone, 1, 0x008A, Next),
        Conv_ovf_u_un => new Opcode(Conv_ovf_u_un, "conv.ovf.u.un", Pop1, PushI, InlineNone, 1, 0x008B, Next),
        Box => new Opcode(Box, "box", Pop1, PushRef, InlineType, 1, 0x008C, Next),
        Newarr => new Opcode(Newarr, "newarr", PopI, PushRef, InlineType, 1, 0x008D, Next),
        Ldlen => new Opcode(Ldlen, "ldlen", PopRef, PushI, InlineNone, 1, 0x008E, Next),
        Ldelema => new Opcode(Ldelema, "ldelema", PopRef_PopI, PushI, InlineType, 1, 0x008F, Next),
        Ldelem_i1 => new Opcode(Ldelem_i1, "ldelem.i1", PopRef_PopI, PushI, InlineNone, 1, 0x0090, Next),
        Ldelem_u1 => new Opcode(Ldelem_u1, "ldelem.u1", PopRef_PopI, PushI, InlineNone, 1, 0x0091, Next),
        Ldelem_i2 => new Opcode(Ldelem_i2, "ldelem.i2", PopRef_PopI, PushI, InlineNone, 1, 0x0092, Next),
        Ldelem_u2 => new Opcode(Ldelem_u2, "ldelem.u2", PopRef_PopI, PushI, InlineNone, 1, 0x0093, Next),
        Ldelem_i4 => new Opcode(Ldelem_i4, "ldelem.i4", PopRef_PopI, PushI, InlineNone, 1, 0x0094, Next),
        Ldelem_u4 => new Opcode(Ldelem_u4, "ldelem.u4", PopRef_PopI, PushI, InlineNone, 1, 0x0095, Next),
        Ldelem_i8 => new Opcode(Ldelem_i8, "ldelem.i8", PopRef_PopI, PushI8, InlineNone, 1, 0x0096, Next),
        Ldelem_i => new Opcode(Ldelem_i, "ldelem.i", PopRef_PopI, PushI, InlineNone, 1, 0x0097, Next),
        Ldelem_r4 => new Opcode(Ldelem_r4, "ldelem.r4", PopRef_PopI, PushR4, InlineNone, 1, 0x0098, Next),
        Ldelem_r8 => new Opcode(Ldelem_r8, "ldelem.r8", PopRef_PopI, PushR8, InlineNone, 1, 0x0099, Next),
        Ldelem_ref => new Opcode(Ldelem_ref, "ldelem.ref", PopRef_PopI, PushRef, InlineNone, 1, 0x009A, Next),
        Stelem_i => new Opcode(Stelem_i, "stelem.i", PopRef_PopI_PopI, Push0, InlineNone, 1, 0x009B, Next),
        Stelem_i1 => new Opcode(Stelem_i1, "stelem.i1", PopRef_PopI_PopI, Push0, InlineNone, 1, 0x009C, Next),
        Stelem_i2 => new Opcode(Stelem_i2, "stelem.i2", PopRef_PopI_PopI, Push0, InlineNone, 1, 0x009D, Next),
        Stelem_i4 => new Opcode(Stelem_i4, "stelem.i4", PopRef_PopI_PopI, Push0, InlineNone, 1, 0x009E, Next),
        Stelem_i8 => new Opcode(Stelem_i8, "stelem.i8", PopRef_PopI_PopI8, Push0, InlineNone, 1, 0x009F, Next),
        Stelem_r4 => new Opcode(Stelem_r4, "stelem.r4", PopRef_PopI_PopR4, Push0, InlineNone, 1, 0x00A0, Next),
        Stelem_r8 => new Opcode(Stelem_r8, "stelem.r8", PopRef_PopI_PopR8, Push0, InlineNone, 1, 0x00A1, Next),
        Stelem_ref => new Opcode(Stelem_ref, "stelem.ref", PopRef_PopI_PopRef, Push0, InlineNone, 1, 0x00A2, Next),
        Ldelem => new Opcode(Ldelem, "ldelem", PopRef_PopI, Push1, InlineType, 1, 0x00A3, Next),
        Stelem => new Opcode(Stelem, "stelem", PopRef_PopI_Pop1, Push0, InlineType, 1, 0x00A4, Next),
        Unbox_any => new Opcode(Unbox_any, "unbox.any", PopRef, Push1, InlineType, 1, 0x00A5, Next),
        // unused: 0x00A6,
        // unused: 0x00A7,
        // unused: 0x00A8,
        // unused: 0x00A9,
        // unused: 0x00AA,
        // unused: 0x00AB,
        // unused: 0x00AC,
        // unused: 0x00AD,
        // unused: 0x00AE,
        // unused: 0x00AF,
        // unused: 0x00B0,
        // unused: 0x00B1,
        // unused: 0x00B2,
        Conv_ovf_i1 => new Opcode(Conv_ovf_i1, "conv.ovf.i1", Pop1, PushI, InlineNone, 1, 0x00B3, Next),
        Conv_ovf_u1 => new Opcode(Conv_ovf_u1, "conv.ovf.u1", Pop1, PushI, InlineNone, 1, 0x00B4, Next),
        Conv_ovf_i2 => new Opcode(Conv_ovf_i2, "conv.ovf.i2", Pop1, PushI, InlineNone, 1, 0x00B5, Next),
        Conv_ovf_u2 => new Opcode(Conv_ovf_u2, "conv.ovf.u2", Pop1, PushI, InlineNone, 1, 0x00B6, Next),
        Conv_ovf_i4 => new Opcode(Conv_ovf_i4, "conv.ovf.i4", Pop1, PushI, InlineNone, 1, 0x00B7, Next),
        Conv_ovf_u4 => new Opcode(Conv_ovf_u4, "conv.ovf.u4", Pop1, PushI, InlineNone, 1, 0x00B8, Next),
        Conv_ovf_i8 => new Opcode(Conv_ovf_i8, "conv.ovf.i8", Pop1, PushI8, InlineNone, 1, 0x00B9, Next),
        Conv_ovf_u8 => new Opcode(Conv_ovf_u8, "conv.ovf.u8", Pop1, PushI8, InlineNone, 1, 0x00BA, Next),
        // unused: 0x00BB,
        // unused: 0x00BC,
        // unused: 0x00BD,
        // unused: 0x00BE,
        // unused: 0x00BF,
        // unused: 0x00C0,
        // unused: 0x00C1,
        Refanyval => new Opcode(Refanyval, "refanyval", Pop1, PushI, InlineType, 1, 0x00C2, Next),
        Ckfinite => new Opcode(Ckfinite, "ckfinite", Pop1, PushR8, InlineNone, 1, 0x00C3, Next),
        // unused: 0x00C4,
        // unused: 0x00C5,
        Mkrefany => new Opcode(Mkrefany, "mkrefany", PopI, Push1, InlineType, 1, 0x00C6, Next),
        // unused: 0x00C7,
        // unused: 0x00C8,
        // unused: 0x00C9,
        // unused: 0x00CA,
        // unused: 0x00CB,
        // unused: 0x00CC,
        // unused: 0x00CD,
        // unused: 0x00CE,
        // unused: 0x00CF,
        Ldtoken => new Opcode(Ldtoken, "ldtoken", Pop0, PushI, InlineTok, 1, 0x00D0, Next),
        Conv_u2 => new Opcode(Conv_u2, "conv.u2", Pop1, PushI, InlineNone, 1, 0x00D1, Next),
        Conv_u1 => new Opcode(Conv_u1, "conv.u1", Pop1, PushI, InlineNone, 1, 0x00D2, Next),
        Conv_i => new Opcode(Conv_i, "conv.i", Pop1, PushI, InlineNone, 1, 0x00D3, Next),
        Conv_ovf_i => new Opcode(Conv_ovf_i, "conv.ovf.i", Pop1, PushI, InlineNone, 1, 0x00D4, Next),
        Conv_ovf_u => new Opcode(Conv_ovf_u, "conv.ovf.u", Pop1, PushI, InlineNone, 1, 0x00D5, Next),
        Add_ovf => new Opcode(Add_ovf, "add.ovf", Pop1_Pop1, Push1, InlineNone, 1, 0x00D6, Next),
        Add_ovf_un => new Opcode(Add_ovf_un, "add.ovf.un", Pop1_Pop1, Push1, InlineNone, 1, 0x00D7, Next),
        Mul_ovf => new Opcode(Mul_ovf, "mul.ovf", Pop1_Pop1, Push1, InlineNone, 1, 0x00D8, Next),
        Mul_ovf_un => new Opcode(Mul_ovf_un, "mul.ovf.un", Pop1_Pop1, Push1, InlineNone, 1, 0x00D9, Next),
        Sub_ovf => new Opcode(Sub_ovf, "sub.ovf", Pop1_Pop1, Push1, InlineNone, 1, 0x00DA, Next),
        Sub_ovf_un => new Opcode(Sub_ovf_un, "sub.ovf.un", Pop1_Pop1, Push1, InlineNone, 1, 0x00DB, Next),
        Endfinally => new Opcode(Endfinally, "endfinally", Pop0, Push0, InlineNone, 1, 0x00DC, Return),
        Leave => new Opcode(Leave, "leave", Pop0, Push0, InlineBrTarget, 1, 0x00DD, Branch),
        Leave_s => new Opcode(Leave_s, "leave.s", Pop0, Push0, ShortInlineBrTarget, 1, 0x00DE, Branch),
        Stind_i => new Opcode(Stind_i, "stind.i", PopI_PopI, Push0, InlineNone, 1, 0x00DF, Next),
        Conv_u => new Opcode(Conv_u, "conv.u", Pop1, PushI, InlineNone, 1, 0x00E0, Next),
        // unused: 0x00E1,
        // unused: 0x00E2,
        // unused: 0x00E3,
        // unused: 0x00E4,
        // unused: 0x00E5,
        // unused: 0x00E6,
        // unused: 0x00E7,
        // unused: 0x00E8,
        // unused: 0x00E9,
        // unused: 0x00EA,
        // unused: 0x00EB,
        // unused: 0x00EC,
        // unused: 0x00ED,
        // unused: 0x00EE,
        // unused: 0x00EF,
        // unused: 0x00F0,
        // unused: 0x00F1,
        // unused: 0x00F2,
        // unused: 0x00F3,
        // unused: 0x00F4,
        // unused: 0x00F5,
        // unused: 0x00F6,
        // unused: 0x00F7,
        // prefix: 0x00F8,
        // prefix: 0x00F9,
        // prefix: 0x00FA,
        // prefix: 0x00FB,
        // prefix: 0x00FC,
        // prefix: 0x00FD,
        // prefix: 0x00FE,
        Arglist => new Opcode(Arglist, "arglist", Pop0, PushI, InlineNone, 2, 0xFE00, Next),
        Ceq => new Opcode(Ceq, "ceq", Pop1_Pop1, PushI, InlineNone, 2, 0xFE01, Next),
        Cgt => new Opcode(Cgt, "cgt", Pop1_Pop1, PushI, InlineNone, 2, 0xFE02, Next),
        Cgt_un => new Opcode(Cgt_un, "cgt.un", Pop1_Pop1, PushI, InlineNone, 2, 0xFE03, Next),
        Clt => new Opcode(Clt, "clt", Pop1_Pop1, PushI, InlineNone, 2, 0xFE04, Next),
        Clt_un => new Opcode(Clt_un, "clt.un", Pop1_Pop1, PushI, InlineNone, 2, 0xFE05, Next),
        Ldftn => new Opcode(Ldftn, "ldftn", Pop0, PushI, InlineMethod, 2, 0xFE06, Next),
        Ldvirtftn => new Opcode(Ldvirtftn, "ldvirtftn", PopRef, PushI, InlineMethod, 2, 0xFE07, Next),
        // unused: 0xFE08,
        Ldarg => new Opcode(Ldarg, "ldarg", Pop0, Push1, InlineVar, 2, 0xFE09, Next),
        Ldarga => new Opcode(Ldarga, "ldarga", Pop0, PushI, InlineVar, 2, 0xFE0A, Next),
        Starg => new Opcode(Starg, "starg", Pop1, Push0, InlineVar, 2, 0xFE0B, Next),
        Ldloc => new Opcode(Ldloc, "ldloc", Pop0, Push1, InlineVar, 2, 0xFE0C, Next),
        Ldloca => new Opcode(Ldloca, "ldloca", Pop0, PushI, InlineVar, 2, 0xFE0D, Next),
        Stloc => new Opcode(Stloc, "stloc", Pop1, Push0, InlineVar, 2, 0xFE0E, Next),
        Localloc => new Opcode(Localloc, "localloc", PopI, PushI, InlineNone, 2, 0xFE0F, Next),
        // unused: 0xFE10,
        Endfilter => new Opcode(Endfilter, "endfilter", PopI, Push0, InlineNone, 2, 0xFE11, Return),
        Unaligned_ => new Opcode(Unaligned_, "unaligned.", Pop0, Push0, ShortInlineI, 2, 0xFE12, Meta),
        Volatile_ => new Opcode(Volatile_, "volatile.", Pop0, Push0, InlineNone, 2, 0xFE13, Meta),
        Tail_ => new Opcode(Tail_, "tail.", Pop0, Push0, InlineNone, 2, 0xFE14, Meta),
        Initobj => new Opcode(Initobj, "initobj", PopI, Push0, InlineType, 2, 0xFE15, Next),
        Constrained_ => new Opcode(Constrained_, "constrained.", Pop0, Push0, InlineType, 2, 0xFE16, Meta),
        Cpblk => new Opcode(Cpblk, "cpblk", PopI_PopI_PopI, Push0, InlineNone, 2, 0xFE17, Next),
        Initblk => new Opcode(Initblk, "initblk", PopI_PopI_PopI, Push0, InlineNone, 2, 0xFE18, Next),
        No_ => new Opcode(No_, "no.", Pop0, Push0, ShortInlineI, 2, 0xFE19, Meta),
        Rethrow => new Opcode(Rethrow, "rethrow", Pop0, Push0, InlineNone, 2, 0xFE1A, ControlFlowKind.Throw),
        // unused: 0xFE1B,
        Sizeof => new Opcode(Sizeof, "sizeof", Pop0, PushI, InlineType, 2, 0xFE1C, Next),
        Refanytype => new Opcode(Refanytype, "refanytype", Pop1, PushI, InlineNone, 2, 0xFE1D, Next),
        Readonly_ => new Opcode(Readonly_, "readonly.", Pop0, Push0, InlineNone, 2, 0xFE1E, Meta),
        // unused: 0xFE1F,
        // unused: 0xFE20,
        // unused: 0xFE21,
        // unused: 0xFE22,
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    public Operand CreateOperand(MetadataReader metadataReader, object? value)
    {
        var operand = new Operand(metadataReader, OperandKind, value: null) {
            Value = value
        };
        return operand;
    }

    public override bool Equals(object? obj) => (obj is Opcode other) && Equals(other);

    public bool Equals(Opcode other) => this == other;

    public override int GetHashCode() => Kind.GetHashCode();

    public override string ToString() => Name;
}