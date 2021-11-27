// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class DeclarativeSecurityAttributeInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly DeclarativeSecurityAttribute _declarativeSecurityAttribute;

    private MetadataInfo? _parent;

    private DeclarativeSecurityAttributeInfo(DeclarativeSecurityAttribute declarativeSecurityAttribute, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _declarativeSecurityAttribute = declarativeSecurityAttribute;
    }

    public DeclarativeSecurityAction Action => DeclarativeSecurityAttribute.Action;

    public ref readonly DeclarativeSecurityAttribute DeclarativeSecurityAttribute => ref _declarativeSecurityAttribute;

    public MetadataReader MetadataReader => _metadataReader;

    public MetadataInfo Parent
    {
        get
        {
            var parent = _parent;

            if (parent is null)
            {
                parent = CompilerInfo.Instance.Resolve(DeclarativeSecurityAttribute.Parent, MetadataReader);
                Debug.Assert(parent is not null);
                _parent = parent;
            }

            return parent;
        }
    }

    // TODO: Handle PermissionSet

    public static DeclarativeSecurityAttributeInfo Create(DeclarativeSecurityAttributeHandle declarativeSecurityAttributeHandle, MetadataReader metadataReader)
    {
        var declarativeSecurityAttribute = metadataReader.GetDeclarativeSecurityAttribute(declarativeSecurityAttributeHandle);
        return new DeclarativeSecurityAttributeInfo(declarativeSecurityAttribute, metadataReader);
    }

    protected override string ResolveDisplayString() => throw new NotImplementedException();
}
