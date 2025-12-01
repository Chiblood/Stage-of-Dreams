/* DialogEvent.cs - Enhanced with method delegate support */

using UnityEngine;
using System;

/// <summary>
/// Abstract base class for all dialog events.
/// Supports SerializeReference for polymorphic serialization in Unity.
/// Enhanced with delegate support for passing methods.
/// </summary>
[System.Serializable]
public abstract class DialogEvent
{
    [SerializeField] protected string eventName;
    [SerializeField] protected bool isEnabled = true;
    [SerializeField, TextArea(2, 4)] protected string description;

    // Delegate support for method passing
    [System.NonSerialized] protected Action onExecuteDelegate;
    [System.NonSerialized] protected Func<bool> validationDelegate;

    /// <summary>
    /// Name identifier for this event
    /// </summary>
    public string EventName
    {
        get => eventName;
        set => eventName = value;
    }

    /// <summary>
    /// Whether this event should execute
    /// </summary>
    public bool IsEnabled
    {
        get => isEnabled;
        set => isEnabled = value;
    }

    /// <summary>
    /// Description of what this event does
    /// </summary>
    public string Description
    {
        get => description;
        set => description = value;
    }

    /// <summary>
    /// Set a custom execution delegate for this event
    /// </summary>
    public void SetExecuteDelegate(Action executeMethod)
    {
        onExecuteDelegate = executeMethod;
    }

    /// <summary>
    /// Set a custom validation delegate for this event
    /// </summary>
    public void SetValidationDelegate(Func<bool> validationMethod)
    {
        validationDelegate = validationMethod;
    }

    /// <summary>
    /// Execute this dialog event
    /// </summary>
    public void Execute()
    {
        if (!isEnabled) return;

        try
        {
            // Execute delegate first if available
            onExecuteDelegate?.Invoke();
            
            // Then execute the event's specific behavior
            OnExecute();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error executing dialog event '{eventName}': {ex.Message}");
        }
    }

    /// <summary>
    /// Override this method to implement the event's specific behavior
    /// </summary>
    protected abstract void OnExecute();

    /// <summary>
    /// Validate that this event can execute properly
    /// </summary>
    public virtual bool IsValid()
    {
        // Use custom validation if available
        if (validationDelegate != null)
        {
            return validationDelegate.Invoke();
        }

        return isEnabled && !string.IsNullOrEmpty(eventName);
    }

    /// <summary>
    /// Get a display name for this event in the Inspector
    /// </summary>
    public virtual string GetDisplayName()
    {
        return string.IsNullOrEmpty(eventName) ? GetType().Name : eventName;
    }
}

// Enhanced DialogEvent implementations with method support

/// <summary>
/// Generic method call event that can execute any Action delegate
/// </summary>
[System.Serializable]
public class MethodCallEvent : DialogEvent
{
    [System.NonSerialized] private Action methodToCall;
    [SerializeField] private string methodDescription;

    public string MethodDescription
    {
        get => methodDescription;
        set => methodDescription = value;
    }

    /// <summary>
    /// Set the method to call when this event executes
    /// </summary>
    public void SetMethod(Action method, string description = "")
    {
        methodToCall = method;
        methodDescription = description;
    }

    protected override void OnExecute()
    {
        methodToCall?.Invoke();
    }

    public override string GetDisplayName()
    {
        return $"Method Call: {methodDescription ?? "Custom Method"}";
    }

    public override bool IsValid()
    {
        return base.IsValid() && methodToCall != null;
    }
}

/// <summary>
/// Parameterized method call event with generic parameter support
/// </summary>
[System.Serializable]
public class ParameterizedMethodEvent<T> : DialogEvent
{
    [System.NonSerialized] private Action<T> methodToCall;
    [SerializeField] private T parameter;
    [SerializeField] private string methodDescription;

    public T Parameter
    {
        get => parameter;
        set => parameter = value;
    }

    public string MethodDescription
    {
        get => methodDescription;
        set => methodDescription = value;
    }

    /// <summary>
    /// Set the method to call with parameter when this event executes
    /// </summary>
    public void SetMethod(Action<T> method, T param, string description = "")
    {
        methodToCall = method;
        parameter = param;
        methodDescription = description;
    }

    protected override void OnExecute()
    {
        methodToCall?.Invoke(parameter);
    }

    public override string GetDisplayName()
    {
        return $"Method Call ({typeof(T).Name}): {methodDescription ?? "Custom Method"}";
    }

    public override bool IsValid()
    {
        return base.IsValid() && methodToCall != null;
    }
}

/// <summary>
/// Event that can call static methods by name using reflection
/// </summary>
[System.Serializable]
public class StaticMethodCallEvent : DialogEvent
{
    [SerializeField] private string className;
    [SerializeField] private string methodName;
    [SerializeField] private string[] stringParameters;

    public string ClassName
    {
        get => className;
        set => className = value;
    }

    public string MethodName
    {
        get => methodName;
        set => methodName = value;
    }

    public string[] StringParameters
    {
        get => stringParameters ?? new string[0];
        set => stringParameters = value;
    }

    protected override void OnExecute()
    {
        try
        {
            var type = Type.GetType(className);
            if (type != null)
            {
                var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                if (method != null)
                {
                    // Convert string parameters to appropriate types
                    var paramTypes = method.GetParameters();
                    object[] convertedParams = new object[paramTypes.Length];
                    
                    for (int i = 0; i < paramTypes.Length && i < stringParameters.Length; i++)
                    {
                        convertedParams[i] = Convert.ChangeType(stringParameters[i], paramTypes[i].ParameterType);
                    }

                    method.Invoke(null, convertedParams);
                }
                else
                {
                    Debug.LogWarning($"Method '{methodName}' not found in class '{className}'");
                }
            }
            else
            {
                Debug.LogWarning($"Class '{className}' not found");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error calling static method '{className}.{methodName}': {ex.Message}");
        }
    }

    public override string GetDisplayName()
    {
        return $"Static Method: {className}.{methodName}";
    }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName);
    }
}
