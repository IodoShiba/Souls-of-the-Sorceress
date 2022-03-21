
using System.Collections.Generic;

public class EnemyCounter
{
    public int countNative {get; private set;} = 0;
    public int countAdded {get; private set;} = 0;
    public int countNativeDefeated {get; private set;} = 0;
    public int countAddedDefeated {get; private set;} = 0;

    public struct InitArg
    {

    }
    void Initialize(InitArg args = new InitArg())
    {

    }

    public int GetAllEnemyCount()
    {
        return countNative + countAdded;
    }
} 
