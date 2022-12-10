// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public partial class CompilerInfo
{
    public sealed class SignatureTypeProvider : ISignatureTypeProvider<MetadataInfo, MetadataInfo>
    {
        public static readonly SignatureTypeProvider Instance = new SignatureTypeProvider();

        public MetadataInfo GetArrayType(MetadataInfo elementType, ArrayShape shape) => new ArrayTypeInfo(elementType, shape);

        public MetadataInfo GetByReferenceType(MetadataInfo elementType) => new ByReferenceTypeInfo(elementType);

        public MetadataInfo GetFunctionPointerType(MethodSignature<MetadataInfo> signature) => new FunctionPointerTypeInfo(signature);

        public MetadataInfo GetGenericInstantiation(MetadataInfo genericType, ImmutableArray<MetadataInfo> typeArguments) => new GenericInstantiationInfo(genericType, typeArguments);

        public MetadataInfo GetGenericMethodParameter(MetadataInfo genericContext, int index) => new GenericMethodParameterInfo(genericContext, index);

        public MetadataInfo GetGenericTypeParameter(MetadataInfo genericContext, int index) => new GenericTypeParameterInfo(genericContext, index);

        public MetadataInfo GetModifiedType(MetadataInfo modifier, MetadataInfo unmodifiedType, bool isRequired) => new ModifiedTypeInfo(modifier, unmodifiedType, isRequired);

        public MetadataInfo GetPinnedType(MetadataInfo elementType) => new PinnedTypeInfo(elementType);

        public MetadataInfo GetPointerType(MetadataInfo elementType) => new PointerTypeInfo(elementType);

        public MetadataInfo GetPrimitiveType(PrimitiveTypeCode typeCode) => new PrimitiveTypeInfo(typeCode);

        public MetadataInfo GetSZArrayType(MetadataInfo elementType) => new ArrayTypeInfo(elementType);

        public MetadataInfo GetTypeFromDefinition(MetadataReader metadataReader, TypeDefinitionHandle typeDefinitionHandle, byte rawTypeKind)
        {
            var typeDefinition = CompilerInfo.Instance.Resolve(typeDefinitionHandle, metadataReader);
            Debug.Assert(typeDefinition is not null);
            return typeDefinition;
        }

        public MetadataInfo GetTypeFromReference(MetadataReader metadataReader, TypeReferenceHandle typeReferenceHandle, byte rawTypeKind)
        {
            var typeReference = CompilerInfo.Instance.Resolve(typeReferenceHandle, metadataReader);
            Debug.Assert(typeReference is not null);
            return typeReference;
        }

        public MetadataInfo GetTypeFromSpecification(MetadataReader metadataReader, MetadataInfo genericContext, TypeSpecificationHandle typeSpecificationHandle, byte rawTypeKind)
        {
            var typeSpecification = CompilerInfo.Instance.Resolve(typeSpecificationHandle, metadataReader);
            Debug.Assert(typeSpecification is not null);
            return typeSpecification;
        }
    }
}
