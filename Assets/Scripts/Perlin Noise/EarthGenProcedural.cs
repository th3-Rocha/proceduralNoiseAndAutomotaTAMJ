using UnityEngine;
using LibNoise;
using LibNoise.Generator;

public class EarthGenProcedural : MonoBehaviour
{
	public int seed;
	public Gradient gradient;
	private Noise2D m_noiseMap;
	private Texture2D terrainTexture;
	public float offsetYHeight = 0;
	public int resolution = 513;
	public float zoom = 1f;
	float offsetX;
	float offsetZ;
	public int perlinOctaves = 6;
	public float terrainSpacing = 50f;
	public int terrainCount = 5;
	public float Increment = 2;

	private int direction = 0;
	private int stepsInCurrentDirection = 1;
	private int stepsTakenInCurrentDirection = 0;
	private int layerCount = 0;


	void Awake()
	{
		genTerrains();
	}


	[ContextMenu("Generate Terrains")]
	public void genTerrains()
	{
		GenerateTerrain(Vector3.zero);
		for (int i = 0; i < terrainCount; i++)
		{
			MoveInSpiral();
			GenerateTerrain(new Vector3(offsetX * terrainSpacing, 0, offsetZ * terrainSpacing));

		}

	}
	[ContextMenu("Clear All Terrains")]
	public void ClearAllTerrains()
	{
		offsetX = 0;
		offsetZ = 0;
		direction = 0;
		stepsInCurrentDirection = 1;
		stepsTakenInCurrentDirection = 0;
		Terrain[] allTerrains = Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None);
		foreach (Terrain terrain in allTerrains)
		{
			if (terrain != null)
			{
				DestroyImmediate(terrain.gameObject);
			}
		}
	}

	void FixedUpdate()
	{
		if (Input.GetButtonDown("Fire1"))
		{

		}
	}

	private void GenerateTerrains()
	{

		for (int i = 0; i < terrainCount; i++)
		{
			MoveInSpiral();
			GenerateTerrain(new Vector3(offsetX * terrainSpacing, 0, offsetZ * terrainSpacing));

		}
	}


	private void MoveInSpiral()
	{
		switch (direction)
		{
			case 0: offsetX += Increment; break;
			case 1: offsetZ += Increment; break;
			case 2: offsetX -= Increment; break;
			case 3: offsetZ -= Increment; break;
		}

		stepsTakenInCurrentDirection++;
		if (stepsTakenInCurrentDirection >= stepsInCurrentDirection)
		{
			stepsTakenInCurrentDirection = 0;
			direction = (direction + 1) % 4;
			layerCount++;
			if (layerCount % 2 == 0)
				stepsInCurrentDirection++;
		}
	}

	private void GenerateTerrain(Vector3 position)
	{

		var terrainData = new TerrainData
		{
			heightmapResolution = resolution,
			size = new Vector3(100, 35, 100)
		};

		var terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
		terrain.transform.position = position;
		terrain.enabled = false;
		ModuleBase noiseModule = new Perlin { OctaveCount = perlinOctaves, Seed = seed };
		m_noiseMap = new Noise2D(resolution, resolution, noiseModule);
		float halfResolution = 1 / zoom;

		m_noiseMap.GeneratePlanar(
			offsetX - halfResolution,
			offsetX + halfResolution,
			offsetZ - halfResolution,
			offsetZ + halfResolution,
			true
		);

		terrainTexture = m_noiseMap.GetTexture(gradient);
		terrainTexture.Apply();


		var terrainLayer = new TerrainLayer
		{
			diffuseTexture = terrainTexture,
			tileSize = new Vector2(terrainData.size.x, terrainData.size.z)
		};
		terrainData.terrainLayers = new[] { terrainLayer };


		ApplyHeightmap(terrainData);


		terrain.terrainData = terrainData;
		terrain.enabled = true;
	}

	private void ApplyHeightmap(TerrainData terrainData)
	{

		float[,] noiseData = m_noiseMap.GetData(false);
		int width = noiseData.GetLength(0);
		int height = noiseData.GetLength(1);
		float[,] heightMap = new float[resolution, resolution];

		float heightOffset = 0.35f + offsetYHeight;

		for (int y = 0; y < resolution; y++)
		{
			for (int x = 0; x < resolution; x++)
			{

				float normalizedHeight = Mathf.Clamp(
					noiseData[y, x] / 2f + heightOffset,
					0f,
					1f
				);
				heightMap[x, y] = normalizedHeight;
			}
		}

		terrainData.SetHeights(0, 0, heightMap);
	}
}