using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://youtu.be/FlzYOv4TCDE?list=PLn1X2QyVjFVDE9syarF1HoUFwB_3K7z2y&t=366

public class WorldGen : MonoBehaviour
{
    [Header("Lighting")]
    public Texture2D worldTilesMap;
    public Material lightShader;
    public float groundLightThreshold = .7f;
    public float airLightThreshold = .85f;
    public float lightRadius = 7f;
    List<Vector2Int> unlitBlocks = new List<Vector2Int>();

    [Header("Player Misc")]
    public PlayerControl player;
    public CamControl mainCam;
    public GameObject tileDrop;

    [Header("World Stuff")]
    public TileAtlas tileAtlas;
    public float seed;
    public BiomeClass[] biomes;

    [Header("Biomes")]
    public float biomeFreq;
    public Gradient biomeGradient;
    public Texture2D biomeMap;

    [Header("World Generation")]
    public int chunkSize = 32;
    public int worldSize = 128;
    public int heightAdd = 25;
    public bool caveGen = true;

    [Header("Noise Settings")]
    public float caveFreq = .05f;
    public float terrainFreq = .05f;
    public Texture2D caveNoiseTexture;

    [Header("Ores")]   
    public OreClass[] ores;

    public GameObject[] worldChunks;

    public GameObject[,] world_SceneObjects;
    public GameObject[,] world_BackdropObjects;

    public TileClass[,] world_BackdropTiles;
    public TileClass[,] world_SceneTiles;

    public BiomeClass curBiome;
    public Color[] biomeCols;

    private void Start()
    {
        world_SceneTiles = new TileClass[worldSize, worldSize];
        world_BackdropTiles = new TileClass[worldSize, worldSize];
        world_SceneObjects = new GameObject[worldSize, worldSize];
        world_BackdropObjects = new GameObject[worldSize, worldSize];

        //initilise light
        worldTilesMap = new Texture2D (worldSize, worldSize);
        worldTilesMap.filterMode = FilterMode.Point; //disable to have smooth lighting
        lightShader.SetTexture("_ShadowTex", worldTilesMap);

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                worldTilesMap.SetPixel(x, y, Color.white);
            }
        }
        worldTilesMap.Apply();
        
        //terrain stuff
        seed = Random.Range(-32767, 32767);
        //Debug.Log("World Seed = " + seed);
        
        for (int i = 0; i < ores.Length; i++)
        {
            ores[i].distributionTexture = new Texture2D(worldSize, worldSize);
        }

        biomeCols = new Color[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeCols[i] = biomes[i].biomeCol;
        }

        //DrawTextures();
        DrawBiomeMap();
        DrawCavesAndOres();

        CreateChunks(); //create chunks before terrain otherwise breaks project
        TerrainGen();

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                if(worldTilesMap.GetPixel(x, y) == Color.white)
                    LightBlock(x, y, 1f, 0);
            }
        }
        worldTilesMap.Apply();

        mainCam.Spawn(new Vector3(player.spawnPos.x, player.spawnPos.y, mainCam.transform.position.z));
        mainCam.worldSize = worldSize;
        player.Spawn();

        RefreshChunks();
    }

    void Update()
    {
        RefreshChunks();

    }

    void RefreshChunks()
    {
        for (int i = 0; i < worldChunks.Length; i++)
        {
            if (Vector2.Distance(new Vector2((i * chunkSize) + (chunkSize / 2), 0), new Vector2(player.transform.position.x, 0)) > Camera.main.orthographicSize * 4f)
                worldChunks[i].SetActive(false);
            else
                worldChunks[i].SetActive(true);
        }
    }

    public void DrawBiomeMap()
    {
        float b;
        Color col;
        biomeMap = new Texture2D(worldSize, worldSize);
        for(int x = 0; x < biomeMap.width; x++)
        {
            for(int y = 0; y < biomeMap.height; y++)
            {
                b = Mathf.PerlinNoise((x + seed) * biomeFreq, (y + seed) * biomeFreq);
                col = biomeGradient.Evaluate(b);
                biomeMap.SetPixel(x, y, col);
            }
        }

        biomeMap.Apply();
    }

    public void DrawCavesAndOres()
    {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        float v;
        float o;
        for (int x = 0; x < caveNoiseTexture.width; x++)
        {
            for (int y = 0; y < caveNoiseTexture.height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                if(v > curBiome.surfaceVal)
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                else
                    caveNoiseTexture.SetPixel(x, y, Color.black);

                for (int i = 0; i < ores.Length; i++)
                {
                    ores[i].distributionTexture.SetPixel(x, y, Color.black);
                    if (curBiome.ores.Length >= i + 1)
                    {
                        o = Mathf.PerlinNoise((x + seed) * curBiome.ores[i].frequency, (y + seed) * curBiome.ores[i].frequency);
                        if(o > curBiome.ores[i].size)
                            ores[i].distributionTexture.SetPixel(x, y, Color.white);
                    
                        ores[i].distributionTexture.Apply();
                    }
                }
            }
        }
        
        caveNoiseTexture.Apply();      
    }

    public void DrawTextures()
    {
        for(int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int o = 0; o < biomes[i].ores.Length; o++)
            {
                biomes[i].ores[o].distributionTexture = new Texture2D(worldSize, worldSize);
                GenerateNoiseTextures(biomes[i].ores[o].frequency, biomes[i].ores[o].size, biomes[i].ores[o].distributionTexture);
            }
        }
    }

    public void GenerateNoiseTextures(float frequency, float limit, Texture2D noiseTexture)
    {
        //gen caves
        float v;

        for(int x = 0; x < noiseTexture.width; x++)
        {
            for(int y = 0; y < noiseTexture.height; y++)
            {
                v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);

                if(v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }

        noiseTexture.Apply();
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i< numChunks; i++){
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }

    public BiomeClass GetCurrentBiome(int x, int y)
    {
        if(System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) >= 0)
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];

        return curBiome;
    }

    public void TerrainGen()
    {
        TileClass tileClass;
        for(int x = 0; x < worldSize - 1; x++)
        {
            float height;
            for(int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);

                height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * curBiome.heightMultiplier + heightAdd;
                if(x == worldSize / 2)
                    player.spawnPos = new Vector2 (x, height + 2);

                if(y >= height){
                    break;
                }

                if(y < height - curBiome.dirtHeight)
                {

                    tileClass = curBiome.tileAtlas.stone;

                    if(ores[0].distributionTexture.GetPixel(x, y).r > .5f && height - y > ores[0].maxSpawnHeight)
                    {
                        tileClass = tileAtlas.coal;
                    }
                    if(ores[1].distributionTexture.GetPixel(x, y).r > .5f && height - y > ores[1].maxSpawnHeight)
                    {
                        tileClass = tileAtlas.iron;
                    }
                    if(ores[2].distributionTexture.GetPixel(x, y).r > .5f && height - y > ores[2].maxSpawnHeight)
                    {
                        tileClass = tileAtlas.gold;
                    }
                    if(ores[3].distributionTexture.GetPixel(x, y).r > .5f && height - y > ores[3].maxSpawnHeight)
                    {
                        tileClass = tileAtlas.diamond;
                    }
                }

                else if(y < height - 1)
                {
                    tileClass = curBiome.tileAtlas.dirt;
                }

                else
                {
                    //top layer of surface
                    tileClass = curBiome.tileAtlas.grass;

                }

                if(y == 0)
                    //set bottom layer to bedrock
                    tileClass = tileAtlas.bedrock;

                if(caveGen && y > 0)
                {
                    if(caveNoiseTexture.GetPixel(x, y).r > .5f)
                    {
                        PlaceTile(tileClass, x, y, true);
                    }  
                    else if(tileClass.wallVariant != null)
                    {
                        PlaceTile(tileClass.wallVariant, x, y, true);
                    }
                }

                else
                {
                    PlaceTile(tileClass, x, y, true);
                }
                
                if(y >= height - 1)
                {
                    int tree = Random.Range(0, curBiome.treeGenChance);
                    if(tree == 1)
                    {
                        //gen tree
                        if(GetTileFromWorld(x, y))  
                            if(curBiome.biomeName == "Desert")
                            {
                                //gen cactus
                                CreateCacti(curBiome.tileAtlas, Random.Range(curBiome.smallTree, curBiome.largeTree), x, y + 1);
                            } 

                            else                       
                                CreateTree(Random.Range(curBiome.smallTree, curBiome.largeTree),x, y + 1);
                    }
                    else
                    {
                        int i = Random.Range(0, curBiome.tallGrassChance);
                        if(i == 1)
                        {
                            //gen grass
                            if(GetTileFromWorld(x, y))
                            {   
                                if(curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass, x, y + 1, true);
                            }   
                        }
                    }
                }
            }

        }

        worldTilesMap.Apply();
    }

    void CreateCacti(TileAtlas atlas, int treeHeight, int x, int y)
    {
        for(int i = 0; i < treeHeight ; i++)
        { 
            //decide a random height for the cacti
            PlaceTile(atlas.log, x, y + i, true);
        }
    }

    void CreateTree(int treeHeight, int x, int y)
    {
        PlaceTile(tileAtlas.trunk, x, y, true); //start of the tree
            for(int i = 0; i < treeHeight ; i++)
            { 
                //decide a random height for the tree
                PlaceTile(tileAtlas.log, x, (y + 1) + i, true);
            }
        //leaves
        PlaceTile(tileAtlas.leaves, x, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaves, x + 1, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaves, x - 1, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaves, x, y + treeHeight + 2, true);
        
        /*fixed problem with tree generation:
        code generates the trunk of the tree first, then from that it randomises the tree length
        but once the tree length is determined it runs and places down those logs. However, it didnt
        take the trunk into the code, so to fix it i pretended the trunks y pos was 0, then made it so 
        the tree builds up from y = 0, so no logs share a block with the trunk.
        */
    }

    public bool RemoveTileWithTool(int x, int y, ItemClass item)
    {
        if (GetTileFromWorld(x, y) && x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            TileClass tile = GetTileFromWorld(x, y);
            if(tile.toolToBreak == ItemClass.ToolType.none)
            {
                RemoveTile(x, y);
                return true;
            }
            else
            {   
                if(item != null)
                {
                    if(item.itemType == ItemClass.ItemType.tool)
                    {
                        if(tile.toolToBreak == item.toolType)
                        {
                            RemoveTile(x, y);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public void RemoveTile(int x, int y)
    {
        if (GetTileFromWorld(x, y) && x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            TileClass tile = GetTileFromWorld(x, y);
            RemoveTileFromWorld(x, y);

            if(tile.wallVariant != null)
            {
                //if tile is naturally placed
                if(tile.naturallyPlaced)
                {
                    PlaceTile(tile.wallVariant, x, y, true);
                }
            }

            //Destroy(worldTileObjects[worldTiles.Indexof(new Vector2(x, y))]);

            //drop block as collectable
            if(tile.tileDrop)
            {
                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(x + .5f, y + .5f), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.tileDrop.tileSprites[0];
                ItemClass tileDropItem = new ItemClass(tile.tileDrop);
                newTileDrop.GetComponent<DroppedTileControl>().item = tileDropItem;
            }

            if (!GetTileFromWorld(x, y))
            {
                worldTilesMap.SetPixel(x, y, Color.white);
                LightBlock(x, y, 1f, 0);   
                worldTilesMap.Apply();
            }
            

            Destroy(GetObjectTileFromWorld(x, y));
            RemoveObjectFromWorld(x, y);

        }
    }

    public bool CheckTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        if(x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            if(tile.inBackdrop)
            {
                if(GetTileFromWorld(x + 1, y) || 
                   GetTileFromWorld(x - 1, y) ||
                   GetTileFromWorld(x, y + 1) ||
                   GetTileFromWorld(x, y - 1))
                {
                    if(!GetTileFromWorld(x, y))
                    {
                        RemoveLightSource(x, y);
                        PlaceTile(tile, x, y, isNaturallyPlaced);
                        return true;
                    }
                    else
                    {
                        if (!GetTileFromWorld(x, y).inBackdrop)
                        {
                            RemoveLightSource(x, y);
                            PlaceTile(tile, x, y, isNaturallyPlaced);
                            return true;
                        }
                    }
                }
            }
            else
            {
                if(GetTileFromWorld(x + 1, y) || 
                   GetTileFromWorld(x - 1, y) ||
                   GetTileFromWorld(x, y + 1) ||
                   GetTileFromWorld(x, y - 1))
                {
                    if(!GetTileFromWorld(x, y))
                    {
                        RemoveLightSource(x, y);
                        PlaceTile(tile, x, y, isNaturallyPlaced);
                        return true;
                    }
                    else
                    {
                        if (GetTileFromWorld(x, y).inBackdrop)
                        {
                            RemoveLightSource(x, y);
                            PlaceTile(tile, x, y, isNaturallyPlaced);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public void PlaceTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        if(x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            GameObject newTile = new GameObject();
            int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;
  
            newTile.transform.parent = worldChunks[chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();

            int spriteIndex = Random.Range(0, tile.tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tile.tileSprites[spriteIndex];

            worldTilesMap.SetPixel(x, y, Color.black);
            if(tile.inBackdrop)
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;

                if(tile.name.ToLower().Contains("backdrop"))
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
                }
                else
                {
                    worldTilesMap.SetPixel(x, y, Color.white);
                }
            }
            else
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.tag = "Ground";
            }              

            newTile.name =  tile.tileSprites[0].name;
            newTile.transform.position = new Vector2(x + .5f, y + .5f);

            TileClass newTileClass = TileClass.CreateInstance(tile, isNaturallyPlaced);

            AddObjectToWorld(x, y, newTile, newTileClass);
            AddTileToWorld(x, y, newTileClass);
        }
    }

    void AddTileToWorld(int x, int y, TileClass tile)
    {
        if(tile.inBackdrop)
        {
            world_BackdropTiles[x, y] = tile;
        }
        else
        {
            world_SceneTiles[x, y] = tile;
        }
    }

    void AddObjectToWorld(int x, int y, GameObject tileObject, TileClass tile)
    {
        if(tile.inBackdrop)
        {
            world_BackdropObjects[x, y] = tileObject;
        }
        else
        {
            world_SceneObjects[x, y] = tileObject;
        }
    }

    void RemoveTileFromWorld(int x, int y)
    {
        if(world_SceneTiles[x, y] != null)
        {
            world_SceneTiles[x, y] = null;
        }
        else if(world_BackdropTiles[x, y] != null)
        {
            world_BackdropTiles[x, y] = null;
        }
    }

    void RemoveObjectFromWorld(int x, int y)
    {
        if(world_SceneObjects[x, y] != null)
        {
            world_SceneObjects[x, y] = null;
        }
        else if(world_BackdropObjects[x, y] != null)
        {
            world_BackdropObjects[x, y] = null;
        }
    }

    GameObject GetObjectTileFromWorld(int x, int y)
    {
        if(world_SceneObjects[x, y] != null)
        {
            return world_SceneObjects[x, y];
        }
        else if(world_BackdropObjects[x, y] != null)
        {
            return world_BackdropObjects[x, y];
        }

        return null;
    }

    TileClass GetTileFromWorld(int x, int y)
    {
        if(world_SceneTiles[x, y] != null)
        {
            return world_SceneTiles[x, y];
        }
        else if(world_BackdropTiles[x, y] != null)
        {
            return world_BackdropTiles[x, y];
        }

        return null;
    }

    void LightBlock(int x, int y, float intensity, int iteration)
    {
        if(iteration < lightRadius)
        {
            worldTilesMap.SetPixel(x, y, Color.white * intensity);

            float thresh = groundLightThreshold;
            if(x >= 0 && x < worldSize && y >= 0 && y < worldSize)
            {
                if (world_SceneTiles[x, y])
                    thresh = groundLightThreshold;
                else
                    thresh = airLightThreshold;
            }

            for (int nx = x - 1; nx < x + 2; nx++)
            {
                for (int ny = y - 1; ny < y + 2; ny++)
                {
                    if (nx != x || ny != y)
                    {
                        
                        if (worldTilesMap.GetPixel(nx, ny) != null)
                        {
                            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(nx, ny));
                            float targetIntensity = Mathf.Pow(thresh, dist) * intensity;

                            if (worldTilesMap.GetPixel(nx, ny).r < targetIntensity)
                            {
                                LightBlock(nx, ny, targetIntensity, iteration + 1);
                            }
                        }
                    }
                }
            }

            worldTilesMap.Apply();
        }
    }

    void RemoveLightSource(int x, int y)
    {
        unlitBlocks.Clear();
        UnLightBlock(x, y, x, y);

        List<Vector2Int> toRelight = new List<Vector2Int>();
        foreach (Vector2Int block in unlitBlocks)
        {
            for (int nx = block.x - 1; nx < block.x + 2; nx++)
            {
                for (int ny = block.y - 1; ny < block.y + 2; ny++)
                {
                    if (worldTilesMap.GetPixel(nx, ny) != null)
                    {
                        if (worldTilesMap.GetPixel(nx, ny).r >worldTilesMap.GetPixel(block.x, block.y).r)
                        {
                            if (!toRelight.Contains(new Vector2Int(nx, ny)))
                                toRelight.Add(new Vector2Int(nx, ny));
                        }
                    }
                }
            }
        }

        foreach(Vector2Int source in toRelight)
        {
            LightBlock(source.x, source.y, worldTilesMap.GetPixel(source.x, source.y).r, 0);
        }

        worldTilesMap.Apply();
    }

    void UnLightBlock(int x, int y, int ix, int iy)
    {
        if(Mathf.Abs(x - ix) >= lightRadius || Mathf.Abs(y - iy) >= lightRadius || unlitBlocks.Contains(new Vector2Int(x, y)))
            return;

        for (int nx = x - 1; nx < x + 2; nx++)
        {
            for (int ny = y - 1; ny < y + 2; ny++)
            {
                if (nx != x || ny != y)
                {
                    if (worldTilesMap.GetPixel(nx, ny) != null)
                    {
                        if (worldTilesMap.GetPixel(nx, ny).r < worldTilesMap.GetPixel(x, y).r)
                        {
                            UnLightBlock(nx, ny, ix, iy);
                        }
                    }
                }
            }
        }

        worldTilesMap.Apply();
        unlitBlocks.Add(new Vector2Int(x, y));
    }
}