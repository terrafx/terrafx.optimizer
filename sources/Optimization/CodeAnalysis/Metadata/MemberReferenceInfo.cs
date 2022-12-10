// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public abstract class MemberReferenceInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly MemberReference _memberReference;

    private CustomAttributeInfoCollection? _customAttributes;
    private string? _name;
    private MetadataInfo? _parent;

    protected MemberReferenceInfo(MemberReference memberReference, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _memberReference = memberReference;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(MemberReference.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public abstract MemberReferenceKind Kind { get; }

    public ref readonly MemberReference MemberReference => ref _memberReference;

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(MemberReference.Name);
                _name = name;
            }

            return name;
        }
    }

    public MetadataInfo Parent
    {
        get
        {
            var parent = _parent;

            if (parent is null)
            {
                parent = CompilerInfo.Instance.Resolve(MemberReference.Parent, MetadataReader);
                Debug.Assert(parent is not null);
                _parent = parent;
            }

            return parent;
        }
    }

    public string QualifiedName => DisplayString;

    public static MemberReferenceInfo Create(MemberReferenceHandle memberReferenceHandle, MetadataReader metadataReader)
    {
        var memberReference = metadataReader.GetMemberReference(memberReferenceHandle);

        return memberReference.GetKind() switch {
            MemberReferenceKind.Method => new MethodReferenceInfo(memberReference, metadataReader),
            MemberReferenceKind.Field => new FieldReferenceInfo(memberReference, metadataReader),
            _ => throw new NotSupportedException(),
        };
    }
}
