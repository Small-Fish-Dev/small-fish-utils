namespace SmallFishUtils;

public static class GameObjectExtensions
{
	/// <summary>
	/// Setup the networking for a GameObject.
	/// </summary>
	public static void SetupNetworking(
		this GameObject obj,
		Connection owner = null,
		OwnerTransfer transfer = OwnerTransfer.Takeover,
		NetworkOrphaned orphaned = NetworkOrphaned.ClearOwner )
	{
		if ( !obj.IsValid() )
			return;

		obj.NetworkMode = NetworkMode.Object;

		if ( !obj.Network.Active )
			obj.NetworkSpawn( owner );
		else if ( Networking.IsActive && owner != null )
			obj.Network.AssignOwnership( owner );

		obj.Network.SetOwnerTransfer( transfer );
		obj.Network.SetOrphanedMode( orphaned );
	}

	/// <summary>
	/// Play a sound via the GameObject.
	/// </summary>
	public static void PlaySound( this GameObject self, SoundEvent sndEvent, SoundSettings? settings = null )
	{
		if ( !self.IsValid() )
			return;

		if ( sndEvent is null )
			return;

		settings ??= new SoundSettings();
		settings.Value.SetHandle( self.PlaySound( sndEvent ) );
	}

	/// <summary>
	/// Play a sound via the GameObject.
	/// </summary>
	public static void PlaySound( this GameObject self, string sndPath, SoundSettings? settings = null )
	{
		if ( ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) )
			self.PlaySound( sndEvent, settings );
	}

	/// <summary>
	/// Broadcast a sound via the GameObject.
	/// </summary>
	[Rpc.Broadcast]
	public static void BroadcastSound( this GameObject self, string soundPath, SoundSettings? settings = null )
	{
		self.PlaySound( soundPath, settings );
	}

	/// <summary>
	/// Broadcast a sound via the GameObject.
	/// </summary>
	[Rpc.Broadcast]
	public static void BroadcastSound( this GameObject self, SoundEvent soundEvent, SoundSettings? settings = null )
	{
		self.PlaySound( soundEvent, settings );
	}
}
