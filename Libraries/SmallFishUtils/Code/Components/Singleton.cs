namespace SmallFishUtils;

// Originally taken from Facepunch source code, thank you!
public abstract class Singleton<T> : Component, IHotloadManaged where T : Singleton<T>
{
	public static T Instance { get; private set; }

	protected override void OnAwake()
	{
		if ( Instance.IsValid() )
		{
			Log.Warning( $"Multiple Singletons found of type: {TypeLibrary.GetType<T>().Name}" );
			Destroy();
			return;
		}

		if ( Active )
		{
			Instance = (T)this;
		}
	}

	void IHotloadManaged.Destroyed( Dictionary<string, object> state )
	{
		state["IsActive"] = Instance == this;
	}

	void IHotloadManaged.Created( IReadOnlyDictionary<string, object> state )
	{
		if ( state.GetValueOrDefault( "IsActive" ) is true )
		{
			Instance = (T)this;
		}
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
		{
			Instance = null;
		}
	}
}
