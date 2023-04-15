using Godot;
using System;
using System.Collections.Generic;

public partial class SubtitleManager : Node
{
	[Export(PropertyHint.File)]
	private string subtitle3DInstancePath;
	private static PackedScene subtitleSoundScene;

	private static Godot.Collections.Dictionary<int, Subtitle3D> subtitles;

	private static Random randomId;

	internal static CanvasLayer canvas;

	internal static Queue<Subtitle3D> subtitlesPool;

	private static int maxSubtitles = 256;

	public static bool subtitlesEnabled = true;

	public override void _Ready()
	{
		subtitleSoundScene = ResourceLoader.Load(subtitle3DInstancePath) as PackedScene;

		subtitles = new Godot.Collections.Dictionary<int, Subtitle3D>();
	
		randomId = new Random();

		subtitlesPool = new Queue<Subtitle3D>();

		canvas = GetNode("CanvasLayer") as CanvasLayer;
	}

	public Subtitle3D GetSubtitle(int id)
	{
		if(subtitles.ContainsKey(id))
			return subtitles[id];
		
		return null;
	}

	public static int StartSubtitle(Node3D subPosition, float volumeDB, float unitSize, float maxDist = 0f, string subtitle = "", bool essentialSubtitle = false)
	{
		//Every subtitle has its own ID, which is how we keep track of them
		//both from the Manager as well as from the corresponding SubtitledSound
		int newId = 0;
		while(subtitles.ContainsKey(newId))
			newId = randomId.Next();

		Subtitle3D newSubtitle;

		//I added object pooling to improve performance, and it did actually make a considerable difference
		if(subtitlesPool.Count > 0)
		{
			newSubtitle = subtitlesPool.Dequeue();
		}
		else
		{
			newSubtitle = subtitleSoundScene.Instantiate() as Subtitle3D;
			canvas.AddChild(newSubtitle);
		}

		float adjustedMaxDist = Mathf.Pow(10f, volumeDB / 10f) * unitSize;

		if(maxDist != 0f)
			adjustedMaxDist = Mathf.Clamp(adjustedMaxDist, 0, maxDist);
		
		newSubtitle.Initialize(subPosition, adjustedMaxDist, subtitle);
		
		subtitles.Add(newId, newSubtitle);

		return newId;
	}

	public static void StopSubtitle(int id)
	{
		if(subtitles.ContainsKey(id))
		{
			if(subtitlesPool.Count < maxSubtitles)
			{
				subtitles[id].StopSubtitle();
				subtitlesPool.Enqueue(subtitles[id]);
			}
			else
			{
				subtitles[id].QueueFree();
			}
			subtitles.Remove(id);
		}
	}
}
