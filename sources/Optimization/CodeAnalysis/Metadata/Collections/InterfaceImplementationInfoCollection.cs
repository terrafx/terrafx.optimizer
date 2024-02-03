// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class InterfaceImplementationInfoCollection : MetadataInfoCollection<InterfaceImplementationInfo, InterfaceImplementationHandle>
{
    public static readonly InterfaceImplementationInfoCollection Empty = new InterfaceImplementationInfoCollection();

    private InterfaceImplementationInfoCollection() : base()
    {
    }

    private InterfaceImplementationInfoCollection(ImmutableArray<InterfaceImplementationHandle> interfaceImplementationHandles, MetadataReader metadataReader)
        : base(interfaceImplementationHandles, metadataReader)
    {
    }

    public static InterfaceImplementationInfoCollection Create(InterfaceImplementationHandleCollection interfaceImplementationHandles, MetadataReader metadataReader)
    {
        return interfaceImplementationHandles.Count == 0
             ? Empty
             : new InterfaceImplementationInfoCollection([.. interfaceImplementationHandles], metadataReader);
    }

    protected override InterfaceImplementationInfo Resolve(InterfaceImplementationHandle metadataHandle, MetadataReader metadataReader)
    {
        var interfaceImplementationInfo = CompilerInfo.Instance.Resolve(metadataHandle, metadataReader);
        Debug.Assert(interfaceImplementationInfo is not null);
        return interfaceImplementationInfo;
    }
}
