using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager main;

    public int env_Width;
    public int env_Height;
    public int seed;

    [Range(0, 100)]
    public float distributingRatio;

    public float lifeSpeed;
    private float markedTime;

    private Cell[,] cellGrid;

    private GameObject environment;

    private System.Random random;

    private void Awake()
    {
        if (main == null)
        {
            DontDestroyOnLoad(gameObject);
            main = this;
        }
        else if (main != null)
        {
            Destroy(main);
        }

        cellGrid = new Cell[env_Width, env_Height];
        environment = GameObject.CreatePrimitive(PrimitiveType.Plane);
        environment.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Background");
        environment.transform.position = new Vector3(env_Width / 2 - 0.5f, 0.5f, env_Height / 2 - 0.5f);
        environment.transform.localScale = new Vector3(env_Width / 10, 1, env_Height / 10);
    }

    private void Start()
    {
        random = new System.Random(seed);

        GenerateCell();

        isSimulating = false;

        markedTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= markedTime + lifeSpeed && isSimulating)
        {
            IterateCellGrid();
            markedTime = Time.time;
        }

        EditCell();
    }

    #region Main Program
    private void GenerateCell()
    {
        // Clear previous cell
        foreach (Transform child in environment.transform)
        {
            Destroy(child.gameObject);
        }

        // Generate new cells
        for (int y = 0; y < cellGrid.GetLength(1); y++)
        {
            for (int x = 0; x < cellGrid.GetLength(0); x++)
            {
                cellGrid[x, y] = new Cell(x, y, RandomizeBoolean(), environment.transform);
            }
        }
    }

    private bool RandomizeBoolean()
    {
        bool result = random.NextDouble() > 1 - (float)(distributingRatio / 100);

        return result;
    }

    private void IterateCellGrid()
    {
        for (int y = 0; y < cellGrid.GetLength(1); y++)
        {
            for (int x = 0; x < cellGrid.GetLength(0); x++)
            {
                CountSurroundingNeiboursOf(cellGrid[x, y]);
            }
        }

        for (int y = 0; y < cellGrid.GetLength(1); y++)
        {
            for (int x = 0; x < cellGrid.GetLength(0); x++)
            {
                Iterate(cellGrid[x, y]);
            }
        }
    }

    private void CountSurroundingNeiboursOf(Cell cell)
    {
        // Reset count
        cell.NeighbourCount = 0;

        // Check 3x3 neighborhood and exclude itself
        for (int y = cell.Y - 1; y < cell.Y + 2; y++)
        {
            for (int x = cell.X - 1; x < cell.X + 2; x++)
            {
                if (cellGrid[Validate(x, y)[0], Validate(x, y)[1]].IsAlive && cellGrid[Validate(x, y)[0], Validate(x, y)[1]] != cell)
                {
                    cell.NeighbourCount++;
                }
            }
        }
    }

    private int[] Validate(int x, int y)
    {
        int[] validatedValues = new int[2];

        if (x < 0)
        {
            validatedValues[0] = cellGrid.GetLength(0) - 1;
        }
        else if (x > cellGrid.GetLength(0) - 1)
        {
            validatedValues[0] = 0;
        }
        else
        {
            validatedValues[0] = x;
        }

        if (y < 0)
        {
            validatedValues[1] = cellGrid.GetLength(1) - 1;
        }
        else if (y > cellGrid.GetLength(1) - 1)
        {
            validatedValues[1] = 0;
        }
        else
        {
            validatedValues[1] = y;
        }

        return validatedValues;
    }

    private void Iterate(Cell cell)
    {
        if
            // Rule #1: alive cell survives if it has 2..3 alive neighbours
            (cell.IsAlive && (cell.NeighbourCount < 2 || cell.NeighbourCount > 3)

            // Or
            ||

            // Rule #2: dead cell revives if it  has 3 alive neighbours
            (!cell.IsAlive && cell.NeighbourCount == 3))
        {
            cell.SelfSwitchLivingState();
        }
    }
    #endregion

    #region UI Control

    private bool isSimulating;

    public bool SwitchSimulation()
    {
        isSimulating = isSimulating ? false : true;

        return isSimulating;
    }

    public void StopSimulation()
    {
        isSimulating = false;
    }

    public void RandomizeSeed()
    {
        seed = UnityEngine.Random.Range(0, int.MaxValue);
        random = new System.Random(seed);
        GenerateCell();
    }

    public void AdjustFillRatio(float value)
    {
        distributingRatio = value;
    }

    public void AdjustSimulationSpeed(float value)
    {
        lifeSpeed = value;
    }

    private void EditCell()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        { // if left button pressed...
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want
                int newX = (int)hit.transform.position.x;
                int newY = (int)hit.transform.position.z;

                if (newX < cellGrid.GetLength(0) && newY < cellGrid.GetLength(1))
                {
                    cellGrid[newX, newY].SelfSwitchLivingState();
                    UnityEngine.UI.Text button = GameObject.Find("HintText").GetComponent<UnityEngine.UI.Text>();
                    button.text = "Click on any cell to edit its state |  " + newX + " " + newY;
                }
            }
        }
    }
    #endregion
}
