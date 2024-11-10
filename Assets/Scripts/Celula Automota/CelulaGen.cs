using UnityEngine;
using LibNoise;
using LibNoise.Generator;

public class CelulaGen : MonoBehaviour
{
	public int seed;
	private Texture2D terrainTexture;
	public enum Resolution
	{
		Low = 129,
		Medium = 257,
		High = 513,
		Ultra = 1025
	}

	public Resolution resolution = Resolution.High; // Default to 513

	private int GetResolutionValue()
	{
		return (int)resolution;
	}

	[Range(0, 100)]
	public int randomFillPercent;

	public Color lightBrown = new Color(0.6f, 0.4f, 0.2f); 
	public Color darkBrown = new Color(0.3f, 0.2f, 0.1f); 
	private Terrain terrain;
	private float[,] heightMap;

	void Awake()
	{
		GenerateCave(Vector3.zero);
	}

	[ContextMenu("Generate Terrain")]
	public void GenerateTerrain()
	{
		GenerateCave(Vector3.zero);
	}

	[ContextMenu("Clear Terrain")]
	public void ClearTerrain()
	{
		if (terrain != null)
		{
			DestroyImmediate(terrain.gameObject);
		}
	}

	private void GenerateCave(Vector3 position)
	{
		ClearTerrain();
		heightMap = new float[GetResolutionValue(), GetResolutionValue()];

		var terrainData = new TerrainData
		{
			heightmapResolution = GetResolutionValue(),
			size = new Vector3(100, 5, 100)
		};
		terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
		terrain.transform.position = position;
		terrain.enabled = false;

		RandomFillMap();
		for (int i = 0; i < 5; i++)
		{
			SmoothMap();
		}

		ApplyTerrainData(terrainData, heightMap);
		terrain.terrainData = terrainData;
		GenerateTerrainTexture();
		terrain.materialTemplate = new Material(Shader.Find("Unlit/Texture")) { mainTexture = terrainTexture };

		terrain.enabled = true;
	}

	void RandomFillMap()
	{
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < GetResolutionValue(); x++)
		{
			for (int y = 0; y < GetResolutionValue(); y++)
			{
				if (x < 2 || x >= GetResolutionValue() - 2 || y < 2 || y >= GetResolutionValue() - 2)
				{
					heightMap[x, y] = 1;
				}
				else
				{
					heightMap[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}

	void SmoothMap()
	{
		for (int x = 1; x < GetResolutionValue() - 1; x++) 
		{
			for (int y = 1; y < GetResolutionValue() - 1; y++)
			{
				int neighbourWallTiles = GetSurroundingWallCount(x, y);

				if (neighbourWallTiles > 4)
					heightMap[x, y] = 1;
				else if (neighbourWallTiles < 4)
					heightMap[x, y] = 0;
			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				if (neighbourX >= 0 && neighbourX < GetResolutionValue() && neighbourY >= 0 && neighbourY < GetResolutionValue())
				{
					if (neighbourX != gridX || neighbourY != gridY)
					{
						wallCount += (int)heightMap[neighbourX, neighbourY];
					}
				}
				else
				{
					wallCount++;
				}
			}
		}

		return wallCount;
	}

	void ApplyTerrainData(TerrainData terrainData, float[,] map)
	{
		terrainData.SetHeights(0, 0, map);
	}

	void GenerateTerrainTexture()
	{
		terrainTexture = new Texture2D(GetResolutionValue(), GetResolutionValue());

		for (int x = 0; x < GetResolutionValue(); x++)
		{
			for (int y = 0; y < GetResolutionValue(); y++)
			{
				terrainTexture.SetPixel(y, x, heightMap[x, y] == 1 ? lightBrown : darkBrown);
			}
		}


		terrainTexture.Apply();
	}
}
