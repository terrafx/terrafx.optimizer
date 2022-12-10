// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

/// <summary>Defines a flowgraph where each node defines a basic block of instructions.</summary>
public sealed class Flowgraph
{
    private readonly SortedSet<BasicBlock> _blocks;
    private bool _isReadOnly;

    public Flowgraph()
    {
        _blocks = new SortedSet<BasicBlock>();
    }

    public IReadOnlySet<BasicBlock> Blocks => _blocks;

    public BasicBlock FirstBlock => Blocks.First();

    public bool IsReadOnly => _isReadOnly;

    public static Flowgraph Decode(MetadataReader metadataReader, MethodBodyBlock methodBody)
    {
        ThrowIfNull(metadataReader);
        ThrowIfNull(methodBody);

        // This is essentially a depth-first traversal of the blocks that dynamically
        // adds the parents, children, and instructions during the traversal. We use
        // a stack-based algorithm to track pending blocks, rather than using recursion,
        // so that we can process much more complex methods.

        // We have two maps that are used to track the internal state of the graph:
        //  * instructionMap: Contains a map of every instruction to the block it belongs to
        //  * firstInstructionMap: Contains a map of the first instruction for each block

        // For each non-branching instruction, we just add the next instruction to the block
        // currently being processed. But only after checking if it is the first instruction
        // of another block (any other instruction should not have been processed yet). When it
        // is the first instruction of another block, we add that block as a child of the current
        // block and move to process the next block in the stack.

        // When we reach a branching instruction we create a new block for each of the target
        // instruction(s) and push them onto the stack. Like with the non-branching instructions,
        // we check if it is the first instruction of existing block and similarly add it as a child
        // of the current block. We then move to process the other target instruction(s) for the
        // branch before finally moving to process the next block in the stack. Additionally, for
        // a block that is not already the first instruction of another block, we check if it has
        // been processed already. If so, we then move that instruction, and any subsequent
        // instructions from the existing block to a new block and make the new block a child
        // of the existing block. We finally move all children of the existing block to be children
        // of the existing block to ensure the directed links remain correct.

        var firstInstruction = Instruction.Decode(metadataReader, methodBody);
        var firstBlock = new BasicBlock(firstInstruction);

        var pendingBlocks = new Stack<BasicBlock>();
        pendingBlocks.Push(firstBlock);

        var firstInstructionMap = new SortedDictionary<Instruction, BasicBlock>() {
            [firstInstruction] = firstBlock,
        };

        var instructionMap = new SortedDictionary<Instruction, BasicBlock>() {
            [firstInstruction] = firstBlock,
        };

        while (pendingBlocks.Count != 0)
        {
            var currentBlock = pendingBlocks.Pop();
            var instruction = currentBlock.FirstInstruction;

            Debug.Assert(instruction is not null, "Expected the first instruction to be not null.");

            while (instruction is not null)
            {
                var nextInstruction = instruction.Next;
                var opcode = instruction.Opcode;
                var operandValue = instruction.Operand.Value;

                switch (opcode.ControlFlow)
                {
                    case ControlFlowKind.Branch:
                    {
                        // Unconditional branches specify the next instruction as their operand and
                        // will cause an unprocessed instruction to push a new block onto the stack.

                        Debug.Assert(operandValue is Instruction, "Expected an instruction for the branch target.");

                        var targetInstruction = (Instruction)operandValue!;
                        ProcessFirstInstructionForParent(currentBlock, targetInstruction, instructionMap, firstInstructionMap, pendingBlocks);

                        instruction = null;
                        break;
                    }

                    case ControlFlowKind.Break:
                    case ControlFlowKind.Call:
                    case ControlFlowKind.Next:
                    {
                        // Break, Call, and Next instructions just transfer control to the instruction
                        // at the next logical offset, they continue processing on the current block.

                        Debug.Assert(nextInstruction is not null, "Expected a next instruction.");
                        instruction = nextInstruction!;

                        if (firstInstructionMap.TryGetValue(instruction, out var childBlock))
                        {
                            // This is already the first instruction of a block, so we just need
                            // to add the existing block as a child of the current block and return
                            // false so that we stop processing this sequence.

                            currentBlock.AddChild(childBlock);
                            childBlock.AddParent(currentBlock);

                            instruction = null;
                        }
                        else
                        {
                            // This instruction hasn't been processed yet, so we need to process it by
                            // adding it to the current block and returning true so that we can continue
                            // processing the current sequence.

                            Debug.Assert(currentBlock.Contains(instruction) == false, "Expected the basic block to not contain the target instruction.");

                            currentBlock.LastInstruction = instruction;
                            instructionMap.Add(instruction, currentBlock);
                        }

                        nextInstruction = null;
                        break;
                    }

                    case ControlFlowKind.Cond_branch:
                    {
                        if (opcode.Kind == OpcodeKind.Switch)
                        {
                            // Switch statements are special in that they can have more than two
                            // child blocks. We need to process each of the normal blocks plus the
                            // instruction at the next logical offset.

                            Debug.Assert(operandValue is IReadOnlyList<Instruction>, "Expected an immutable array of instructions for the branch targets.");
                            var targetInstructions = (IReadOnlyList<Instruction>)operandValue!;

                            foreach (var targetInstruction in targetInstructions)
                            {
                                ProcessFirstInstructionForParent(currentBlock, targetInstruction, instructionMap, firstInstructionMap, pendingBlocks);
                            }
                        }
                        else
                        {
                            // Conditional branches should always have two children. One child will
                            // be the target instruction if the branch is taken and the other will
                            // be the instruction at the next logical offset.

                            Debug.Assert(operandValue is Instruction, "Expected an instruction for the branch target.");

                            var targetInstruction = (Instruction)operandValue!;
                            ProcessFirstInstructionForParent(currentBlock, targetInstruction, instructionMap, firstInstructionMap, pendingBlocks);
                        }
                        instruction = null;

                        // We need to add the next logical instruction as well, for when none of the
                        // branch conditions are met. However, we want to add this as a child block and
                        // then do no further processing.

                        Debug.Assert(nextInstruction is not null, "Expected a next instruction.");
                        ProcessFirstInstructionForParent(currentBlock, nextInstruction!, instructionMap, firstInstructionMap, pendingBlocks);

                        nextInstruction = null;
                        break;
                    }

                    case ControlFlowKind.Meta:
                    {
                        // Meta blocks generally provide additional information to the instruction at
                        // the next logical offset. We just want to treat them as `Next` for most cases.
                        goto case ControlFlowKind.Next;
                    }

                    case ControlFlowKind.Return:
                    case ControlFlowKind.Throw:
                    {
                        // Return and Throw instructions terminate the sequence for the current block and
                        // do not cause any new blocks to appear on the stack.

                        instruction = null;
                        break;
                    }
                }

                if (nextInstruction is not null)
                {
                    // Ensure the next instruction has already been processed or is part of the pending
                    // block list so that we can properly track dead/dangling code for later cleanup.
                    _ = ProcessFirstInstruction(nextInstruction, instructionMap, firstInstructionMap, pendingBlocks);
                }
            }
        }

        var flowgraph = new Flowgraph();

        foreach (var block in firstInstructionMap.Values)
        {
            block.Freeze();
            flowgraph.AddBlock(block);
        }

        flowgraph.Freeze();
        return flowgraph;

        static void ProcessFirstInstructionForParent(BasicBlock parentBlock, Instruction firstInstruction, SortedDictionary<Instruction, BasicBlock> instructionMap, SortedDictionary<Instruction, BasicBlock> firstInstructionMap, Stack<BasicBlock> pendingBlocks)
        {
            var childBlock = ProcessFirstInstruction(firstInstruction, instructionMap, firstInstructionMap, pendingBlocks);

            parentBlock.AddChild(childBlock);
            childBlock.AddParent(parentBlock);
        }

        static BasicBlock ProcessFirstInstruction(Instruction firstInstruction, SortedDictionary<Instruction, BasicBlock> instructionMap, SortedDictionary<Instruction, BasicBlock> firstInstructionMap, Stack<BasicBlock> pendingBlocks)
        {
            BasicBlock childBlock;

            if (firstInstructionMap.TryGetValue(firstInstruction, out childBlock!))
            {
                // This is already the first instruction of a block, so we don't need
                // to do anything and can just return.

                Debug.Assert(instructionMap.ContainsKey(firstInstruction), "Expected instruction to have been processed already.");
            }
            else if (instructionMap.TryGetValue(firstInstruction, out childBlock!))
            {
                // This instruction has already been processed, but it is not the first
                // instruction of the block it belongs to. We need to create a new block
                // where it is the first instruction and insert it as a child of the
                // existing block.

                var targetBlock = new BasicBlock(firstInstruction) {
                    LastInstruction = childBlock.LastInstruction
                };

                Debug.Assert(firstInstruction.Previous is not null, "");
                childBlock.LastInstruction = firstInstruction.Previous!;

                firstInstructionMap.Add(firstInstruction, targetBlock);

                targetBlock.AddChildren(childBlock.Children);
                childBlock.ClearChildren();

                childBlock.AddChild(targetBlock);
                targetBlock.AddParent(childBlock);
            }
            else
            {
                // This instruction hasn't been processed yet, so we need to process it by
                // creating a new block and adding it as the first instruction and then
                // pushing it into the list of pending blocks.

                childBlock = new BasicBlock(firstInstruction);

                instructionMap.Add(firstInstruction, childBlock);
                firstInstructionMap.Add(firstInstruction, childBlock);

                pendingBlocks.Push(childBlock);
            }

            return childBlock;
        }
    }

    public void AddBlock(BasicBlock block)
    {
        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(Flowgraph));
        }
        _ = _blocks.Add(block);
    }

    public void Freeze()
    {
        _isReadOnly = true;
    }

    public bool IsReachable(BasicBlock block) => FirstBlock.CanReach(block);

    public void TraverseBreadthFirst(Action<BasicBlock> action) => FirstBlock.TraverseBreadthFirst(action);

    public IEnumerable<T> TraverseBreadthFirst<T>(Func<BasicBlock, T> func) => FirstBlock.TraverseBreadthFirst(func);

    public void TraverseDepthFirst(Action<BasicBlock> action) => FirstBlock.TraverseDepthFirst(action);

    public IEnumerable<T> TraverseDepthFirst<T>(Func<BasicBlock, T> func) => FirstBlock.TraverseDepthFirst(func);
}
