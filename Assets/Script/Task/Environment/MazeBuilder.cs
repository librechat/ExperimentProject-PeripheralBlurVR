using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MazeBuilder : BaseEnvBuilder {

    [Header("Load Level Reference")]
    public LevelData.LevelEnum loadLevel;
    [SerializeField]
    private List<LevelData> loadLevelList;

    [Header("Generate Reference")]

    [SerializeField]
    bool AutoGenerate = false;
    [SerializeField]
    string saveFileName;

    [SerializeField]
    Vector2i mapsize;
    [SerializeField]
    Vector2i entryIndex;
    [SerializeField]
    Transform wallPrefab;

    private float entryRotation = 0.0f;

    public override void Init(Transform playerController)
    {
        CollectTaskPosList = new List<Vector3>();
        SpatialInfoList = new List<SpatialTaskData>();
        Vector3 initialPos = playerController.position;

        // build maze by map
        if (AutoGenerate) GenerateMaze();
        
        MazeInfo mazeInfo = ReadFromJson();

        float scale = 2.0f;

        List<WallPos> wallPosList = new List<WallPos>(mazeInfo.wallPos);        
        for (int i = 0; i < wallPosList.Count; i++)
        {
            Vector3 position = new Vector3(wallPosList[i].x, 0, wallPosList[i].y) * scale;
            Quaternion rotation = (wallPosList[i].direction) ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
            Instantiate(wallPrefab, position, rotation);
        }

        List<Vector2i> collectTaskPosList = new List<Vector2i>(mazeInfo.collectTaskPos);
        for (int i = 0; i < collectTaskPosList.Count; i++)
        {
            CollectTaskPosList.Add(new Vector3(collectTaskPosList[i].x * scale, initialPos.y, collectTaskPosList[i].y * scale));
        }

        List<SpatialTaskInfo> spatialInfoList = new List<SpatialTaskInfo>(mazeInfo.spatialTaskInfo);
        for (int i = 0; i < spatialInfoList.Count; i++)
        {
            SpatialTaskData data = new SpatialTaskData();
            data.startPos = new Vector3(spatialInfoList[i].startPos.x * scale, initialPos.y, spatialInfoList[i].startPos.y * scale);
            data.endPos = new Vector3(spatialInfoList[i].endPos.x * scale, initialPos.y, spatialInfoList[i].endPos.y * scale);

            SpatialInfoList.Add(data);
        }

        ExperimentManager.VRRig.position = new Vector3(entryIndex.x * scale, 0, entryIndex.y * scale);
        ExperimentManager.VRRig.Rotate(new Vector3(0, entryRotation, 0));

        return;
    }


    //=============== Build Maze ==============
    [System.Serializable]
    public class Vector2i {
        public int x;
        public int y;

        public Vector2i(int i, int j){
            x = i;
            y = j;
        }

        public static Vector2i operator+ (Vector2i A, Vector2i B){
            return new Vector2i(A.x+B.x, A.y+B.y);            
        }
    };

    class Grid
    {
        public bool visited;
        public bool frontier;

        public Vector2i pos;
        public List<Wall> wallList;

        public Grid(Vector2i p)
        {
            visited = false;
            frontier = false;
            pos = p;
            wallList = new List<Wall>();
            for (int i = 0; i < 4; i++) wallList.Add(null);
        }
    };
    class Wall
    {
        public Grid A;
        public Grid B;
        
        public bool exist;

        public WallPos pos;
        public bool direction; // false = 0 degree, true = 90 degree


        public Wall(Grid a, Grid b, bool d)
        {
            A = a;
            B = b;
            exist = true;
            direction = d;

            Vector2i sum = A.pos + B.pos;
            pos = new WallPos((float)sum.x / 2, (float)sum.y / 2, direction);
        }
    };

    List<Vector2i> offset = new List<Vector2i>(){
	    new Vector2i(1,0),
	    new Vector2i(0,1),
	    new Vector2i(-1,0),
	    new Vector2i(0,-1)
    };

    bool LegalPos(Vector2i pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= mapsize.x || pos.y >= mapsize.y) return false;
        else return true;
    }

    void GenerateMaze()
    {
        Debug.Log("Generate");
        List<List<Grid>> gridList = new List<List<Grid>>();
        for (int i = 0; i < mapsize.x; i++)
        {
            gridList.Add(new List<Grid>());
            for (int j = 0; j < mapsize.y; j++)
            {
                gridList[i].Add(new Grid(new Vector2i(i, j)));
            }
        }

        List<Wall> wallList = new List<Wall>();
        for (int i = 0; i < mapsize.x; i++)
        {
            gridList.Add(new List<Grid>());
            for (int j = 0; j < mapsize.y; j++)
            {

                for (int k = 0; k < 4; k++)
                {
                    Vector2i neighborIndex = gridList[i][j].pos + offset[k];
                    if (!LegalPos(neighborIndex))
                    {
                        Wall wall = new Wall(new Grid(neighborIndex), gridList[i][j], k % 2 == 0); // edge wall
                        gridList[i][j].wallList[k] = wall;
                        wallList.Add(wall);
                    }
                    else
                    {
                        Grid neighbor = gridList[neighborIndex.x][neighborIndex.y];
                        if (neighbor.wallList[(k + 2) % 4] == null)
                        {
                            Wall wall = new Wall(neighbor, gridList[i][j], k % 2 == 0);
                            neighbor.wallList[(k + 2) % 4] = wall;
                            gridList[i][j].wallList[k] = wall;
                            wallList.Add(wall);
                        }
                        else
                        {
                            Wall wall = neighbor.wallList[(k + 2) % 4];
                            gridList[i][j].wallList[k] = wall;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < mapsize.x; i++)
        {
            gridList.Add(new List<Grid>());
            for (int j = 0; j < mapsize.y; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (gridList[i][j].wallList[k] == null) Debug.Log(i + " " + j + " " + k);
                }
            }
        }

        Grid entry = gridList[entryIndex.x][entryIndex.y];
        entry.visited = true;
        entry.frontier = true;

        List<Vector2i> candidateIndexList = new List<Vector2i>();

        for (int i = 0; i < 4; i++)
        {
            Vector2i neighborIndex = entry.pos + offset[i];
            if (!LegalPos(neighborIndex)) continue;

            Grid neighbor = gridList[neighborIndex.x][neighborIndex.y];
            if (!neighbor.visited) candidateIndexList.Add(neighbor.pos);
        }

        while (candidateIndexList.Count != 0)
        {
            int pickIndex = Random.Range(0, candidateIndexList.Count);
            Vector2i chosenIndex = candidateIndexList[pickIndex];
            Grid chosen = gridList[chosenIndex.x][chosenIndex.y];
            chosen.visited = true;

            candidateIndexList.RemoveAt(pickIndex);

            List<int> visitedOffset = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                // randomly choose a visited neighbor of chosen, break the wall between them (add to not-wall list?)
                // add to candidate if neighbor of chosen is legalpos and !visited
                Vector2i neighborIndex = chosen.pos + offset[i];
                if (!LegalPos(neighborIndex)) continue;

                Grid neighbor = gridList[neighborIndex.x][neighborIndex.y];
                if (!neighbor.visited)
                {
                    if (!neighbor.frontier)
                    {
                        candidateIndexList.Add(neighbor.pos);
                        neighbor.frontier = true;
                    }
                }
                else if (chosen.wallList[i].exist) visitedOffset.Add(i);
            }

            if(visitedOffset.Count != 0){
                int offsetIndex = visitedOffset[Random.Range(0, visitedOffset.Count)];
                Vector2i inMazeNeighborIndex = chosen.pos + offset[offsetIndex];
                Grid inMazeNeighbor = gridList[inMazeNeighborIndex.x][inMazeNeighborIndex.y];

                // no wall between inMazeNeighbor and chosen
                Wall wall = chosen.wallList[offsetIndex];
                wall.exist = false;
            }
        }

        // build walls (physical position)
        List<WallPos> wallPosList = new List<WallPos>();
        for (int i = 0; i < wallList.Count; i++)
        {
            if (wallList[i].exist)
            {
                wallPosList.Add(new WallPos(wallList[i].pos.x, wallList[i].pos.y, wallList[i].direction));
            }
        }

        // generate collect position
        int total = mapsize.x * mapsize.y;
        List<int> gridIndex = new List<int>();
        for (int i = 0; i < total; i++) gridIndex.Add(i);

        List<Vector2i> collectPosList = new List<Vector2i>();
        for (int i = 0; i < CollectTaskManager.NumOfTask; i++)
        {
            int n = Random.Range(0, gridIndex.Count);
            int index = gridIndex[n];

            collectPosList.Add(new Vector2i(index / mapsize.y, index % mapsize.y));

            gridIndex.RemoveAt(n);
        }

        // generate spatial task position
        gridIndex = new List<int>();
        for (int i = 0; i < total; i++) gridIndex.Add(i);

        List<SpatialTaskInfo> spatialInfoList = new List<SpatialTaskInfo>();
        for (int i = 0; i < SpatialTaskManager.NumOfTask; i++)
        {
            int n = Random.Range(0, gridIndex.Count);
            int index = gridIndex[n];

            SpatialTaskInfo info = new SpatialTaskInfo();
            info.startPos = new Vector2i(index / mapsize.y, index % mapsize.y);

            int dx = Random.Range(-1, 2);
            int dy = Random.Range(-1, 2);
            
            while((dx==0&&dy==0) || !(info.startPos.x + dx >= 0 && info.startPos.x + dx < mapsize.x && info.startPos.y + dy >= 0 && info.startPos.y + dy < mapsize.y))
            {
                dx = Random.Range(-1, 2);
                dy = Random.Range(-1, 2);
            }

            info.endPos = info.startPos + new Vector2i(dx, dy);

            spatialInfoList.Add(info);

            gridIndex.RemoveAt(n);
        }


        // write to json file
        PrintToJson(wallPosList, collectPosList, spatialInfoList);
    }

    // ====================== JSON =============================

    [System.Serializable]
    public class MazeInfo{
        public WallPos[] wallPos;
        public Vector2i[] collectTaskPos;
        public SpatialTaskInfo[] spatialTaskInfo;
    }
    [System.Serializable]
    public class WallPos{
        public float x;
        public float y;
        public bool direction;

        public WallPos(float a, float b, bool d){
            x = a; 
            y = b;
            direction = d;
        }
    };
    [System.Serializable]
    public class SpatialTaskInfo
    {
        public Vector2i startPos;
        public Vector2i endPos;
    }

    void PrintToJson(List<WallPos> wallPosList, List<Vector2i> collectPosList, List<SpatialTaskInfo> spatialInfoList)
    {
        MazeInfo WL = new MazeInfo();
        WL.wallPos = wallPosList.ToArray();
        WL.collectTaskPos = collectPosList.ToArray();
        WL.spatialTaskInfo = spatialInfoList.ToArray();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + saveFileName + ".json");
        string dataAsJson = JsonUtility.ToJson(WL, true);
        File.WriteAllText(filePath, dataAsJson);
    }

    MazeInfo ReadFromJson()
    {
        LevelData levelData = loadLevelList[(int)loadLevel];

        mapsize = levelData.mapSize;
        entryIndex = levelData.entryPos;
        entryRotation = levelData.entryRotation;
        
        string filePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + levelData.fileName + ".json");
        string dataAsJson = File.ReadAllText(filePath);
        MazeInfo WL = JsonUtility.FromJson<MazeInfo>(dataAsJson);

        // RevertPath(WL, levelData.fileName);
        // StartPathFrom(50, 9, WL, levelData.fileName);

        return WL;
    }

    void RevertPath(MazeInfo input, string originFileName)
    {
        MazeInfo WL = new MazeInfo();
        WL.wallPos = input.wallPos;

        List<Vector2i> collectPosList = new List<Vector2i>(input.collectTaskPos);
        collectPosList.Reverse();
        WL.collectTaskPos = collectPosList.ToArray();

        List<SpatialTaskInfo> spatialTaskList = new List<SpatialTaskInfo>(input.spatialTaskInfo);
        spatialTaskList.Reverse();
        for (int i = 0; i < spatialTaskList.Count; i++)
        {
            Vector2i temp = spatialTaskList[i].startPos;
            spatialTaskList[i].startPos = spatialTaskList[i].endPos;
            spatialTaskList[i].endPos = temp;
        }
        WL.spatialTaskInfo = spatialTaskList.ToArray();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + originFileName + "_Revert" + ".json");
        string dataAsJson = JsonUtility.ToJson(WL, true);
        File.WriteAllText(filePath, dataAsJson);
    }
    void StartPathFrom(int firstCollectIndex, int firstSpatialIndex, MazeInfo input, string originFileName)
    {
        MazeInfo WL = new MazeInfo();
        WL.wallPos = input.wallPos;

        List<Vector2i> collectPosList = new List<Vector2i>(input.collectTaskPos);
        List<Vector2i> new_collectPosList = collectPosList.GetRange(firstCollectIndex, collectPosList.Count - firstCollectIndex);
        new_collectPosList.AddRange(collectPosList.GetRange(0, firstCollectIndex));
        WL.collectTaskPos = new_collectPosList.ToArray();

        List<SpatialTaskInfo> spatialTaskList = new List<SpatialTaskInfo>(input.spatialTaskInfo);
        List<SpatialTaskInfo> new_spatialTaskList = spatialTaskList.GetRange(firstSpatialIndex, spatialTaskList.Count- firstSpatialIndex);
        new_spatialTaskList.AddRange(spatialTaskList.GetRange(0, firstSpatialIndex));
        WL.spatialTaskInfo = new_spatialTaskList.ToArray();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + originFileName + "_StartAt" + firstCollectIndex + ".json");
        string dataAsJson = JsonUtility.ToJson(WL, true);
        File.WriteAllText(filePath, dataAsJson);
    }
}
