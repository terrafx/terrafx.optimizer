// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class MethodImplementationInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly MethodImplementation _methodImplementation;

    private CustomAttributeInfoCollection? _customAttributes;
    private MetadataInfo? _methodBody;
    private MetadataInfo? _methodDeclaration;
    private TypeDefinitionInfo? _type;

    private MethodImplementationInfo(MethodImplementation methodImplementation, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _methodImplementation = methodImplementation;
    }

    public CustomAttributeInfoCollection CustomAttributes
    {
        get
        {
            var customAttributes = _customAttributes;

            if (customAttributes is null)
            {
                customAttributes = CustomAttributeInfoCollection.Create(MethodImplementation.GetCustomAttributes(), MetadataReader);
                _customAttributes = customAttributes;
            }

            return customAttributes;
        }
    }

    public MetadataReader MetadataReader => _metadataReader;

    public MetadataInfo MethodBody
    {
        get
        {
            var methodBody = _methodBody;

            if (methodBody is null)
            {
                methodBody = CompilerInfo.Instance.Resolve(MethodImplementation.MethodBody, MetadataReader);
                Debug.Assert(methodBody is not null);
                _methodBody = methodBody;
            }

            return methodBody;
        }
    }

    public MetadataInfo MethodDeclaration
    {
        get
        {
            var methodDeclaration = _methodDeclaration;

            if (methodDeclaration is null)
            {
                methodDeclaration = CompilerInfo.Instance.Resolve(MethodImplementation.MethodDeclaration, MetadataReader);
                Debug.Assert(methodDeclaration is not null);
                _methodDeclaration = methodDeclaration;
            }

            return methodDeclaration;
        }
    }

    public ref readonly MethodImplementation MethodImplementation => ref _methodImplementation;

    public TypeDefinitionInfo Type
    {
        get
        {
            var type = _type;

            if (type is null)
            {
                type = CompilerInfo.Instance.Resolve(MethodImplementation.Type, MetadataReader);
                Debug.Assert(type is not null);
                _type = type;
            }

            return type;
        }
    }

    public static MethodImplementationInfo Create(MethodImplementationHandle methodImplementationHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var methodImplementation = metadataReader.GetMethodImplementation(methodImplementationHandle);
        return new MethodImplementationInfo(methodImplementation, metadataReader);
    }

    protected override string ResolveDisplayString() => ThrowNotImplementedException<string>();
}
