namespace SmallFishUtils;

public static class Vector3Extensions
{
	/// <summary>
	/// Play a sound at the given position.
	/// </summary>
	public static void PlaySound( this Vector3 pos, SoundEvent sndEvent, SoundSettings? settings = null )
	{
		if ( sndEvent is null )
			return;

		settings ??= new SoundSettings();

		var handle = Sound.Play( sndEvent, pos );

		if ( settings.Value.Pitch.HasValue )
			handle.Pitch = settings.Value.Pitch.Value;

		if ( settings.Value.Volume.HasValue )
			handle.Volume = settings.Value.Volume.Value;

		if ( !string.IsNullOrEmpty( settings.Value.Mixer ) )
			handle.TargetMixer = Mixer.FindMixerByName( settings.Value.Mixer );
	}

	public static void PlaySound( this Vector3 pos, string sndPath, SoundSettings? settings = null )
	{
		if ( ResourceLibrary.TryGet<SoundEvent>( sndPath, out var sndEvent ) )
			pos.PlaySound( sndEvent, settings );
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
