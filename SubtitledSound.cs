using Godot;

public partial class SubtitledSound : Node
{
	[Export]
	private string subtitle;

	[Export(PropertyHint.Range, "0,5,")]
	private float visualVolumeMultiplier = 1;

	[Export]
	private float fuzziness = 0f;

	private bool curPlaying = false;
	private int id = 0;

	[Export]
	private float maxDistance;

	[Export]
	private float volume;

	[Export]
	private float duration;

	private Timer t;

	public override void _Ready()
	{
		TreeExiting += StopOnExit;

		t = new Timer();
		AddChild(t);

		t.WaitTime = duration;
		t.OneShot = true;
		t.Timeout += StopSubtitle;
	}

	public override void _Process(double delta)
	{

	}

	public void StartSubtitle()
	{
		t.Stop();
		curPlaying = true;

		float m = maxDistance;

		//Right now, the loudness of the sound is set via the visual volume multiplier property. 
		//I'll change it to automatically detecting it based on the sound once I figure out how to do it.

		id = SubtitleManager.StartSubtitle(GetParent<Node3D>(), volume, visualVolumeMultiplier, m, subtitle, fuzziness:fuzziness);
		t.Start();
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
