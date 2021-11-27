// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class EventDefinitionInfoCollection : MetadataInfoCollection<EventDefinitionInfo, EventDefinitionHandle>
{
    public static readonly EventDefinitionInfoCollection Empty = new EventDefinitionInfoCollection();

    private EventDefinitionInfoCollection() : base()
    {
    }

    private EventDefinitionInfoCollection(ImmutableArray<EventDefinitionHandle> eventDefinitionHandles, MetadataReader metadataReader)
        : base(eventDefinitionHandles, metadataReader)
    {
    }

    public static EventDefinitionInfoCollection Create(EventDefinitionHandleCollection eventDefinitionHandles, MetadataReader metadataReader)
    {
        if (eventDefinitionHandles.Count == 0)
        {
            return Empty;
        }
        return new EventDefinitionInfoCollection(eventDefinitionHandles.ToImmutableArray(), metadataReader);
    }

    protected override EventDefinitionInfo Resolve(EventDefinitionHandle eventDefinitionHandle, MetadataReader metadataReader)
    {
        var eventDefinitionInfo = CompilerInfo.Instance.Resolve(eventDefinitionHandle, metadataReader);
        Debug.Assert(eventDefinitionInfo is not null);
        return eventDefinitionInfo;
    }
}
