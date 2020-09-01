// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis
{
    /// <summary>Defines a basic block of instructions, for use in a control flow graph.</summary>
    public sealed partial class BasicBlock
    {
        internal HashSet<BasicBlock> _children;
        internal Instruction _lastInstruction;
        internal HashSet<BasicBlock> _parents;

        internal BasicBlock(Instruction firstInstruction)
        {
            if (firstInstruction is null)
            {
                throw new ArgumentNullException(nameof(firstInstruction));
            }

            _children = new HashSet<BasicBlock>();
            _parents = new HashSet<BasicBlock>();

            FirstInstruction = firstInstruction;
            _lastInstruction = firstInstruction;
        }

        public IReadOnlyCollection<BasicBlock> Children => _children;

        public Instruction FirstInstruction { get; }

        public Instruction LastInstruction => _lastInstruction;

        public int InDegree => Parents.Count;

        public int OutDegree => Children.Count;

        public IReadOnlyCollection<BasicBlock> Parents => _parents;

        public bool CanReach(BasicBlock block)
        {
            // Determining if this block can reach the target block is a simple traversal
            // where we return true if the traversed block is ever the target block.

            return TraverseDepthFirst((traversedBlock) => (traversedBlock == block)).Any();
        }

        public bool Contains(Instruction instruction)
        {
            for (var currentInstruction = FirstInstruction; currentInstruction != LastInstruction; currentInstruction = currentInstruction.Next!)
            {
                if (instruction == currentInstruction)
                {
                    return true;
                }
            }

            return instruction == LastInstruction;
        }

        public int GetInstructionCount()
        {
            var count = 0;

            for (var currentInstruction = FirstInstruction; currentInstruction != LastInstruction; currentInstruction = currentInstruction.Next!)
            {
                count++;
            }

            // Increment one remaining time for "LastInstruction"
            return ++count;
        }

        public bool IsAdjacent(BasicBlock block)
        {
            // Determining if we are adjacent to a block is simply checking if the
            // children or parents of this block contains the target block.

            return Children.Contains(block) || Parents.Contains(block);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            var firstInstruction = FirstInstruction;

            _ = builder.Append("IL_");
            var offset = firstInstruction.GetOffset();
            _ = builder.Append(offset.ToString("X4"));

            var lastInstruction = LastInstruction;

            if (lastInstruction != firstInstruction)
            {
                _ = builder.Append(',');
                _ = builder.Append(' ');

                _ = builder.Append("IL_");
                offset = lastInstruction.GetOffset();
                _ = builder.Append(offset.ToString("X4"));
            }

            return builder.ToString();
        }

        public void TraverseBreadthFirst(Action<BasicBlock> action)
        {
            // We do a breadth-first traversal of the blocks using a queue-based algorithm
            // to track pending blocks, rather than using recursion, so that we can
            // process much more complex graphs.

            var visitedBlocks = new HashSet<BasicBlock>();
            var pendingBlocks = new Queue<BasicBlock>();

            _ = visitedBlocks.Add(this);
            pendingBlocks.Enqueue(this);

            do
            {
                var block = pendingBlocks.Dequeue();
                action(block);

                foreach (var child in block.Children)
                {
                    if (visitedBlocks.Contains(child) == false)
                    {
                        _ = visitedBlocks.Add(child);
                        pendingBlocks.Enqueue(child);
                    }
                }
            }
            while (pendingBlocks.Count != 0);
        }

        public IEnumerable<T> TraverseBreadthFirst<T>(Func<BasicBlock, T> func)
        {
            // We do a breadth-first traversal of the blocks using a queue-based algorithm
            // to track pending blocks, rather than using recursion, so that we can
            // process much more complex graphs.

            var visitedBlocks = new HashSet<BasicBlock>();
            var pendingBlocks = new Queue<BasicBlock>();

            _ = visitedBlocks.Add(this);
            pendingBlocks.Enqueue(this);

            do
            {
                var block = pendingBlocks.Dequeue();
                yield return func(block);

                foreach (var child in block.Children)
                {
                    if (visitedBlocks.Contains(child) == false)
                    {
                        _ = visitedBlocks.Add(child);
                        pendingBlocks.Enqueue(child);
                    }
                }
            }
            while (pendingBlocks.Count != 0);
        }

        public void TraverseDepthFirst(Action<BasicBlock> action)
        {
            // We do a depth-first traversal of the blocks using a stack-based algorithm
            // to track pending blocks, rather than using recursion, so that we can
            // process much more complex graphs.

            var visitedBlocks = new HashSet<BasicBlock>();
            var pendingBlocks = new Stack<BasicBlock>();

            _ = visitedBlocks.Add(this);
            pendingBlocks.Push(this);

            do
            {
                var block = pendingBlocks.Pop();
                action(block);

                foreach (var child in block.Children)
                {
                    if (visitedBlocks.Contains(child) == false)
                    {
                        _ = visitedBlocks.Add(child);
                        pendingBlocks.Push(child);
                    }
                }
            }
            while (pendingBlocks.Count != 0);
        }

        public IEnumerable<T> TraverseDepthFirst<T>(Func<BasicBlock, T> func)
        {
            // We do a depth-first traversal of the blocks using a stack-based algorithm
            // to track pending blocks, rather than using recursion, so that we can
            // process much more complex graphs.

            var visitedBlocks = new HashSet<BasicBlock>();
            var pendingBlocks = new Stack<BasicBlock>();

            _ = visitedBlocks.Add(this);
            pendingBlocks.Push(this);

            do
            {
                var block = pendingBlocks.Pop();
                yield return func(block);

                foreach (var child in block.Children)
                {
                    if (visitedBlocks.Contains(child) == false)
                    {
                        _ = visitedBlocks.Add(child);
                        pendingBlocks.Push(child);
                    }
                }
            }
            while (pendingBlocks.Count != 0);
        }
    }
}
