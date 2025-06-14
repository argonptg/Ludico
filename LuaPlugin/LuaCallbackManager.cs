using System.Collections.Generic;
using NLua;

namespace LudicoGTK.LuaPlugin;

public class LuaCallbackManager
{
    private static readonly Dictionary<string, LuaFunction> Callbacks = new();

    public static void RegisterCallback(string name, LuaFunction func)
    {
        Callbacks[name] = func;
    }

    public static void InvokeCallback(string name, params object[] args)
    {
        if (Callbacks.TryGetValue(name, out var func))
        {
            func.Call(args);
        }
    }
}