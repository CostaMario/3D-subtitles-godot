using Godot;

public partial class Subtitle3D : BoxContainer
{
	private float maxDist = 15f;
	private Node3D target;
	private Camera3D camera;

	private float fuzziness;

	private bool startOnNextFrame = false;

	private RandomNumberGenerator rng;
	private Vector3 offset;

	private float vol;

	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
	}

	public virtual void Initialize(Node3D targ, float maxDistance = 15f, string subtitle = "", float fuzzinessAmmount = 0f, float volume = 1f)
	{
		camera = GetViewport().GetCamera3D();

		target = targ;

		if(subtitle != "")
			(GetNode("Label") as Label).Text = subtitle;

		maxDist = maxDistance;
		fuzziness = fuzzinessAmmount;
		offset = new Vector3(rng.RandfRange(-fuzziness / 2f, fuzziness / 2f), rng.RandfRange(-fuzziness / 2f, fuzziness / 2f), rng.RandfRange(-fuzziness / 2f, fuzziness / 2f));
		
		Visible = false;
		startOnNextFrame = true;

		vol = volume;
	}

	public override void _Process(double delta)
	{
		//Making sure the subtitle is only calculating its position when visible
		//allows us to toggle subtitles by setting the visibility of a common parent node
		if(target != null && (IsVisibleInTree() || startOnNextFrame))
		{
			float dist = camera.GlobalPosition.DistanceTo(target.GlobalPosition);
			float nScale = Mathf.Clamp(1f - (dist/maxDist), 0f, 1f) * (vol + 1);
			Scale = new Vector2(nScale, nScale);

			if(dist < maxDist)
			{
				Vector2 curPosition = camera.UnprojectPosition(target.GlobalPosition + offset) - new Vector2((Size.X * nScale) / 2, (Size.Y * nScale) / 2);
				
				Vector2 screenSize = GetViewport().GetVisibleRect().Size;

				/*This here check is in place to prevent a random bug I frankly couldn't explain.
				At certain angles, the subtitles would go the opposite side of the screen, and the range
				was so narrow that it would appear as a flicker, unless you carefully went over that specific area.
				This check seems to be pretty accurate at capturing the area, and we simply do not update the subtitle position inside it.
				
				Technically this also introduces a bug where you could get the subtitle stuck in one point on the screen
				as long as you looked up along this specific "track" on the screen, but it's much less noticeable, so for now it's staying.

				I suspect the root cause is in the UnprojectPosition function, since projecting positions too outside of view will result in
				the position's X and Y coordinates flipping.
				*/
				if(!(curPosition.Abs().X > screenSize.X * 32 || curPosition.Abs().Y > screenSize.Y * 32))
				{
					if(!camera.IsPositionInFrustum(target.GlobalPosition))
					{
						Vector2 center = new Vector2(screenSize.X / 2f, screenSize.Y / 2f);
						float centerDist = Mathf.InverseLerp(0, center.Length(), curPosition.DistanceTo(center));
						Vector2 dir = center.DirectionTo(curPosition);

						if(camera.IsPositionBehind(target.GlobalPosition))
							curPosition = (-dir * screenSize + center);
						else
							curPosition = (dir * screenSize + center);
					}

					Position = new Vector2(Mathf.Clamp(curPosition.X, 2, screenSize.X - Size.X * nScale), Mathf.Clamp(curPosition.Y, 0, screenSize.Y - Size.Y * nScale));

				}

				ZIndex = -(int)(dist * 256f);
				
				startOnNextFrame = false;
				Visible = true;
			}
		}
	}

	public void StopSubtitle()
	{
		target = null;
		Visible = false;
	}
}
