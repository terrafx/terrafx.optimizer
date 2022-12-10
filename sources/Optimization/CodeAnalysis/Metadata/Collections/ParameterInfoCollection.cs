// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ParameterInfoCollection : MetadataInfoCollection<ParameterInfo, ParameterHandle>
{
    public static readonly ParameterInfoCollection Empty = new ParameterInfoCollection();

    private ParameterInfoCollection() : base()
    {
    }

    private ParameterInfoCollection(ImmutableArray<ParameterHandle> parameterHandles, MetadataReader metadataReader)
        : base(parameterHandles, metadataReader)
    {
    }

    public static ParameterInfoCollection Create(ParameterHandleCollection parameterHandles, MetadataReader metadataReader)
    {
        if (parameterHandles.Count == 0)
        {
            return Empty;
        }
        return new ParameterInfoCollection(parameterHandles.ToImmutableArray(), metadataReader);
    }

    protected override ParameterInfo Resolve(ParameterHandle parameterHandle, MetadataReader metadataReader)
    {
        var parameterInfo = CompilerInfo.Instance.Resolve(parameterHandle, metadataReader);
        Debug.Assert(parameterInfo is not null);
        return parameterInfo;
    }
}
