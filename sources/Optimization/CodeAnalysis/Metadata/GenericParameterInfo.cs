// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class GenericParameterInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly GenericParameter _genericParameter;

    private GenericParameterConstraintInfoCollection? _constraints;
    private CustomAttributeInfoCollection? _customAttributes;
    private string? _name;
    private MetadataInfo? _parent;

    private GenericParameterInfo(GenericParameter genericParameter, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _genericParameter = genericParameter;
    }

    public GenericParameterAttributes Attributes => GenericParameter.Attributes;

    public GenericParameterConstraintInfoCollection Constraints
    {
        get
        {
            var constraints = _constraints;

            if (constraints is null)
            {
                constraints = GenericParameterConstraintInfoCollection.Create(GenericParameter.GetConstraints(), MetadataReader);
                _constraints = constraints;
            }

            return constraints;
        }
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(GenericParameter.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public ref readonly GenericParameter GenericParameter => ref _genericParameter;

    public int Index => GenericParameter.Index;

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(GenericParameter.Name);
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
                parent = CompilerInfo.Instance.Resolve(GenericParameter.Parent, MetadataReader);
                Debug.Assert(parent is not null);
                _parent = parent;
            }

            return parent;
        }
    }

    public static GenericParameterInfo Create(GenericParameterHandle genericParameterHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var genericParameter = metadataReader.GetGenericParameter(genericParameterHandle);
        return new GenericParameterInfo(genericParameter, metadataReader);
    }

    protected override string ResolveDisplayString() => Name;
}
