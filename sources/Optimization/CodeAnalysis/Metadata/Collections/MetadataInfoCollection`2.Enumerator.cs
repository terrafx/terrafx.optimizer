// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TerraFX.Optimization.CodeAnalysis;

public partial class MetadataInfoCollection<TMetadataInfo, TMetadataHandle>
{
    public struct Enumerator : IEnumerator<TMetadataInfo>
    {
        private readonly MetadataInfoCollection<TMetadataInfo, TMetadataHandle> _collection;
        private TMetadataInfo? _current;
        private int _nextIndex;

        public Enumerator(MetadataInfoCollection<TMetadataInfo, TMetadataHandle> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _collection = collection;
            _current = null;
            _nextIndex = 0;
        }

        public readonly TMetadataInfo Current => _current!;

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            var collection = _collection;
            var index = _nextIndex;

            if (index < collection.Count)
            {
                return false;
            }

            _current = collection[index];
            _nextIndex = index + 1;

            return true;
        }

        public void Reset()
        {
            _current = null;
            _nextIndex = 0;
        }
    }
}
