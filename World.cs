using System;
using Godot;

public partial class World : Node2D
{
	[ExportGroup("Noise Parameter")]
	[Export]
	int noiseType = 1;
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

	uint screenX;
	uint screenY;

	FastNoiseLite noise;
	RandomNumberGenerator rng;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dirt = GetNode<TileMap>("Dirt");
		floor = GetNode<TileMap>("Floor");
		newMapButton = GetNode<Button>("NewMapButton");
		noiseVisual = GetNode<Godot.Sprite2D>("NoiseVisual");

		noise = new FastNoiseLite();

		rng = new RandomNumberGenerator();



		// Sets the noise type
		switch (noiseType)
		{
			case 0:
				noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
				break;
			case 1:
				noise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
				break;
			case 2:
				noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular;
				break;
			case 3:
				noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
				break;
			case 4:
				noise.NoiseType = FastNoiseLite.NoiseTypeEnum.ValueCubic;
				break;
			default:
				noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
				break;
		}

		NoiseTexture2D noiseTexture2D = new NoiseTexture2D();
		noiseTexture2D.Noise = noise;

		noiseTexture2D.Normalize = true;

		noiseVisual.Texture = noiseTexture2D;
		 
		noiseVisual.Hide();


		// Configure the noise parameter
		noise.Frequency = frequency; // Adjust frequency for different levels of detail
		noise.FractalOctaves = fractalOctaves; // Adjust number of octaves for more details
		noise.FractalLacunarity = fractalLacunarity; // Higher octaves producing noise with finer details and rougher appearance
		noise.FractalGain = fractalGain; // Determines strength of subsequent layer of noise


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
		dirt.Clear();
		noise.Seed = (int)rng.Randi();

		// Screen size
		var screenX = GetViewportRect().Size.X;
		var screenY = GetViewportRect().Size.Y;

		// Set the random floor and noise for path
		for (int x = 0; x < screenX; x++)
		{
			for (int y = 0; y < screenY; y++)
			{
				if (noise.GetNoise2D(x, y) < -0)
				{
					dirt.SetCell(0, new Vector2I(x, y), 0, new Vector2I(1, 1));
				}
				else
				{
					floor.SetCell(0, new Vector2I(x, y), 0, randTile());
				}
			}
		}

		GD.Print(noise.GetNoise2D(screenX, screenY));

	}


	public void OnNoiseMapButtonPressed()
	{
		if (noiseVisual.Visible)
			noiseVisual.Hide();
		else
			noiseVisual.Show();
	}
}
