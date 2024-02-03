// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace TerraFX.Optimization.CodeAnalysis;

public partial class CompilerInfo
{
    internal sealed class SignatureTypeProvider : ISignatureTypeProvider<MetadataInfo, MetadataInfo>
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

        public MetadataInfo GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            var typeDefinition = CompilerInfo.Instance.Resolve(handle, reader);
            Debug.Assert(typeDefinition is not null);
            return typeDefinition;
        }

        public MetadataInfo GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            var typeReference = CompilerInfo.Instance.Resolve(handle, reader);
            Debug.Assert(typeReference is not null);
            return typeReference;
        }

        public MetadataInfo GetTypeFromSpecification(MetadataReader reader, MetadataInfo genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
        {
            var typeSpecification = CompilerInfo.Instance.Resolve(handle, reader);
            Debug.Assert(typeSpecification is not null);
            return typeSpecification;
        }
    }
}
