using System;

namespace LudicoGTK.Plugin;

public class Game
{
    public string GameName { get; set; }

    public string GetGameName()
    {
        return GameName;
    }

    public void SetGameName(string name)
    {
        GameName = name;
    }
}
