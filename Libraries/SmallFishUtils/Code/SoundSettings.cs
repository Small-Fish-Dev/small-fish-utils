namespace SmallFishUtils;

public struct SoundSettings
{
	/// <summary>
	/// Should we follow the GameObject?
	/// </summary>
	public bool Follow = true;

	/// <summary>
	/// Override the pitch?
	/// </summary>
	public float? Pitch = null;

	/// <summary>
	/// Override the volume?
	/// </summary>
	public float? Volume = null;

	/// <summary>
	/// The mixer that the sound will play from.
	/// </summary>
	public string Mixer = "";

	public SoundSettings() { }

	public readonly void SetHandle( SoundHandle handle )
	{
		if ( !Follow )
			handle.FollowParent = false;

		if ( Pitch.HasValue )
			handle.Pitch = Pitch.Value;

		if ( Volume.HasValue )
			handle.Volume = Volume.Value;

		if ( !string.IsNullOrEmpty( Mixer ) )
			handle.TargetMixer = Sandbox.Audio.Mixer.FindMixerByName( Mixer );
	}
}
