// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class InterfaceImplementationInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly InterfaceImplementation _interfaceImplementation;

    private CustomAttributeInfoCollection? _customAttributes;
    private MetadataInfo? _interface;

    private InterfaceImplementationInfo(InterfaceImplementation interfaceImplementation, MetadataReader metadataReader)
    {
        if (metadataReader is null)
        {
            throw new ArgumentNullException(nameof(metadataReader));
        }

        _metadataReader = metadataReader;
        _interfaceImplementation = interfaceImplementation;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(InterfaceImplementation.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public MetadataInfo Interface
    {
        get
        {
            var @interface = _interface;

            if (@interface is null)
            {
                @interface = CompilerInfo.Instance.Resolve(InterfaceImplementation.Interface, MetadataReader);
                Debug.Assert(@interface is not null);
                _interface = @interface;
            }

            return @interface;
        }
    }

    public ref readonly InterfaceImplementation InterfaceImplementation => ref _interfaceImplementation;

    public static InterfaceImplementationInfo Create(InterfaceImplementationHandle interfaceImplementationHandle, MetadataReader metadataReader)
    {
        var interfaceImplementation = metadataReader.GetInterfaceImplementation(interfaceImplementationHandle);
        return new InterfaceImplementationInfo(interfaceImplementation, metadataReader);
    }

    public MetadataReader MetadataReader => _metadataReader;

    protected override string ResolveDisplayString() => throw new NotImplementedException();
}
