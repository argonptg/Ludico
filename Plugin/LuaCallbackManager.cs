using System.Collections.Generic;
using NLua;

namespace LudicoGTK.Plugin;

public class LuaCallbackManager
{
    private static readonly Dictionary<string, LuaFunction> Callbacks = new();

    public static void RegisterCallback(string name, LuaFunction func)
    {
        Callbacks[name] = func;
    }

    public static object[] InvokeCallback(string name, params object[] args)
    {
        if (Callbacks.TryGetValue(name, out var func))
        {
            return func.Call(args);
        }

        return null;
    }
}