// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class CustomAttributeInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly CustomAttribute _customAttribute;

    private MetadataInfo? _constructor;
    private MetadataInfo? _parent;

    private CustomAttributeValue<MetadataInfo> _value;
    private bool _isValueInitialized;

    private CustomAttributeInfo(CustomAttribute customAttribute, MetadataReader metadataReader)
    {
        ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _customAttribute = customAttribute;
    }

    public MetadataInfo Constructor
    {
        get
        {
            var constructor = _constructor;

            if (constructor is null)
            {
                var metadataReader = MetadataReader;
                var constructorHandle = CustomAttribute.Constructor;

                switch (constructorHandle.Kind)
                {
                    case HandleKind.MethodDefinition:
                    {
                        var methodDefinitionHandle = (MethodDefinitionHandle)constructorHandle;
                        constructor = CompilerInfo.Instance.Resolve(methodDefinitionHandle, metadataReader);
                        break;
                    }

                    case HandleKind.MemberReference:
                    {
                        var memberReferenceHandle = (MemberReferenceHandle)constructorHandle;
                        constructor = CompilerInfo.Instance.Resolve(memberReferenceHandle, metadataReader);
                        break;
                    }

                    default:
                    {
                        ThrowForInvalidKind(constructorHandle.Kind);
                        break;
                    }
                }

                Debug.Assert(constructor is not null);
                _constructor = constructor;
            }

            return constructor;
        }
    }

    public ref readonly CustomAttribute CustomAttribute => ref _customAttribute;

    public MetadataReader MetadataReader => _metadataReader;

    public MetadataInfo Parent
    {
        get
        {
            var parent = _parent;

            if (parent is null)
            {
                parent = CompilerInfo.Instance.Resolve(CustomAttribute.Parent, MetadataReader);
                Debug.Assert(parent is not null);
                _parent = parent;
            }

            return parent;
        }
    }

    public ref readonly CustomAttributeValue<MetadataInfo> Value
    {
        get
        {
            if (!_isValueInitialized)
            {
                _value = CustomAttribute.DecodeValue(CompilerInfo.CustomAttributeTypeProvider.Instance);
                _isValueInitialized = true;
            }

            return ref _value;
        }
    }

    public static CustomAttributeInfo Create(CustomAttributeHandle customAttributeHandle, MetadataReader metadataReader)
    {
        var customAttribute = metadataReader.GetCustomAttribute(customAttributeHandle);
        return new CustomAttributeInfo(customAttribute, metadataReader);
    }

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(".custom instance");
        _ = builder.Append(Constructor);
        _ = builder.Append(" = (");
        _ = builder.Append(Value);
        _ = builder.Append(')');

        return builder.ToString();
    }
}
