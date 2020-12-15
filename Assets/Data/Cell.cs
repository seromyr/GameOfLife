using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private bool _currentLivingState;
    private bool _previousLivingState;
    private string name;

    private int _x;
    private int _y;
    private int neighbourCount;
    private GameObject form;
    private MeshRenderer meshRenderer;

    public int NeighbourCount { get { return neighbourCount; } set { neighbourCount = value; } }
    public bool IsAlive { get { return _currentLivingState; } set { _currentLivingState = value; } }
    public int X { get { return _x; } }
    public int Y { get { return _y; } }

    public Cell(int x, int y, bool initialLivingStatus, Transform parent = null)
    {
        form = GameObject.CreatePrimitive(PrimitiveType.Cube);
        form.name = name = x + " " + y;
        form.transform.parent = parent;
        form.TryGetComponent(out meshRenderer);

        _x = x;
        _y = y;

        SetCellPosition(x, 1, y);

        _previousLivingState = _currentLivingState = initialLivingStatus;
        //form.SetActive(_currentLivingState);
        meshRenderer.material = Resources.Load<Material>("Materials/Cell"); ;
        meshRenderer.enabled = _currentLivingState;

        form.AddComponent<MouseOver>();
    }

    private void SetCellPosition(float x, float y, float z)
    {
        form.transform.position = new Vector3(x, y, z);
    }

    public void SelfSwitchLivingState()
    {
        _currentLivingState = _currentLivingState ? false : true;
        _previousLivingState = !_currentLivingState;
        SetActiveSelf();
    }

    private void SetActiveSelf()
    {
        if (_previousLivingState != _currentLivingState)
        {
            //form.SetActive(_currentLivingState);
            meshRenderer.enabled = _currentLivingState;
        }
    }

    public void UpdateName(string newName)
    {
        form.name = name + " " + newName;
    }
}