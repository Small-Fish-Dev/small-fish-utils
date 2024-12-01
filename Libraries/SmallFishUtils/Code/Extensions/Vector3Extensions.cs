namespace SmallFishUtils;

public static class Vector3Extensions
{
	/// <summary>
	/// Play a sound at the given position.
	/// </summary>
	public static SoundHandle PlaySound( this Vector3 pos, SoundEvent sndEvent, SoundSettings? settings = null )
	{
		if ( sndEvent is null )
			return null;

		var handle = Sound.Play( sndEvent, pos );
		settings ??= new SoundSettings();
		settings.Value.SetHandleSettings( handle );
		return handle;
	}

	public static SoundHandle PlaySound( this Vector3 pos, string sndPath, SoundSettings? settings = null )
	{
		return ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) ? pos.PlaySound( sndEvent, settings ) : null;
	}

	/// <summary>
	/// Broadcast a sound at the given position.
	/// </summary>
	[Rpc.Broadcast]
	public static void BroadcastSound( this Vector3 pos, string soundPath, SoundSettings? settings = null )
	{
		pos.PlaySound( soundPath, settings );
	}

	/// <summary>
	/// Broadcast a sound.
	/// </summary>
	[Rpc.Broadcast]
	public static void BroadcastSound( this Vector3 pos, SoundEvent soundEvent, SoundSettings? settings = null )
	{
		pos.PlaySound( soundEvent, settings );
	}
}
