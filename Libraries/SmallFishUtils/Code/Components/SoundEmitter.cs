namespace SmallFishUtils;

public struct SoundSettings
{
	public bool Follow = true;
	public float? Pitch = null;
	public float? Volume = null;
	public string Mixer = "";

	public SoundSettings() { }
}

// Originally taken from Facepunch source code, thank you!

/// <summary>
/// A simple component that handles playing sounds.
/// </summary>
public sealed class SoundEmitter : Component
{
	/// <summary>
	/// How long until we destroy the GameObject.
	/// </summary>
	[Property]
	public SoundEvent SoundEvent { get; set; }

	/// <summary>
	/// Should we follow the current GameObject?
	/// </summary>
	[Property]
	public bool Follow { get; set; } = true;

	/// <summary>
	/// Should the GameObject be destroyed when the sound has finished?
	/// </summary>
	[Property]
	public bool DestroyOnFinish { get; set; } = true;

	/// <summary>
	/// Which mixer should this sound play on?
	/// </summary>
	[Property]
	public string MixerName { get; set; } = "Game";

	/// <summary>
	/// Should we automatically play the sound when creating the sound emitter, or should we trigger it manually?
	/// </summary>
	[Property]
	public bool AutoStart { get; set; } = true;

	/// <summary>
	/// How much should we scale this sound's volume by?
	/// </summary>
	[Property, ToggleGroup( "VolumeModifier", Label = "Volume Modifier" )]
	public bool VolumeModifier { get; set; } = false;

	/// <summary>
	/// Should we change the volume over time? Good for fading in/out a sound.
	/// </summary>
	[Property, ToggleGroup( "VolumeModifier" )]
	public Curve VolumeOverTime { get; set; } = new( new Curve.Frame( 0f, 1f ), new Curve.Frame( 1f, 1f ) );

	/// <summary>
	/// How long should this sound last until being destroyed, since we're creating a GameObject per-sound, it's useful.
	/// </summary>
	[Property, ToggleGroup( "VolumeModifier" )]
	public float LifeTime { get; set; } = 1f;

	/// <summary>
	/// The pitch the sound should play with.
	/// </summary>
	[Property, Range( 0.1f, 2f )]
	public float? Pitch { get; set; } = null;

	/// <summary>
	/// If set, overrides the volume value of the sound event.
	/// </summary>
	[Property, Range( 0f, 1f, 0.01f, true, true )]
	public float? Volume { get; set; } = null;

	/// <summary>
	/// How long until we started playing the sound?
	/// </summary>
	private TimeSince TimeSincePlayed { get; set; }

	private float _initVolume = 1f;
	private SoundHandle _handle;

	/// <summary>
	/// Play the sound
	/// </summary>
	public void Play()
	{
		_handle?.Stop();

		if ( SoundEvent == null ) return;
		TimeSincePlayed = 0f;
		_handle = Sound.Play( SoundEvent, WorldPosition );
		_handle.TargetMixer = Mixer.FindMixerByName( MixerName );

		if ( Pitch.HasValue )
			_handle.Pitch = Pitch.Value;

		if ( Volume.HasValue )
			_handle.Volume = Volume.Value;

		if ( Follow )
		{
			_handle.FollowParent = true;
			_handle.SetParent( GameObject );
		}

		_initVolume = _handle.Volume;
	}

	protected override void OnStart()
	{
		if ( AutoStart )
		{
			Play();
		}
	}

	protected override void OnUpdate()
	{
		if ( _handle is null )
			return;

		// If we stopped playing, kill the game object (maybe)
		if ( _handle.IsStopped )
		{
			if ( DestroyOnFinish )
				GameObject.Destroy();
		}

		if ( VolumeModifier )
			_handle.Volume = _initVolume * VolumeOverTime.Evaluate( TimeSincePlayed / LifeTime );
	}

	protected override void OnDestroy()
	{
		_handle?.Stop();
		_handle = null;
	}
}
