// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Reflection.Metadata;
using static TerraFX.Optimization.Utilities.ExceptionUtilities;

namespace TerraFX.Optimization.CodeAnalysis;

public partial class CompilerInfo
{
    public class CustomAttributeTypeProvider : ICustomAttributeTypeProvider<MetadataInfo>
    {
        public static readonly CustomAttributeTypeProvider Instance = new CustomAttributeTypeProvider();

        public MetadataInfo GetPrimitiveType(PrimitiveTypeCode typeCode) => SignatureTypeProvider.Instance.GetPrimitiveType(typeCode);

        public MetadataInfo GetSystemType() => ThrowNotImplementedException<MetadataInfo>();

        public MetadataInfo GetSZArrayType(MetadataInfo elementType) => SignatureTypeProvider.Instance.GetSZArrayType(elementType);

        public MetadataInfo GetTypeFromDefinition(MetadataReader metadataReader, TypeDefinitionHandle typeDefinitionHandle, byte rawTypeKind) => SignatureTypeProvider.Instance.GetTypeFromDefinition(metadataReader, typeDefinitionHandle, rawTypeKind);

        public MetadataInfo GetTypeFromReference(MetadataReader metadataReader, TypeReferenceHandle typeReferenceHandle, byte rawTypeKind) => SignatureTypeProvider.Instance.GetTypeFromReference(metadataReader, typeReferenceHandle, rawTypeKind);

        public MetadataInfo GetTypeFromSerializedName(string name) => ThrowNotImplementedException<MetadataInfo>();

        public PrimitiveTypeCode GetUnderlyingEnumType(MetadataInfo type) => ThrowNotImplementedException<PrimitiveTypeCode>();

        public bool IsSystemType(MetadataInfo type) => ThrowNotImplementedException<bool>();
    }
}
