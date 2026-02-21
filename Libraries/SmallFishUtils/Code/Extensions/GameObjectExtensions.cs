namespace SmallFishUtils;

public static class GameObjectExtensions
{
	/// <summary>
	/// Setup the networking for a GameObject. If already networked and the owner is provided
	/// then the ownership will be assigned to the provided owner.
	/// </summary>
	public static void NetworkSpawnOrAssignOwner(
		this GameObject obj,
		Connection owner = null,
		OwnerTransfer transfer = OwnerTransfer.Takeover,
		NetworkOrphaned orphaned = NetworkOrphaned.ClearOwner )
	{
		if ( !obj.IsValid() )
			return;

		var parent = obj.Parent;
		if ( parent.IsValid() )
			obj.Parent = null;

		if ( !obj.Network.Active )
			obj.NetworkSpawn( new NetworkSpawnOptions() { Owner = owner, OwnerTransfer = transfer, OrphanedMode = orphaned } );
		else if ( Networking.IsActive && owner != null )
			obj.Network.AssignOwnership( owner );

		if ( parent.IsValid() )
			obj.Parent = parent;
	}

	/// <summary>
	/// Play a sound via the GameObject.
	/// </summary>
	public static SoundHandle PlaySound( this GameObject self, SoundEvent sndEvent, SoundSettings? settings = null )
	{
		if ( !self.IsValid() || sndEvent is null )
			return null;

		var handle = self.PlaySound( sndEvent );
		settings ??= new SoundSettings();
		settings.Value.SetHandleSettings( handle );

		var soundHandler = self.Components.GetOrCreate<SoundHandler>();
		soundHandler.AddSound( handle, settings.Value );

		return handle;
	}

	/// <summary>
	/// Play a sound via the GameObject.
	/// </summary>
	public static SoundHandle PlaySound( this GameObject self, string sndPath, SoundSettings? settings = null )
	{
		return ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) ? self.PlaySound( sndEvent, settings ) : null;
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
