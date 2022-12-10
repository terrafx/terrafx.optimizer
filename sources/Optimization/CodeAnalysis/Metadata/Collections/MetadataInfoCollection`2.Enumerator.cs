// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections;
using System.Collections.Generic;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

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
            ThrowIfNull(collection);

            _collection = collection;
            _current = null;
            _nextIndex = 0;
        }

        public TMetadataInfo Current => _current!;

        object IEnumerator.Current => Current;

        public void Dispose()
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
