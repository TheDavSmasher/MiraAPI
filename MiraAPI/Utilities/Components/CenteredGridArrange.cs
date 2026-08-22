using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace MiraAPI.Utilities.Components;

[RegisterInIl2Cpp]
public class CenteredGridArrange(IntPtr ptr) : MonoBehaviour(ptr)
{
    public Vector2 CellSize;
    public int MaxColumns = 3;

    private List<Transform> cells;
    private static List<Transform> currentChildren = new List<Transform>();

    private void Start()
    {
        cells = new List<Transform>();
        GetChildsActive();
        CheckCurrentChildren();
    }

    private void FixedUpdate() => CheckCurrentChildren();

    private void CheckCurrentChildren()
    {
        GetChildsActive();
        if (cells.SequenceEqual(currentChildren))
            return;
        cells.Clear();
        foreach (Transform currentChild in currentChildren)
            cells.Add(currentChild);
        ArrangeChilds();
    }

    private void GetChildsActive()
    {
        currentChildren.Clear();
        foreach (var obj in transform)
        {
            var child = obj.TryCast<Transform>();
            if (child == null) continue;
            if (child.gameObject.activeSelf)
                currentChildren.Add(child);
        }
    }

    private void ArrangeChilds()
    {
        if (cells.Count == 0)
            return;

        int totalRows = Mathf.CeilToInt((float)cells.Count / MaxColumns);
        float totalHeight = (totalRows - 1) * CellSize.y;
        float startY = transform.position.y + totalHeight * 0.5f;

        for (int index = 0; index < cells.Count; ++index)
        {
            int row = index / MaxColumns;
            int rowStartIndex = row * MaxColumns;
            int itemsInRow = Mathf.Min(MaxColumns, cells.Count - rowStartIndex);
            int col = index - rowStartIndex;

            float rowWidth = (itemsInRow - 1) * CellSize.x;
            float startX = transform.position.x - rowWidth * 0.5f;

            float x = startX + col * CellSize.x;
            float y = startY - row * CellSize.y;

            Transform cell = cells[index];
            cell.position = new Vector3(x, y, cell.position.z);
        }
    }
}