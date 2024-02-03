// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Text;

namespace TerraFX.Optimization.CodeAnalysis;

public sealed class GenericMethodParameterInfo(MetadataInfo genericContext, int index) : MetadataInfo
{
    private readonly MetadataInfo _genericContext = genericContext;
    private readonly int _index = index;

    public MetadataInfo GenericContext => _genericContext;

    public int Index => _index;

    protected override string ResolveDisplayString()
    {
        var builder = new StringBuilder();

        _ = builder.Append("!!");
        _ = builder.Append(Index);

        return builder.ToString();
    }
}
