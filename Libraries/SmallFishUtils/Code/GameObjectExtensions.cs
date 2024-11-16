namespace SmallFishUtils;

public static class SFUGameObjectExtensions
{
	/// <summary>
	/// Creates a GameObject that plays a sound.
	/// </summary>
	/// <param name="self"></param>
	/// <param name="sndEvent"></param>
	/// <param name="follow">Should this sound follow the GameObject?</param>
	/// <param name="mixerName">The audio mixer, look at the Mixer window in the Editor</param>
	public static void PlaySound( this GameObject self, SoundEvent sndEvent, bool follow = true, string mixerName = null )
	{
		if ( !self.IsValid() )
			return;

		if ( sndEvent is null )
			return;

		var gameObject = self.Scene.CreateObject();
		gameObject.Name = sndEvent.ResourceName;

		if ( follow )
			gameObject.Parent = self;
		else
			gameObject.Transform.World = self.Transform.World;

		var emitter = gameObject.Components.Create<SoundEmitter>();

		// Point to an emitter if we chose one
		if ( !string.IsNullOrEmpty( mixerName ) )
		{
			emitter.MixerName = mixerName;
		}

		emitter.SoundEvent = sndEvent;
		emitter.Play();
	}

	/// <inheritdoc cref="PlaySound(GameObject, SoundEvent, bool, string)"/>
	public static void PlaySound( this GameObject self, string sndPath, bool follow = true, string mixerName = null )
	{
		if ( ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) )
		{
			self.PlaySound( sndEvent, follow, mixerName );
		}
	}

	/// <summary>
	/// Broacast a sound to all players via SoundEmitter.
	/// </summary>
	[Broadcast]
	public static void BroadcastSound( this GameObject self, string soundPath, bool follow = true, string mixerName = null )
	{
		self.PlaySound( soundPath, follow, mixerName );
	}

	/// <summary>
	/// Broacast a sound to all players via SoundEmitter.
	/// </summary>
	[Broadcast]
	public static void BroadcastSound( this GameObject self, SoundEvent soundEvent, bool follow = true, string mixerName = null )
	{
		self.PlaySound( soundEvent, follow, mixerName );
	}
}
