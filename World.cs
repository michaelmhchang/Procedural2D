using System;
using Godot;

public partial class World : Node2D
{
	[ExportGroup("Noise Parameter")]
	[Export(PropertyHint.Enum, "Simplex, Simplex Smooth, Cellular, Perlin, Value, Value Cubic")]
	public FastNoiseLite.NoiseTypeEnum noiseType { get; set; }
	[Export(PropertyHint.Range, "0, 0.1f")]
	float frequency = 0.02f; // Adjust frequency for different levels of detail
	[Export]
	int fractalOctaves = 5; // Adjust number of octaves for more details
	[Export]
	float fractalLacunarity = 2.0f; // Higher octaves producing noise with finer details and rougher appearance
	[Export]
	float fractalGain = 0.5f; // Determines strength of subsequent layer of noise

	TileMap dirt;
	TileMap floor;
	Button newMapButton;
	Godot.Sprite2D noiseVisual;

	float screenX;
	float screenY;

	FastNoiseLite noise;
	NoiseTexture2D noiseTexture2D;
	RandomNumberGenerator rng;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Screen size
		screenX = GetViewportRect().Size.X;
		screenY = GetViewportRect().Size.Y;

		dirt = GetNode<TileMap>("Dirt");
		floor = GetNode<TileMap>("Floor");
		newMapButton = GetNode<Button>("NewMapButton");
		noiseVisual = GetNode<Godot.Sprite2D>("NoiseVisual");

		noise = new FastNoiseLite();
		noise.NoiseType = noiseType;

		noiseTexture2D = new NoiseTexture2D();

		rng = new RandomNumberGenerator();


		// NOISE STUFF
		// ---------------------------------------------------------------------- 
		// Showing full noise map
		noiseTexture2D.Noise = noise;

		noiseTexture2D.Normalize = true;
		noiseTexture2D.Width = 200;
		noiseTexture2D.Height = 200;

		GD.Print("NoiseTexture2D: " + noiseTexture2D.GetSize());
		GD.Print(screenX + " " + screenY);
		
		noiseVisual.Texture = noiseTexture2D;
		noiseVisual.Offset = new Vector2(110, 110);
		// noiseVisual.Hide();

		// Configure the noise parameter
		noise.Frequency = frequency; // Adjust frequency for different levels of detail
		noise.FractalOctaves = fractalOctaves; // Adjust number of octaves for more details
		noise.FractalLacunarity = fractalLacunarity; // Higher octaves producing noise with finer details and rougher appearance
		noise.FractalGain = fractalGain; // Determines strength of subsequent layer of noise
		// ---------------------------------------------------------------------- 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Return random tile axis
	public Vector2I randTile()
	{
		var x = GD.Randi() % 6;
		var y = GD.Randi() % 1 + 5;

		return new Vector2I((int)x, (int)y);
	}

	public void OnNewMapButtonPressed()
	{
		// TODO: Zoom map out to fit noise map

		dirt.Clear();
		noise.Seed = (int)rng.Randi();

		Color red = new Color("ff0000");

		var noiseX = noiseTexture2D.Width;
		var noiseY = noiseTexture2D.Height;


		// Set the random floor and noise for path
		for (int x = 0; x < noiseTexture2D.Width; x++)
		{
			for (int y = 0; y < noiseTexture2D.Height; y++)
			{
				if (noise.GetNoise2D(x, y) < 0)
				{
					dirt.SetCell(0, new Vector2I(x, y), 0, new Vector2I(1, 1));
				}
				else
				{
					floor.SetCell(0, new Vector2I(x, y), 0, randTile());
				}
			}
		}
	}


	public void OnNoiseMapButtonPressed()
	{
		if (noiseVisual.Visible)
			noiseVisual.Hide();
		else
			noiseVisual.Show();
	}


}
