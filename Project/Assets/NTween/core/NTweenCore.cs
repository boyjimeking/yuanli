

using System;
using System.Collections;
using UnityEngine;

public class NTweenCore
{
    #region platform
    protected static void LogErr(object message)
    {
        throw new Exception("NTween Error: " + message.ToString());
    }
    #endregion
    #region Helper Functions
    protected static T SafeGetVar<T>(Hashtable vars, string key)
    {
        var t = typeof(T);
        if (vars[key] != null)
            return (T)Convert.ChangeType(vars[key], t);//more safe
        else
        {
            return default(T);
        }
    }
    protected static object GetPropValue(object src, string propName)
    {
        var type = src.GetType();
        var property = type.GetProperty(propName);
        if (property != null)
        {
            return property.GetValue(src, null);
        }
        var field = type.GetField(propName);
        if (field != null)
        {
            return field.GetValue(src);
        }
        var method = type.GetMethod(propName, Type.EmptyTypes);
        if (method != null)
        {
            return method.Invoke(src, null);
        }
        if (propName.IndexOf("Set") > -1)
        {
            var getMethod = type.GetMethod("Get" + propName.Substring(propName.IndexOf("Set") + 3));
            if (getMethod != null)
            {
                return getMethod.Invoke(src, null);
            }
        }
        if (propName.IndexOf("set") > -1)
        {
            var getMethod = type.GetMethod("get" + propName.Substring(propName.IndexOf("set") + 3));
            if (getMethod != null)
            {
                return getMethod.Invoke(src, null);
            }
        }
        return null;
    }
    protected static void SetPropValue(object src, string propName, object value)
    {
        var type = src.GetType();
        var propertyInfo = type.GetProperty(propName);
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(src, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            return;
        }
        var field = type.GetField(propName);
        if (field != null)
        {
            field.SetValue(src, Convert.ChangeType(value, field.FieldType));
            return;
        }
        var argType = GetPropType(src, propName);
        if (argType != null)
        {
            var setMethod = type.GetMethod(propName, new Type[] { argType });
            if (setMethod != null)
            {
                setMethod.Invoke(src, new object[] { Convert.ChangeType(value, argType) });
                return;
            }
        }
        throw new Exception("Unkown property:" + propName + " of type:" + src);
    }
    protected static Type GetPropType(object src, string propName)
    {
        var type = src.GetType();
        var property = type.GetProperty(propName);
        if (property != null)
        {
            return property.PropertyType;
        }
        var field = type.GetField(propName);
        if (field != null)
        {
            return field.FieldType;
        }
        var method = type.GetMethod(propName, Type.EmptyTypes);
        if (method != null)
        {
            return method.ReturnType;
        }
        if (propName.IndexOf("Set") > -1)
        {
            var getMethod = type.GetMethod("Get" + propName.Substring(propName.IndexOf("Set") + 3));
            if (getMethod != null)
            {
                return getMethod.ReturnType;
            }
        }
        if (propName.IndexOf("set") > -1)
        {
            var getMethod = type.GetMethod("get" + propName.Substring(propName.IndexOf("set") + 3));
            if (getMethod != null)
            {
                return getMethod.ReturnType;
            }
        }
        return null;
    }
    public static Hashtable Hash(params object[] args)
    {
        Hashtable hashTable = new Hashtable(args.Length / 2);
        if (args.Length % 2 != 0)
        {
            LogErr("Tween Error: Hash requires an even number of arguments!");
            return null;
        }
        else
        {
            int i = 0;
            while (i < args.Length - 1)
            {
                hashTable.Add(args[i], args[i + 1]);
                i += 2;
            }
            return hashTable;
        }
    }
    protected static Hashtable CleanArgs(Hashtable args)
    {
        Hashtable argsCopy = new Hashtable(args.Count);

        foreach (DictionaryEntry item in args)
        {
            argsCopy.Add(item.Key, item.Value);
        }

        foreach (DictionaryEntry item in argsCopy)
        {
            if (item.Value is int)
            {
                int original = (int)item.Value;
                float casted = (float)original;
                args[item.Key] = casted;
            }
            else if (item.Value is double)
            {
                double original = (double)item.Value;
                float casted = (float)original;
                args[item.Key] = casted;
            }
        }

        return args;
    }
    protected static object Subtraction(object a, object b)
    {
        var type = a.GetType();
        var op = type.GetMethod("op_Subtraction", new Type[] { type, b.GetType() });
        if (op == null)
        {
            return PrimitiveSubtraction(a, b);
        }
        return op.Invoke(null, new object[] { a, b });
    }
    protected static float CastToFloat(object a)
    {
        if (a is int)
        {
            int original = (int)a;
            float casted = (float)original;
            return casted;
        }
        else if (a is double)
        {
            double original = (double)a;
            float casted = (float)original;
            return casted;
        }
        else if (a is float)
            return (float)a;
        else
            throw new Exception("can't cast " + a + " of type " + a.GetType() + " to float");
    }
    protected static float PrimitiveSubtraction(object a, object b)
    {
        return CastToFloat(a) - CastToFloat(b);
    }
    protected static object Addition(object a, object b)
    {
        var type = a.GetType();
        var op = type.GetMethod("op_Addition", new Type[] { type, b.GetType() });
        if (op == null)
        {
            return PrimitiveAddition(a, b);
        }
        return op.Invoke(null, new object[] { a, b });
    }
    protected static float PrimitiveAddition(object a, object b)
    {
        return CastToFloat(a) + CastToFloat(b);
    }
    protected static object Multiply(object a, object b)
    {
        var type = a.GetType();
        var op = type.GetMethod("op_Multiply", new Type[] { type, b.GetType() });
        if (op == null)
        {
            return PrimitiveMultiply(a, b);
        }
        return op.Invoke(null, new object[] { a, b });
    }
    protected static float PrimitiveMultiply(object a, object b)
    {
        return CastToFloat(a) * CastToFloat(b);
    }
    protected static object Negation(object a)
    {
        var type = a.GetType();
        var op = type.GetMethod("op_UnaryNegation", new Type[] { type });
        if (op == null)
        {
            return PrimitiveNegation(a);
        }
        return op.Invoke(null, new object[] { a });
    }
    protected static float PrimitiveNegation(object a)
    {
        return -CastToFloat(a);
    }
    #endregion
}

