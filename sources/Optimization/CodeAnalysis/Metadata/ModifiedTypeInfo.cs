// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class ModifiedTypeInfo : MetadataInfo
{
    private readonly MetadataInfo _modifier;
    private readonly MetadataInfo _unmodifiedType;
    private readonly bool _isRequired;

    public ModifiedTypeInfo(MetadataInfo modifier, MetadataInfo unmodifiedType, bool isRequired)
    {
        if (modifier is null)
        {
            throw new ArgumentNullException(nameof(modifier));
        }

        if (unmodifiedType is null)
        {
            throw new ArgumentNullException(nameof(unmodifiedType));
        }

        _modifier = modifier;
        _unmodifiedType = unmodifiedType;
        _isRequired = isRequired;
    }

    public bool IsRequired => _isRequired;

    public MetadataInfo Modifier => _modifier;

    public MetadataInfo UnmodifiedType => _unmodifiedType;

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append(UnmodifiedType);
        _ = builder.Append(IsRequired ? " modreq" : " modopt");
        _ = builder.Append('(');
        _ = builder.Append(Modifier);
        _ = builder.Append(')');

        return builder.ToString();
    }
}
