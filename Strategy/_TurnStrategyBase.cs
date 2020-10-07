namespace DotNet.Strategy
{
	public abstract class TurnStrategyBase
	{
		private readonly TurnStrategyBase _parent;

		protected TurnStrategyBase(TurnStrategyBase parent = null)
		{
			_parent = parent;
		}

		public virtual bool TryExecuteStrategy(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			var result = TryExecuteTurn(randomizer, gameLayer, state);
			if (!result && _parent != null)
				result = _parent.TryExecuteStrategy(randomizer, gameLayer, state);
			return result;
		}

		protected abstract bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state);
	}
}
