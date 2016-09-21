using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace LazyAssemblyLoading.Utilities
{
    internal static class MefUtils
    {
        public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Gets the importing constructor for a type. If it doesn't exist, attempts to get the type's default constructor.
        /// </summary>
        /// <param name="type">The type to get the importing constructor for.</param>
        /// <returns>A <see cref="ConstructorInfo"/> object of the type's importing constructor.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if one of the following is true:
        /// * The type has more than one constructor with the <see cref="ImportingConstructorAttribute"/> attribute.
        /// * The type has no constructors with the <see cref="ImportingConstructorAttribute"/> attribute and no default constructor.
        /// </exception>
        public static ConstructorInfo GetImportingConstructor(Type type)
        {
            var constructors = type.GetConstructors(DefaultBindingFlags)
                                   .Where(x => x.IsDefined(typeof(ImportingConstructorAttribute)))
                                   .ToList();
            if (constructors.Count > 1)
            {
                throw new InvalidOperationException(string.Format("Constructor for type {0} has more than one constructor with the ImportingConstructor attribute.", type));
            }

            // A single importing constructor was found.
            if (constructors.Count == 1)
            {
                return constructors[0];
            }

            // No importing constructors found - try to get default constructor.
            var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor != null)
            {
                return defaultConstructor;
            }

            throw new InvalidOperationException(string.Format("The type {0} does not define an importing constructor or a parameterless constructor.", type));
        }
    }
}
