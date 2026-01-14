namespace SmallFishUtils;

// Originally taken from Facepunch source code, thank you!
public abstract class Singleton<T> : Component, IHotloadManaged where T : Singleton<T>
{
	public static T Current { get; private set; }

	protected override void OnAwake()
	{
		if ( Current.IsValid() )
		{
			Log.Warning( $"Multiple Singletons found of type: {TypeLibrary.GetType<T>().Name}" );
			Destroy();
			return;
		}

		if ( Active )
		{
			Current = (T)this;
		}
	}

	void IHotloadManaged.Destroyed( Dictionary<string, object> state )
	{
		state["IsActive"] = Current == this;
	}

	void IHotloadManaged.Created( IReadOnlyDictionary<string, object> state )
	{
		if ( state.GetValueOrDefault( "IsActive" ) is true )
		{
			Current = (T)this;
		}
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
		{
			Current = null;
		}
	}
}
