// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public abstract partial class MetadataInfoCollection<TMetadataInfo, TMetadataHandle> : IReadOnlyList<TMetadataInfo>
    where TMetadataInfo : MetadataInfo
{
    private readonly MetadataReader? _metadataReader;
    private readonly ImmutableArray<TMetadataHandle> _metadataHandles;
    private readonly TMetadataInfo[] _values;

    protected MetadataInfoCollection()
    {
        _metadataReader = null;
        _metadataHandles = ImmutableArray<TMetadataHandle>.Empty;
        _values = Array.Empty<TMetadataInfo>();
    }

    protected MetadataInfoCollection(ImmutableArray<TMetadataHandle> metadataHandles, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }
        Debug.Assert(!metadataHandles.IsDefaultOrEmpty);

        _metadataReader = metadataReader;
        _metadataHandles = metadataHandles;

        var length = metadataHandles.Length;
        _values = new TMetadataInfo[length];
    }

    public TMetadataInfo this[int index]
    {
        get
        {
            var value = _values[index];

            if (value is null)
            {
                var metadataHandle = _metadataHandles[index];
                var metadataReader = MetadataReader;

                Debug.Assert(metadataReader is not null);
                value = Resolve(metadataHandle, metadataReader);

                Debug.Assert(value is not null);
                _values[index] = value;
            }

            return value;
        }
    }

    public ImmutableArray<TMetadataHandle> MetadataHandles => _metadataHandles;

    public MetadataReader? MetadataReader => _metadataReader;

    public int Count => _values.Length;

    public IEnumerator<TMetadataInfo> GetEnumerator() => new Enumerator(this);

    protected abstract TMetadataInfo Resolve(TMetadataHandle metadataHandle, MetadataReader metadataReader);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
