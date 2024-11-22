namespace SmallFishUtils;

public static class GameObjectExtensions
{
	/// <summary>
	/// Setup the networking for a game object.
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
	/// Creates a GameObject that plays a sound.
	/// </summary>
	public static void PlaySound( this GameObject self, SoundEvent sndEvent, SoundSettings? settings = null )
	{
		if ( !self.IsValid() )
			return;

		if ( sndEvent is null )
			return;

		settings ??= new SoundSettings();

		var gameObject = self.Scene.CreateObject();
		gameObject.Name = sndEvent.ResourceName;

		if ( settings.Value.Follow )
			gameObject.Parent = self;
		else
			gameObject.Transform.World = self.Transform.World;

		var emitter = gameObject.Components.Create<SoundEmitter>();

		// Point to an emitter if we chose one
		if ( !string.IsNullOrEmpty( settings.Value.Mixer ) )
			emitter.MixerName = settings.Value.Mixer;

		emitter.SoundEvent = sndEvent;
		emitter.Pitch = settings.Value.Pitch;
		emitter.Play();
	}

	/// <summary>
	/// Play a sound given the sound path.
	/// </summary>
	public static void PlaySound( this GameObject self, string sndPath, SoundSettings? settings = null )
	{
		if ( ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) )
			self.PlaySound( sndEvent, settings );
	}

	/// <summary>
	/// Broacast a sound.
	/// </summary>
	[Broadcast]
	public static void BroadcastSound( this GameObject self, string soundPath, SoundSettings? settings = null )
	{
		self.PlaySound( soundPath, settings );
	}

	/// <summary>
	/// Broacast a sound.
	/// </summary>
	[Broadcast]
	public static void BroadcastSound( this GameObject self, SoundEvent soundEvent, SoundSettings? settings = null )
	{
		self.PlaySound( soundEvent, settings );
	}
}
