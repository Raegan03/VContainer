using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    static class RuntimeTypeCache
    {
        static readonly Dictionary<Type, Type> OpenGenericTypes = new Dictionary<Type, Type>();
        static readonly Dictionary<Type, Type[]> GenericTypeParameters = new Dictionary<Type, Type[]>();
        static readonly Dictionary<Type, Type> ArrayTypes = new Dictionary<Type, Type>();
        static readonly Dictionary<Type, Type> EnumerableTypes = new Dictionary<Type, Type>();
        static readonly Dictionary<Type, Type> ReadOnlyListTypes = new Dictionary<Type, Type>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type OpenGenericTypeOf(Type closedGenericType)
            => OpenGenericTypes.GetOrAdd(closedGenericType, key => key.GetGenericTypeDefinition());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GenericTypeParametersOf(Type closedGenericType)
            => GenericTypeParameters.GetOrAdd(closedGenericType, key => key.GetGenericArguments());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ArrayTypeOf(Type elementType)
            => ArrayTypes.GetOrAdd(elementType, key => key.MakeArrayType());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type EnumerableTypeOf(Type elementType)
            => EnumerableTypes.GetOrAdd(elementType, key => typeof(IEnumerable<>).MakeGenericType(key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadOnlyListTypeOf(Type elementType)
            => ReadOnlyListTypes.GetOrAdd(elementType, key => typeof(IReadOnlyList<>).MakeGenericType(key));
    }
}