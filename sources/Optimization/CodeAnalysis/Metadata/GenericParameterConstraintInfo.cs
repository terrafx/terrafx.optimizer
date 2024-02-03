// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class GenericParameterConstraintInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly GenericParameterConstraint _genericParameterConstraint;

    private CustomAttributeInfoCollection? _customAttributes;
    private GenericParameterInfo? _parameter;
    private MetadataInfo? _type;

    private GenericParameterConstraintInfo(GenericParameterConstraint genericParameterConstraint, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _genericParameterConstraint = genericParameterConstraint;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(GenericParameterConstraint.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public ref readonly GenericParameterConstraint GenericParameterConstraint => ref _genericParameterConstraint;

    public MetadataReader MetadataReader => _metadataReader;

    public GenericParameterInfo Parameter
    {
        get
        {
            var parameter = _parameter;

            if (parameter is null)
            {
                parameter = CompilerInfo.Instance.Resolve(GenericParameterConstraint.Parameter, MetadataReader);
                Debug.Assert(parameter is not null);
                _parameter = parameter;
            }

            return parameter;
        }
    }

    public MetadataInfo Type
    {
        get
        {
            var type = _type;

            if (type is null)
            {
                type = CompilerInfo.Instance.Resolve(GenericParameterConstraint.Type, MetadataReader);
                Debug.Assert(type is not null);
                _type = type;
            }

            return type;
        }
    }

    public static GenericParameterConstraintInfo Create(GenericParameterConstraintHandle genericParameterConstraintHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var genericParameterConstraint = metadataReader.GetGenericParameterConstraint(genericParameterConstraintHandle);
        return new GenericParameterConstraintInfo(genericParameterConstraint, metadataReader);
    }

    protected override string ResolveDisplayString() => ThrowNotImplementedException<string>();
}
