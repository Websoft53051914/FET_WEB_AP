using Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Utility
{
    public class EnumUtility
    {
        /// <summary>
        /// 列舉
        /// </summary>
        /// <remarks>
        /// https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Ordering/Ordering.Domain/SeedWork/Enumeration.cs
        /// </remarks>
        public class Enumeration<TId> : IComparable
            where TId : IComparable
        {
            public string Name { get; private set; }

            public TId Id { get; private set; }

            protected Enumeration(TId id, string name) => (Id, Name) = (id, name);

            public override string ToString() => Name;

            public static IEnumerable<T> GetAll<T>() where T : Enumeration<TId> =>
                typeof(T).GetFields(BindingFlags.Public |
                                    BindingFlags.Static |
                                    BindingFlags.DeclaredOnly)
                            .Select(f => f.GetValue(null))
                            .Cast<T>();

            public override bool Equals(object obj)
            {
                if (obj is not Enumeration<TId> otherValue)
                {
                    return false;
                }

                var typeMatches = GetType().Equals(obj.GetType());
                var valueMatches = Id.Equals(otherValue.Id);

                return typeMatches && valueMatches;
            }

            public override int GetHashCode() => Id.GetHashCode();

            //public static int AbsoluteDifference(Enumeration<TId> firstValue, Enumeration<TId> secondValue)
            //{
            //    var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            //    return absoluteDifference;
            //}

            public static T FromValue<T>(TId value) where T : Enumeration<TId>
            {
                var matchingItem = Parse<T, TId>(value, "value", item => item.Id.Equals(value));
                return matchingItem;
            }

            public static T FromDisplayName<T>(string displayName) where T : Enumeration<TId>
            {
                var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
                return matchingItem;
            }

            private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration<TId>
            {
                var matchingItem = GetAll<T>().FirstOrDefault(predicate);

                //if (matchingItem == null)
                //    throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

                return matchingItem;
            }

            public int CompareTo(object other) => Id.CompareTo(((Enumeration<TId>)other).Id);
        }

        public static string GetDescriptionByInt<T>(int value, string defaultVal = "")
            where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                return defaultVal;
            }

            return ((T)(object)value).GetDescription();
        }
    }
}
