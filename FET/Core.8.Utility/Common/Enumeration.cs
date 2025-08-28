using System.Reflection;

namespace Core.Utility.Common
{
    // TODO 待確認
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

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration<TId> =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                        .Select(f => f.GetValue(null))
                        .Cast<T>();

        /// <summary>
        /// 檢查是否相同
        /// </summary>
        /// <param name="obj">列舉</param>
        /// <returns>是/否</returns>
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

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Id.GetHashCode();

        //public static int AbsoluteDifference(Enumeration<TId> firstValue, Enumeration<TId> secondValue)
        //{
        //    var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        //    return absoluteDifference;
        //}

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromValue<T>(TId value) where T : Enumeration<TId>
        {
            var matchingItem = Parse<T, TId>(value, "value", item => item.Id.Equals(value));
            return matchingItem;
        }

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static T FromDisplayName<T>(string displayName) where T : Enumeration<TId>
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration<TId>
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            //if (matchingItem == null)
            //    throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other) => Id.CompareTo(((Enumeration<TId>)other).Id);
    }
}
