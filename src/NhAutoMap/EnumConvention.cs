using System;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace NhAutoMap {
	public static class EnumConvention {
		public static bool IsEnum(this PropertyInfo property) {
			return property.PropertyType.IsEnum || (property.PropertyType.IsGenericType &&
			                                        property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>) &&
			                                        property.PropertyType.GetGenericArguments()[0].IsEnum);
		}

		public static void MapAllEnumsToStrings(this ModelMapper mapper) {
			mapper.BeforeMapProperty += MapProperty;
		}

		private static void CallGenericTypeMethod(IPropertyMapper map, PropertyInfo property) {
			var enumStringOfPropertyType = typeof (EnumStringType<>).MakeGenericType(property.PropertyType);
			var method = map.GetType().GetMethods().First(x => x.Name == "Type" && !x.GetParameters().Any());
			var genericMethod = method.MakeGenericMethod(new[] {enumStringOfPropertyType});
			genericMethod.Invoke(map, null);
		}

		private static void MapProperty(IModelInspector modelInspector, PropertyPath member, IPropertyMapper map) {
			var property = member.LocalMember as PropertyInfo;
			if (property == null) return;
			if (IsEnum(property))
			{
				CallGenericTypeMethod(map, property);
			}
		}
	}
}
