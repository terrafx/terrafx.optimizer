// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

/// <summary>Defines a basic block of instructions, for use in a control flow graph.</summary>
public sealed partial class BasicBlock : IComparable, IComparable<BasicBlock>, IEnumerable<Instruction>, IEquatable<BasicBlock>
{
    private readonly Instruction _firstInstruction;

    private readonly SortedSet<BasicBlock> _children;
    private readonly SortedSet<BasicBlock> _parents;

    private Instruction _lastInstruction;
    private bool _isReadOnly;

    public BasicBlock(Instruction firstInstruction)
    {
        ArgumentNullException.ThrowIfNull(firstInstruction);

        _children = [];
        _parents = [];

        _firstInstruction = firstInstruction;
        _lastInstruction = firstInstruction;
    }

    public IReadOnlySet<BasicBlock> Children => _children;

    public Instruction FirstInstruction => _firstInstruction;

    public Instruction LastInstruction
    {
        get
        {
            return _lastInstruction;
        }

        set
        {
            if (IsReadOnly)
            {
                ThrowForReadOnly(nameof(BasicBlock));
            }
            _lastInstruction = value;
        }
    }

    public int InDegree => Parents.Count;

    public bool IsReadOnly => _isReadOnly;

    public int OutDegree => Children.Count;

    public IReadOnlySet<BasicBlock> Parents => _parents;

    public static bool operator ==(BasicBlock? left, BasicBlock? right) => ReferenceEquals(left, right);

    public static bool operator !=(BasicBlock? left, BasicBlock? right) => !(left == right);

    public static bool operator <(BasicBlock? left, BasicBlock? right)
        => (left is null) ? (right is not null) : (left.CompareTo(right) < 0);

    public static bool operator <=(BasicBlock? left, BasicBlock? right)
        => (left is null) || (left.CompareTo(right) <= 0);

    public static bool operator >(BasicBlock? left, BasicBlock? right)
        => (left is not null) && (left.CompareTo(right) > 0);

    public static bool operator >=(BasicBlock? left, BasicBlock? right)
        => (left is null) ? (right is null) : (left.CompareTo(right) >= 0);

    public void AddChild(BasicBlock childBlock)
    {
        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(BasicBlock));
        }
        _ = _children.Add(childBlock);
    }

    public void AddChildren(IEnumerable<BasicBlock> children)
    {
        ArgumentNullException.ThrowIfNull(children);

        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(BasicBlock));
        }

        foreach (var child in children)
        {
            _ = _children.Add(child);
        }
    }

    public void AddParent(BasicBlock parentBlock)
    {
        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(BasicBlock));
        }
        _ = _parents.Add(parentBlock);
    }

    // Determining if this block can reach the target block is a simple traversal
    // where we return true if the traversed block is ever the target block.
    public bool CanReach(BasicBlock block) => TraverseDepthFirst((traversedBlock) => traversedBlock == block).Any();

    public void ClearChildren()
    {
        if (IsReadOnly)
        {
            ThrowForReadOnly(nameof(BasicBlock));
        }
        _children.Clear();
    }

    public int CompareTo(BasicBlock? other)
    {
        if (this == other)
        {
            return 0;
        }
        else if (other is not null)
        {
            return FirstInstruction.CompareTo(other.FirstInstruction);
        }
        return 1;
    }

    public int CompareTo(object? obj)
    {
        if (obj is BasicBlock other)
        {
            return CompareTo(other);
        }
        else if (obj is not null)
        {
            ThrowForInvalidType(obj.GetType(), typeof(BasicBlock));
        }
        return 1;
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

    public bool Equals(BasicBlock? other) => this == other;

    public override bool Equals(object? obj) => (obj is BasicBlock other) && Equals(other);

    public void Freeze()
    {
        _isReadOnly = true;
    }

    public Enumerator GetEnumerator() => new Enumerator(FirstInstruction, LastInstruction);

    public override int GetHashCode() => base.GetHashCode();

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
        _ = builder.Append(firstInstruction.Offset.ToString("X4", CultureInfo.InvariantCulture));

        var lastInstruction = LastInstruction;

        if (lastInstruction != firstInstruction)
        {
            _ = builder.Append(',');
            _ = builder.Append(' ');

            _ = builder.Append("IL_");
            _ = builder.Append(lastInstruction.Offset.ToString("X4", CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    public void TraverseBreadthFirst(Action<BasicBlock> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        // We do a breadth-first traversal of the blocks using a queue-based algorithm
        // to track pending blocks, rather than using recursion, so that we can
        // process much more complex graphs.

        var visitedBlocks = new SortedSet<BasicBlock>();
        var pendingBlocks = new Queue<BasicBlock>();

        _ = visitedBlocks.Add(this);
        pendingBlocks.Enqueue(this);

        do
        {
            var block = pendingBlocks.Dequeue();
            action(block);

            foreach (var child in block.Children)
            {
                if (visitedBlocks.Add(child))
                {
                    pendingBlocks.Enqueue(child);
                }
            }
        }
        while (pendingBlocks.Count != 0);
    }

    public IEnumerable<T> TraverseBreadthFirst<T>(Func<BasicBlock, T> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        // We do a breadth-first traversal of the blocks using a queue-based algorithm
        // to track pending blocks, rather than using recursion, so that we can
        // process much more complex graphs.

        var visitedBlocks = new SortedSet<BasicBlock>();
        var pendingBlocks = new Queue<BasicBlock>();

        _ = visitedBlocks.Add(this);
        pendingBlocks.Enqueue(this);

        do
        {
            var block = pendingBlocks.Dequeue();
            yield return func(block);

            foreach (var child in block.Children)
            {
                if (visitedBlocks.Add(child))
                {
                    pendingBlocks.Enqueue(child);
                }
            }
        }
        while (pendingBlocks.Count != 0);
    }

    public void TraverseDepthFirst(Action<BasicBlock> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        // We do a depth-first traversal of the blocks using a stack-based algorithm
        // to track pending blocks, rather than using recursion, so that we can
        // process much more complex graphs.

        var visitedBlocks = new SortedSet<BasicBlock>();
        var pendingBlocks = new Stack<BasicBlock>();

        _ = visitedBlocks.Add(this);
        pendingBlocks.Push(this);

        do
        {
            var block = pendingBlocks.Pop();
            action(block);

            foreach (var child in block.Children)
            {
                if (visitedBlocks.Add(child))
                {
                    pendingBlocks.Push(child);
                }
            }
        }
        while (pendingBlocks.Count != 0);
    }

    public IEnumerable<T> TraverseDepthFirst<T>(Func<BasicBlock, T> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        // We do a depth-first traversal of the blocks using a stack-based algorithm
        // to track pending blocks, rather than using recursion, so that we can
        // process much more complex graphs.

        var visitedBlocks = new SortedSet<BasicBlock>();
        var pendingBlocks = new Stack<BasicBlock>();

        _ = visitedBlocks.Add(this);
        pendingBlocks.Push(this);

        do
        {
            var block = pendingBlocks.Pop();
            yield return func(block);

            foreach (var child in block.Children)
            {
                if (visitedBlocks.Add(child))
                {
                    pendingBlocks.Push(child);
                }
            }
        }
        while (pendingBlocks.Count != 0);
    }

    IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
