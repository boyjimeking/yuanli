using System;
using System.Collections;
using System.Collections.Generic;

namespace Rubystyle
{
    public static class RubystyleExtension
    {
        /// <summary>
        /// 遍历List集合
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        public static void RubyEach<TSource>(this IEnumerable<TSource> source, Action<TSource> yield)
        {
            foreach (var item in source)
            {
                yield(item);
            }
        }

        /// <summary>
        /// 遍历Hash表
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        public static void RubyEach<TKey, TValue>(this IDictionary<TKey, TValue> source, Action<TKey, TValue> yield)
        {
            foreach (var item in source)
            {
                yield(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 集合映射，返回新的List集合。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        /// <returns></returns>
        public static List<TResult> RubyMap<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> yield)
        {
            List<TResult> ret = new List<TResult>();
            foreach (var item in source)
            {
                ret.Add(yield(item));
            }
            return ret;
        }

        /// <summary>
        /// 参考ruby的inject方法
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="initargs"></param>
        /// <param name="yield"></param>
        /// <returns></returns>
        public static TResult RubyInject<TSource, TResult>(this IEnumerable<TSource> source, TResult initargs, Func<TResult, TSource, TResult> yield)
        {
            TResult result = initargs;
            foreach (var item in source)
            {
                result = yield(result, item);
            }
            return result;
        }

        /// <summary>
        /// 从List集合里筛选所有判断为真的元素。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        /// <returns></returns>
        public static List<TSource> RubySelect<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> yield)
        {
            List<TSource> ret = new List<TSource>();
            foreach (var item in source)
            {
                if (yield(item))
                    ret.Add(item);
            }
            return ret;
        }

        /// <summary>
        /// List集合里所有元素判断为真则返回true，否则返回false。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        /// <returns></returns>
        public static bool RubyAll<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> yield)
        {
            foreach (var item in source)
            {
                if (!yield(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// List集合里任意元素判断为真则返回ture，否则返回false。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        /// <returns></returns>
        public static bool RubyAny<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> yield)
        {
            foreach (var item in source)
            {
                if (yield(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 查找List集合中首次判断为真的元素，没找到任何元素则返回对应类型的默认值。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="yield"></param>
        /// <returns></returns>
        public static TSource RubyFind<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> yield)
        {
            foreach (var item in source)
            {
                if (yield(item))
                    return item;
            }
            return default(TSource);
        }
    }
}