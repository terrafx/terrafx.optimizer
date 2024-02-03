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
        return (eventDefinitionHandles.Count == 0)
             ? Empty
             : new EventDefinitionInfoCollection([.. eventDefinitionHandles], metadataReader);
    }

    protected override EventDefinitionInfo Resolve(EventDefinitionHandle metadataHandle, MetadataReader metadataReader)
    {
        var eventDefinitionInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(eventDefinitionInfo is not null);
        return eventDefinitionInfo;
    }
}
