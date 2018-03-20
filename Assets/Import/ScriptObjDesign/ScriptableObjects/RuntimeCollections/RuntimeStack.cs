using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeStack<T> : ScriptableObject {
	private readonly Stack<T> stack = new Stack<T>();

	public void Push(T t) {
		stack.Push(t);
	}

	public T Peek() {
		return stack.Peek();
	}

	public T Pop() {
		return stack.Pop();
	}
}