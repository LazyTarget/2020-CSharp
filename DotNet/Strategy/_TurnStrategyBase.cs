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

		protected TurnStrategyBase(TurnStrategyBase parent = null)
		{
			_parent = parent;
		}

		public virtual bool TryExecuteStrategy(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			var result = TryExecuteTurn(randomizer, gameLayer, state);
			if (!result && _parent != null)
				result = _parent.TryExecuteStrategy(randomizer, gameLayer, state);
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

			public T Append<T>(Action<T> configure = null)
				where T : TurnStrategyBase, new()
			{
				var strategy = new T();
				strategy._parent = _current;
				strategy._loggerFactory = _loggerFactory;
				strategy.Logger = _loggerFactory.CreateLogger<T>();

				configure?.Invoke(strategy);
				return strategy;
			}
		}
	}
}
