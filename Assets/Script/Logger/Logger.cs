using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;

public interface ILogger
{
    abstract void Log(string _log);
    abstract void LogWarning(string _log);
    abstract void LogError(string _log);
}
/// <summary>
/// TODO :: Record Interface 는 추후에 진짜 필요하면 다시 구현하자.
/// </summary>
public interface ILoggerRecord
{
    abstract void RecordLog(string _log);
}

/// <summary>
/// UnityEngine 제공 Logger 입니다.
/// </summary>
public class UnityLogger : ILogger
{
    private static UnityLogger _Instance;
    public static UnityLogger GetInstance()
    {
        if (_Instance == null)
            _Instance = new UnityLogger();

        return _Instance;
    }
    public void Log(string _log) => UnityEngine.Debug.Log(_log);
    public void LogError(string _log) => UnityEngine.Debug.LogError(_log);
    public void LogWarning(string _log)  => UnityEngine.Debug.LogWarning(_log);
    public void LogFuncFailed(string _className, string _callFuncName, string _reason) => UnityEngine.Debug.Log($"[{_className}] {_callFuncName}, {_reason}");
}