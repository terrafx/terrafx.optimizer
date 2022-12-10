// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Reflection;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ParameterInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly Parameter _parameter;

    private CustomAttributeInfoCollection? _customAttributes;
    private ConstantInfo? _defaultValue;
    private string? _name;

    private ParameterInfo(Parameter parameter, MetadataReader metadataReader)
    {
        ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _parameter = parameter;
    }

    public ParameterAttributes Attributes => Parameter.Attributes;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(Parameter.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public ConstantInfo? DefaultValue
    {
        get
        {
            var defaultValue = _defaultValue;

            if (defaultValue is null)
            {
                defaultValue = CompilerInfo.Instance.Resolve(Parameter.GetDefaultValue(), MetadataReader);
                _defaultValue = defaultValue;
            }

            return defaultValue;
        }
    }

    // TODO: Handle MarshallingDescriptor

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(Parameter.Name);
                _name = name;
            }

            return name;
        }
    }

    public ref readonly Parameter Parameter => ref _parameter;

    public int SequenceNumber => Parameter.SequenceNumber;

    public static ParameterInfo Create(ParameterHandle parameterHandle, MetadataReader metadataReader)
    {
        var parameter = metadataReader.GetParameter(parameterHandle);
        return new ParameterInfo(parameter, metadataReader);
    }

    protected override string ResolveDisplayString() => Name;
}
