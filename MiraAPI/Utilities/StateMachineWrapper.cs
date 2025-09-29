using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;

namespace MiraAPI.Utilities;

/// <summary>
/// A wrapper for state machine objects to access their parent instance and state.
/// </summary>
/// <typeparam name="T">The type of the parent class that owns the state machine.</typeparam>
public class StateMachineWrapper<T> where T : Il2CppObjectBase
{
    private readonly Il2CppObjectBase _stateMachine;
    private readonly PropertyInfo _thisProperty;
    private readonly PropertyInfo _stateProperty;

    private T? _parentInstance;

    /// <summary>
    /// Gets the instance of the parent class that owns the state machine.
    /// </summary>
    public T Instance => _parentInstance ??= (T)_thisProperty.GetValue(_stateMachine)!;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMachineWrapper{T}"/> class.
    /// </summary>
    /// <param name="stateMachine">The state machine instance to wrap.</param>
    public StateMachineWrapper(Il2CppObjectBase stateMachine)
    {
        _stateMachine = stateMachine;

        var type = _stateMachine.GetType();
        _thisProperty = AccessTools.Property(type, "__4__this");
        _stateProperty = AccessTools.Property(type, "__1__state");
    }

    /// <summary>
    /// Gets the current state of the state machine.
    /// </summary>
    /// <returns>The current state as an integer.</returns>
    public int GetState() => (int)_stateProperty.GetValue(_stateMachine)!;
}
