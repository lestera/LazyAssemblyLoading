using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;

namespace LazyAssemblyLoading.Serialization
{
    [Serializable]
    public class SerializableLazyMemberInfo
    {
        public SerializableLazyMemberInfo()
        {
        }

        public SerializableLazyMemberInfo(LazyMemberInfo lazyMemberInfo)
        {
            MemberType = lazyMemberInfo.MemberType;

            var accessor = lazyMemberInfo.GetAccessors()[0];
            if (MemberType == MemberTypes.TypeInfo)
            {
                DeclaringTypeAssemblyQualifiedName = ((Type)accessor).AssemblyQualifiedName;
                MemberName = null;
            }
            else
            {
                // For properties, fields, etc. we keep the declaring type and the member name.
                DeclaringTypeAssemblyQualifiedName = accessor.DeclaringType.AssemblyQualifiedName;
                MemberName = accessor.Name;
            }

            if (MemberType == MemberTypes.Property)
            {
                // Remove the "get_" prefix from the property name.
                MemberName = MemberName.Substring(4);
            }
        }

        public SerializableLazyMemberInfo(ParameterInfo parameterInfo)
        {
            MemberType = parameterInfo.Member.MemberType;
            DeclaringTypeAssemblyQualifiedName = parameterInfo.Member.DeclaringType.AssemblyQualifiedName;
            MemberName = parameterInfo.Name;
        }

        public MemberTypes MemberType { get; set; }
        public string DeclaringTypeAssemblyQualifiedName { get; set; }
        public string MemberName { get; set; }
    }
}
