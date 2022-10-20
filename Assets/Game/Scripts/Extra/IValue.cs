using UnityEngine.Events;

public interface IValue<T> : IReadOnlyValue<T>
{
    new T CurrentValue { get; set; }
}

public interface IReadOnlyValue<T> : IObservableValue
{
	T CurrentValue { get; }
}

public interface IBounded<T>
{
	T MinValue { get; }
	T MaxValue { get; }
}

public interface IBar : IBounded<float>
{
	float PercentValue { get; }
}


public interface IObservableValue
{
	event UnityAction onChanged;
}
