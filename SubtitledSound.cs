using Godot;

public partial class SubtitledSound : AudioStreamPlayer3D
{
	[Export]
	private string subtitle;

	[Export(PropertyHint.Range, "0,5,")]
	private float visualVolumeMultiplier = 1;

	[Export]
	private float fuzziness = 0f;

	private bool curPlaying = false;
	private int id = 0;

	public override void _Ready()
	{
		TreeExiting += StopOnExit;
		Finished += StopSubtitle;
	}

	public override void _Process(double delta)
	{
		if(curPlaying && !Playing)
			StopSubtitle();
		else if(!curPlaying && Playing)
			StartSubtitle();
	}

	private void StartSubtitle()
	{
		curPlaying = true;

		float m = MaxDistance;

		//Right now, the loudness of the sound is set via the visual volume multiplier property. 
		//I'll change it to automatically detecting it based on the sound once I figure out how to do it.

		id = SubtitleManager.StartSubtitle(this, VolumeDb, UnitSize * visualVolumeMultiplier, m, subtitle, fuzziness:fuzziness);
	}

	private void StopSubtitle()
	{
		curPlaying = false;
		SubtitleManager.StopSubtitle(id);
	}

	private void StopOnExit()
	{
		if(curPlaying)
			StopSubtitle();
	}
}
