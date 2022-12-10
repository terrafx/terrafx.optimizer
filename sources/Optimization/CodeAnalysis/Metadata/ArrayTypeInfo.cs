// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ArrayTypeInfo : MetadataInfo
{
    private readonly MetadataInfo _elementType;
    private readonly ArrayShape _shape;
    private readonly bool _isSZArray;

    public ArrayTypeInfo(MetadataInfo elementType)
        : this(elementType, new ArrayShape(rank: 1, sizes: ImmutableArray<int>.Empty, lowerBounds: ImmutableArray<int>.Empty))
    {
    }

    public ArrayTypeInfo(MetadataInfo elementType, ArrayShape shape)
    {
        ThrowIfNull(elementType);

        _elementType = elementType;
        _shape = shape;
        _isSZArray = (shape.Rank == 1) && shape.LowerBounds.IsEmpty && shape.Sizes.IsEmpty;
    }

    public MetadataInfo ElementType => _elementType;

    public bool IsSZArray => _isSZArray;

    public ImmutableArray<int> LowerBounds => Shape.LowerBounds;

    public int Rank => Shape.Rank;

    public ref readonly ArrayShape Shape => ref _shape;

    public ImmutableArray<int> Sizes => Shape.Sizes;

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(ElementType);
        _ = builder.Append('[');

        if (!IsSZArray)
        {
            ref readonly var shape = ref Shape;
            var wasSimple = AppendRank(builder, shape, index: 0);

            for (var index = 1; index < shape.Rank; index++)
            {
                _ = builder.Append(',');

                if (!wasSimple)
                {
                    _ = builder.Append(' ');
                }
                wasSimple = AppendRank(builder, in shape, index);
            }
        }

        _ = builder.Append(']');
        return builder.ToString();

        static bool AppendRank(StringBuilder builder, in ArrayShape arrayShape, int index)
        {
            var wasSimple = true;

            var lowerBound = GetValue(arrayShape.LowerBounds, index);
            var size = GetValue(arrayShape.Sizes, index);

            if (lowerBound == 0)
            {
                if (size != 0)
                {
                    _ = builder.Append(size);
                    wasSimple = false;
                }
            }
            else
            {
                _ = builder.Append(lowerBound);
                wasSimple = false;

                _ = builder.Append("...");

                if (size != 0)
                {
                    _ = builder.Append(size - lowerBound);
                }
            }

            return wasSimple;
        }

        static int GetValue(ImmutableArray<int> array, int index)
        {
            return index < array.Length ? array[index] : 0;
        }
    }
}
