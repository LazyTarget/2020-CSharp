using System;
using DotNet.Interfaces;
using Microsoft.Extensions.Logging;

namespace DotNet.Strategy
{
	public abstract class TurnStrategyBase
	{
		private TurnStrategyBase _parent;
		private ILoggerFactory _loggerFactory;
		protected ILogger Logger { get; private set; }

		private Func<GameState, bool> _predicate;
		private Action<GameState, bool, bool> _callback;

		protected TurnStrategyBase(TurnStrategyBase parent = null)
		{
			_parent = parent;
		}

		public virtual bool TryExecuteStrategy(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			var result = _predicate == null || _predicate(state);
			if (result)
			{
				// Has no predicate or has meet the requirements
				result = TryExecuteTurn(randomizer, gameLayer, state);
			}

			var globalResult = result;
			if (!globalResult && _parent != null)
			{
				// Could not execute, continue with the parent
				globalResult = _parent.TryExecuteStrategy(randomizer, gameLayer, state);
			}

			// Make a callback that the strategy has succeeded
			if (_callback != null)
			{
				_callback?.Invoke(state, result, globalResult);
			}
			return result;
		}

		protected abstract bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state);


		public static StrategyBuilder Build(ILoggerFactory loggerFactory)
		{
			var builder = new StrategyBuilder(loggerFactory);
			return builder;
		}

		public T Append<T>(Action<T> configure = null)
			where T : TurnStrategyBase, new()
		{
			var strategy = new T();
			strategy._parent = this;
			strategy._loggerFactory = _loggerFactory;
			strategy.Logger = _loggerFactory.CreateLogger<T>();

			configure?.Invoke(strategy);
			return strategy;
		}


		public class StrategyBuilder
		{
			private readonly ILoggerFactory _loggerFactory;

			private TurnStrategyBase _current;

			public StrategyBuilder(ILoggerFactory loggerFactory)
			{
				_loggerFactory = loggerFactory;
			}

			public StrategyBuilder Append<T>(Action<T> configureStrategy = null, Func<GameState, bool> predicate = null, Action<GameState, bool, bool> callback = null)
				where T : TurnStrategyBase, new()
			{
				var strategy = new T();
				strategy._parent = _current;
				strategy._predicate = predicate;
				strategy._callback = callback;
				strategy._loggerFactory = _loggerFactory;
				strategy.Logger = _loggerFactory.CreateLogger<T>();

				configureStrategy?.Invoke(strategy);
				_current = strategy;
				return this;
			}

			public TurnStrategyBase Compile()
			{
				return _current;
			}
		}
	}
}
