using System;
using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	public enum QuackType
	{
		Stack,
		Queue,
		PriorityQueue,
		InvertedPriorityQueue
	}

    public interface IQuack<T>
	{
		void Put(T item, int priority = 0);
		bool TryGet(out T item);
		int Count { get; }
	}
    public abstract class Quack<T>
    {
        public static IQuack<T> Create(QuackType type) => type switch
		{
			QuackType.Stack => new QuackStack<T>(),
			QuackType.Queue => new QuackQueue<T>(),
			QuackType.PriorityQueue => new QuackPriorityQueue<T>(1),
			QuackType.InvertedPriorityQueue => new QuackPriorityQueue<T>(-1),
			_ => throw new Exception($"Unhandled type {type}")
		};
    }

	internal class QuackStack<T> : IQuack<T>
	{
		private readonly Stack<T> _stack = new();
		public void Put(T item, int _)
		{
			_stack.Push(item);
		}
		public bool TryGet(out T item)
		{
			return _stack.TryPop(out item);
		}
		public int Count => _stack.Count;
	}

	internal class QuackQueue<T> : IQuack<T>
	{
		private readonly Queue<T> _queue = new();
		public void Put(T item, int _)
		{
			_queue.Enqueue(item);
		}
		public bool TryGet(out T item)
		{
			return _queue.TryDequeue(out item);
		}
		public int Count => _queue.Count;
	}

	internal class QuackPriorityQueue<T> : IQuack<T>
	{
		private readonly PriorityQueue<T,int> _queue = new();
		private readonly int _factor;
		public QuackPriorityQueue(int factor) => _factor = factor;
		public void Put(T item, int priority)
		{
			_queue.Enqueue(item, priority * _factor);
		}
		public bool TryGet(out T item)
		{
			return _queue.TryDequeue(out item, out var _);
		}
		public int Count => _queue.Count;
	}
}