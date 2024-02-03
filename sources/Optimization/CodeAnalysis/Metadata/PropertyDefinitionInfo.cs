// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class PropertyDefinitionInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly PropertyDefinition _propertyDefinition;

    private CustomAttributeInfoCollection? _customAttributes;
    private ConstantInfo? _defaultValue;
    private string? _name;

    private PropertyAccessors _accessors;
    private MethodSignature<MetadataInfo> _signature;
    private bool _isAccessorsInitialized;
    private bool _isSignatureInitialized;

    private PropertyDefinitionInfo(PropertyDefinition propertyDefinition, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _propertyDefinition = propertyDefinition;
    }

    public ref readonly PropertyAccessors Accessors
    {
        get
        {
            if (!_isAccessorsInitialized)
            {
                _accessors = PropertyDefinition.GetAccessors();
                _isAccessorsInitialized = true;
            }

            return ref _accessors;
        }
    }

    public PropertyAttributes Attributes => PropertyDefinition.Attributes;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(PropertyDefinition.GetCustomAttributes(), MetadataReader);
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
                defaultValue = CompilerInfo.Instance.Resolve(PropertyDefinition.GetDefaultValue(), MetadataReader);
                _defaultValue = defaultValue;
            }

            return defaultValue;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(PropertyDefinition.Name);
                _name = name;
            }

            return name;
        }
    }

    public ref readonly PropertyDefinition PropertyDefinition => ref _propertyDefinition;

    public ref readonly MethodSignature<MetadataInfo> Signature
    {
        get
        {
            if (!_isSignatureInitialized)
            {
                _signature = PropertyDefinition.DecodeSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                _isSignatureInitialized = true;
            }

            return ref _signature;
        }
    }

    public static PropertyDefinitionInfo Create(PropertyDefinitionHandle propertyDefinitionHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var propertyDefinition = metadataReader.GetPropertyDefinition(propertyDefinitionHandle);
        return new PropertyDefinitionInfo(propertyDefinition, metadataReader);
    }

    protected override string ResolveDisplayString() => ThrowNotImplementedException<string>();
}
