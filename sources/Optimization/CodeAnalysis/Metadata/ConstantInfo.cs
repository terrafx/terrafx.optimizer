// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ConstantInfo : MetadataInfo
{
    private readonly MetadataReader _metadataReader;
    private readonly Constant _constant;

    private MetadataInfo? _parent;

    private ConstantInfo(Constant constant, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);

        _metadataReader = metadataReader;
        _constant = constant;
    }

    public ref readonly Constant Constant => ref _constant;

    public MetadataReader MetadataReader => _metadataReader;

    public MetadataInfo Parent
    {
        get
        {
            var parent = _parent;

            if (parent is null)
            {
                parent = CompilerInfo.Instance.Resolve(Constant.Parent, MetadataReader);
                Debug.Assert(parent is not null);
                _parent = parent;
            }

            return parent;
        }
    }

    public ConstantTypeCode TypeCode => Constant.TypeCode;

    // TODO: Handle Value

    public static ConstantInfo Create(ConstantHandle constantHandle, MetadataReader metadataReader)
    {
        ArgumentNullException.ThrowIfNull(metadataReader);
        var constant = metadataReader.GetConstant(constantHandle);
        return new ConstantInfo(constant, metadataReader);
    }

    protected override string ResolveDisplayString() => ThrowNotImplementedException<string>();
}
