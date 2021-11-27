// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodImplementationInfoCollection : MetadataInfoCollection<MethodImplementationInfo, MethodImplementationHandle>
{
    public static readonly MethodImplementationInfoCollection Empty = new MethodImplementationInfoCollection();

    private MethodImplementationInfoCollection() : base()
    {
    }

    private MethodImplementationInfoCollection(ImmutableArray<MethodImplementationHandle> methodImplementationHandles, MetadataReader metadataReader)
        : base(methodImplementationHandles, metadataReader)
    {
    }

    public static MethodImplementationInfoCollection Create(MethodImplementationHandleCollection methodImplementationHandles, MetadataReader metadataReader)
    {
        if (methodImplementationHandles.Count == 0)
        {
            return Empty;
        }
        return new MethodImplementationInfoCollection(methodImplementationHandles.ToImmutableArray(), metadataReader);
    }

    protected override MethodImplementationInfo Resolve(MethodImplementationHandle methodImplementationHandle, MetadataReader metadataReader)
    {
        var methodImplementationInfo = CompilerInfo.Instance.Resolve(methodImplementationHandle, metadataReader);
        Debug.Assert(methodImplementationInfo is not null);
        return methodImplementationInfo;
    }
}
