using System;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Extensions {
    internal static class ConfPropertyInfo {
        public static ConfAttribute GetConfAttribute(this PropertyInfo propertyInfo) {
            if (null == propertyInfo) throw new ArgumentNullException(nameof(propertyInfo));
            return propertyInfo.GetCustomAttributes(typeof(ConfAttribute), inherit: true)?.FirstOrDefault() as ConfAttribute;
        }
    }
}
