namespace SmallFishUtils;

/// <summary>
/// SoundSettings that are applied to the SoundHandle.
/// </summary>
public struct SoundSettings
{
	/// <summary>
	/// Override the pitch?
	/// </summary>
	public float? Pitch = null;

	/// <summary>
	/// Override the volume?
	/// </summary>
	public float? Volume = null;

	/// <summary>
	/// The time it takes for the sound to fade.
	/// </summary>
	public float FadeTime = 0;

	/// <summary>
	/// The mixer that the sound will play from.
	/// </summary>
	public string Mixer = "";

	/// <summary>
	/// Should we follow the GameObject?
	/// </summary>
	public bool Follow = true;

	/// <summary>
	/// When should the sound stop... if at all?
	/// </summary>
	public StopCondition StopOn = StopCondition.Destroy;

	[Flags]
	public enum StopCondition
	{
		None = 0, // The sound will stop when it is finished playing.
		Destroy = 1 << 0, // The sound stops when the component is destroyed.
		Disabled = 1 << 1 // The sound stops when the component is disabled.
	}

	public SoundSettings() { }

	public void SetHandleSettings( SoundHandle handle )
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

/// <summary>
/// A component that handles the lifetime of sounds. If the parent GameObject is destroyed the
/// sounds are stopped as well.
/// </summary>
public sealed class SoundHandler : Component
{
	public struct SoundConfig
	{
		[KeyProperty] public readonly string Name => Handle.IsValid() ? Handle.Name : "";
		[Hide] public SoundHandle Handle;
		[Hide] public SoundSettings Config;
	}

	[Property, ReadOnly]
	public List<SoundConfig> ActiveSounds { get; set; } = new();

	public void AddSound( SoundHandle handle, SoundSettings soundSettings )
	{
		if ( !handle.IsValid() )
			return;

		ActiveSounds.Add( new SoundConfig() { Handle = handle, Config = soundSettings } );
	}

	protected override void OnUpdate()
	{
		for ( int i = ActiveSounds.Count - 1; i >= 0; i-- )
		{
			var sound = ActiveSounds[i].Handle;
			if ( !sound.IsValid() || sound.IsStopped || sound.Finished )
				ActiveSounds.RemoveAt( i );
		}
	}

	protected override void OnDisabled()
	{
		foreach ( var sound in ActiveSounds )
		{
			if ( sound.Handle.IsValid() && sound.Config.StopOn.HasFlag( SoundSettings.StopCondition.Disabled ) )
				sound.Handle.Stop( sound.Config.FadeTime );
		}
	}

	protected override void OnDestroy()
	{
		foreach ( var sound in ActiveSounds )
		{
			if ( sound.Handle.IsValid() && sound.Config.StopOn.HasFlag( SoundSettings.StopCondition.Destroy ) )
				sound.Handle.Stop( sound.Config.FadeTime );
		}
	}
}
