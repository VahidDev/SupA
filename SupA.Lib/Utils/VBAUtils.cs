using SupA.Lib.Core;
using System.Reflection;

namespace SupA.Lib.Utils
{
    public enum VbCallType
    {
        Method = 1,
        Get = 2,
        Let = 4,
        Set = 8
    }

    public static class VbaInterop
    {
        public static object CallByName(object obj, string procName, VbCallType callType, params object[] args)
        {
            Type type = obj.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            MemberInfo[] members = type.GetMember(procName, flags);

            foreach (MemberInfo member in members)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = (PropertyInfo)member;
                        if ((callType & VbCallType.Let) != 0)
                            property.SetValue(obj, args[0]);
                        else if ((callType & VbCallType.Get) != 0)
                            return property.GetValue(obj);
                        else if ((callType & VbCallType.Set) != 0)
                            property.SetValue(obj, args[0]);
                        break;

                    case MemberTypes.Method:
                        MethodInfo method = (MethodInfo)member;
                        if ((callType & VbCallType.Method) != 0)
                            return method.Invoke(obj, args);
                        break;

                    default:
                        throw new ArgumentException($"Unsupported member type: {member.MemberType}");
                }
            }

            throw new MissingMemberException($"Member '{procName}' not found in type '{type.FullName}'.");
        }

        public static string GetPropertyValue(cGroupNode groupNode, string propertyName)
        {
            // Get the type of the object (cGroupNode)
            Type type = typeof(cGroupNode);

            // Get the PropertyInfo for the specified property name
            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            // If propertyInfo is null, the property doesn't exist
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' does not exist in type '{type.Name}'.");
            }

            // Get the value of the property for the given object (groupNode)
            object value = propertyInfo.GetValue(groupNode);

            // Convert the value to string
            string propertyValue = Convert.ToString(value);

            return propertyValue;
        }
    }
}
