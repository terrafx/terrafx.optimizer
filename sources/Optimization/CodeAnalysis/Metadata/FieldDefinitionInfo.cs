// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class FieldDefinitionInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly FieldDefinition _fieldDefinition;

    private ConstantInfo? _defaultValue;
    private CustomAttributeInfoCollection? _customAttributes;
    private TypeDefinitionInfo? _declaringType;
    private string? _name;
    private MetadataInfo? _signature;

    private int _offset;
    private int _relativeVirtualAddress;
    private bool _isOffsetInitialized;
    private bool _isRelativeVirtualAddressInitialized;

    private FieldDefinitionInfo(FieldDefinition fieldDefinition, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _fieldDefinition = fieldDefinition;
    }

    public FieldAttributes Attributes => FieldDefinition.Attributes;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(FieldDefinition.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public TypeDefinitionInfo DeclaringType
    {
        get
        {
            var declaringType = _declaringType;

            if (declaringType is null)
            {
                declaringType = CompilerInfo.Instance.Resolve(FieldDefinition.GetDeclaringType(), MetadataReader);
                Debug.Assert(declaringType is not null);
                _declaringType = declaringType;
            }

            return declaringType;
        }
    }

    public ConstantInfo? DefaultValue
    {
        get
        {
            var defaultValue = _defaultValue;

            if (defaultValue is null)
            {
                defaultValue = CompilerInfo.Instance.Resolve(FieldDefinition.GetDefaultValue(), MetadataReader);
                _defaultValue = defaultValue;
            }

            return defaultValue;
        }
    }

    public ref readonly FieldDefinition FieldDefinition => ref _fieldDefinition;

    // TODO: Handle MarshallingDescriptor

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(FieldDefinition.Name);
                _name = name;
            }

            return name;
        }
    }

    public int Offset
    {
        get
        {
            if (!_isOffsetInitialized)
            {
                _offset = FieldDefinition.GetOffset();
                _isOffsetInitialized = true;
            }

            return _offset;
        }
    }

    public string QualifiedName => DisplayString;

    public int RelativeVirtualAddress
    {
        get
        {
            if (!_isRelativeVirtualAddressInitialized)
            {
                _relativeVirtualAddress = FieldDefinition.GetRelativeVirtualAddress();
                _isRelativeVirtualAddressInitialized = true;
            }

            return _relativeVirtualAddress;
        }
    }

    public MetadataInfo Signature
    {
        get
        {
            var signature = _signature;

            if (signature is null)
            {
                signature = FieldDefinition.DecodeSignature(CompilerInfo.SignatureTypeProvider.Instance, genericContext: this);
                Debug.Assert(signature is not null);
                _signature = signature;
            }

            return signature;
        }
    }

    public static FieldDefinitionInfo Create(FieldDefinitionHandle fieldDefinitionHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var fieldDefinition = metadataReader.GetFieldDefinition(fieldDefinitionHandle);
        return new FieldDefinitionInfo(fieldDefinition, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(Signature);
        _ = builder.Append(' ');
        _ = builder.Append(DeclaringType);
        _ = builder.Append("::");
        _ = builder.Append(Name);

        return builder.ToString();
    }
}
