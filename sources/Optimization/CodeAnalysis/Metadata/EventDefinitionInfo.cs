// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class EventDefinitionInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly EventDefinition _eventDefinition;

    private CustomAttributeInfoCollection? _customAttributes;
    private string? _name;
    private MetadataInfo? _type;

    private EventAccessors _accessors;
    private bool _isAccessorsInitialized;

    private EventDefinitionInfo(EventDefinition eventDefinition, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _eventDefinition = eventDefinition;
    }

    public ref readonly EventAccessors Accessors
    {
        get
        {
            if (!_isAccessorsInitialized)
            {
                _accessors = EventDefinition.GetAccessors();
                _isAccessorsInitialized = true;
            }

            return ref _accessors;
        }
    }

    public EventAttributes Attributes => EventDefinition.Attributes;

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(EventDefinition.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public ref readonly EventDefinition EventDefinition => ref _eventDefinition;

    public MetadataReader MetadataReader => _metadataReader;

    public string Name
    {
        get
        {
            var name = _name;

            if (name is null)
            {
                name = MetadataReader.GetString(EventDefinition.Name);
                _name = name;
            }

            return name;
        }
    }

    public string QualifiedName => DisplayString;

    public MetadataInfo Type
    {
        get
        {
            var type = _type;

            if (type is null)
            {
                type = CompilerInfo.Instance.Resolve(EventDefinition.Type, MetadataReader);
                Debug.Assert(type is not null);
                _type = type;
            }

            return type;
        }
    }

    public static EventDefinitionInfo Create(EventDefinitionHandle eventDefinitionHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var eventDefinition = metadataReader.GetEventDefinition(eventDefinitionHandle);
        return new EventDefinitionInfo(eventDefinition, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(Type);
        _ = builder.Append('.');
        _ = builder.Append(Name);

        return builder.ToString();
    }
}
