using System;
using DotNet.Interfaces;

namespace DotNet.Strategy
{
	public abstract class TurnStrategyBase
	{
		private TurnStrategyBase _parent;

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


		public static T Create<T>(Action<T> configure = null)
			where T : TurnStrategyBase, new()
		{
			var strategy = new T();
			configure?.Invoke(strategy);
			return strategy;
		}

		public T Append<T>(Action<T> configure = null)
			where T : TurnStrategyBase, new()
		{
			var strategy = new T();
			strategy._parent = this;
			configure?.Invoke(strategy);
			return strategy;
		}
	}
}
