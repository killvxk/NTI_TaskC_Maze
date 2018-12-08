using UnityEngine;

public class MazeSpawner : MonoBehaviour {
    public enum MazeGenerationAlgorithm {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }
    
    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Wall = null;
    public GameObject Pillar = null;
    public int Rows = 5;
    public int Columns = 5;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject GoalPrefab = null;

    private const int GoalLimit = 1;
    private BasicMazeGenerator mMazeGenerator = null;
    //private NavMesh

    private void Start () {
        if (!FullRandom) {
            Random.InitState(RandomSeed);
        }
        switch (Algorithm) {
        case MazeGenerationAlgorithm.PureRecursive:
            mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
            break;
        case MazeGenerationAlgorithm.RecursiveTree:
            mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
            break;
        case MazeGenerationAlgorithm.RandomTree:
            mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
            break;
        case MazeGenerationAlgorithm.OldestTree:
            mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
            break;
        case MazeGenerationAlgorithm.RecursiveDivision:
            mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
            break;
        }
        var spawnedGoals = 0;
        mMazeGenerator.GenerateMaze();
        for (int row = 0; row < Rows; ++row) {
            for (int column = 0; column < Columns; ++column){
                var x = column * (CellWidth + (AddGaps?.2f : 0));
                var z = row * (CellHeight + (AddGaps?.2f : 0));
                var cell = mMazeGenerator.GetMazeCell(row, column);
                var tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                tmp.transform.parent = transform;
                if (cell.WallRight) {
                    tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0));
                    tmp.transform.parent = transform;
                }
                if (cell.WallFront) {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0));
                    tmp.transform.parent = transform;
                }
                if (cell.WallLeft) {
                    tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0));
                    tmp.transform.parent = transform;
                }
                if (cell.WallBack) {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0));
                    tmp.transform.parent = transform;
                }
                cell.IsGoal = cell.IsGoal && spawnedGoals < GoalLimit;
                if (cell.IsGoal && GoalPrefab != null) {
                    ++spawnedGoals;
                    tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0));
                    tmp.transform.parent = transform;
                }
            }
        }
        Debug.Assert(spawnedGoals == 1);

        if (Pillar != null) {
            for (var row = 0; row <= Rows; ++row) {
                for (int column = 0; column <= Columns; ++column) {
                    var x = column * (CellWidth + (AddGaps?.2f : 0));
                    var z = row * (CellHeight + (AddGaps?.2f : 0));
                    var tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity);
                    tmp.transform.parent = transform;
                }
            }
        }
    }
}
