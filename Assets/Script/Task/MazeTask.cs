using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MazeTask : Task {

    [Header("Build Maze Reference")]

    [SerializeField]
    bool AutoGenerate = false;
    [SerializeField]
    string fileName;

    [SerializeField]
    Vector2i mapsize;
    [SerializeField]
    Vector2i entryIndex;
    [SerializeField]
    Transform wallPrefab;

    public override List<TaskTarget> Init(Transform playerController)
    {
        List<TaskTarget> targetList = new List<TaskTarget>();
        Vector3 initialPos = playerController.position;

        // build maze by map
        if (AutoGenerate) GenerateMaze();
        
        MazeInfo mazeInfo = ReadFromJson();

        List<WallPos> wallPosList = new List<WallPos>(mazeInfo.wallPos);        
        for (int i = 0; i < wallPosList.Count; i++)
        {
            Vector3 position = new Vector3(wallPosList[i].x, 0, wallPosList[i].y);
            Quaternion rotation = (wallPosList[i].direction) ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
            Instantiate(wallPrefab, position, rotation);
        }

        List<Vector2i> targetPosList = new List<Vector2i>(mazeInfo.targetPos);
        for (int i = 0; i < targetPosList.Count; i++)
        {
            GameObject gm = Instantiate(targetPrefab, new Vector3(targetPosList[i].x, initialPos.y, targetPosList[i].y), Quaternion.identity);

            targetList.Add(gm.GetComponent<TaskTarget>());
            targetList[i].TargetIndex = i;
        }

        return targetList;
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

        // generate target position
        int total = mapsize.x * mapsize.y;
        List<int> gridIndex = new List<int>();
        for (int i = 0; i < total; i++) gridIndex.Add(i);

        List<Vector2i> targetPosList = new List<Vector2i>();
        for (int i = 0; i < NumOfTargets; i++)
        {
            int n = Random.Range(0, gridIndex.Count);
            int index = gridIndex[n];

            targetPosList.Add(new Vector2i(index / mapsize.y, index % mapsize.y));

            gridIndex.RemoveAt(n);
        }

        // write to json file
        PrintToJson(wallPosList, targetPosList);
    }

    // ====================== JSON =============================

    [System.Serializable]
    public class MazeInfo{
        public WallPos[] wallPos;
        public Vector2i[] targetPos;
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

    void PrintToJson(List<WallPos> wallPosList, List<Vector2i> targetPosList)
    {
        /*
        {
            "wall": [
                {
                    "x": 123,
                    "y": 123
                }
            ]
        }
        */


        MazeInfo WL = new MazeInfo();
        WL.wallPos = wallPosList.ToArray();
        WL.targetPos = targetPosList.ToArray();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + fileName + ".json");
        string dataAsJson = JsonUtility.ToJson(WL, true);
        File.WriteAllText(filePath, dataAsJson);
    }

    MazeInfo ReadFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + fileName + ".json");
        string dataAsJson = File.ReadAllText(filePath);
        MazeInfo WL = JsonUtility.FromJson<MazeInfo>(dataAsJson);
        return WL;
    }
}
