using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;


public class BuildManager : MonoSingleton<BuildManager>
{
    public BuildingList buildingList;
    [SerializeField] private Material _transparentMat;

    private bool _inBuildMode = false;
    private int _selectedBuild = -1;
    private Vector2Int _selectedPos;
    private SpriteRenderer curPreview;

    private void Start()
    {
        GameManager.Instance.playerInput.Player.NumberKey.started += OnNumberInput;
        GameManager.Instance.playerInput.Player.Fire.started += OnPlaceInput;
    }

    private void OnNumberInput(InputAction.CallbackContext ctx)
    {
        int selected = (int)(ctx.ReadValue<float>() - 1f);
        if (selected == _selectedBuild && _inBuildMode) _inBuildMode = false;
        else if (!_inBuildMode)
        {
            if (selected < 0 || selected >= buildingList.builds.Count()) return;
            SelectBuild(selected);
        }
    }

    private void OnPlaceInput(InputAction.CallbackContext ctx)
    {
        if (_inBuildMode)
        {
            if (CanPlaceAt(_selectedPos) && PlayerManager.Instance.Inventory_Spend(buildingList.builds[_selectedBuild].cost))
            {
                ObstacleManager.Instance.PlaceObstacleAt(_selectedPos, buildingList.builds[_selectedBuild].obstacleId);
                PlayerManager.Instance.Inventory_CostResourceAnim(
                    GridManager.Instance.GridToCenterOfGridWorldPos(_selectedPos) + new Vector2(0, GridManager.Instance.gridConfig.gridSquareSize / 2),
                    buildingList.builds[_selectedBuild].cost
                    );
            }
            _inBuildMode = false;
        }
    }

    /// <summary>
    /// Checks if can place a building at grid position
    /// </summary>
    private bool CanPlaceAt(Vector2Int gridPos)
    {
        return gridPos != PlayerManager.Instance.PlayerGridPosition && !ObstacleManager.Instance.IsOccupied(gridPos);
    }

    private void SelectBuild(int buildId)
    {
        _inBuildMode = true;
        _selectedBuild = buildId;
        int obstacleId = buildingList.builds[buildId].obstacleId;
        GameObject preview = Instantiate(ObstacleManager.Instance.obstacleData.obstacleDict[obstacleId]).gameObject;
        foreach (Component comp in preview.GetComponents<Component>())
        {
            if (comp is Transform || comp is SpriteRenderer) continue;
            Destroy(comp);
        }
        curPreview = preview.GetComponent<SpriteRenderer>();
        if (curPreview != null) {
            curPreview.enabled = true;
            curPreview.material = _transparentMat;
        } else
        {
            throw new System.Exception("Building has no sprite renderer");
        }
        StartCoroutine(BuildMode());
    }

    private IEnumerator BuildMode()
    {
        while (_inBuildMode)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _selectedPos = GridManager.Instance.WorldToGridPos(mousePos);
            curPreview.transform.position = GridManager.Instance.GridToCenterOfGridWorldPos(_selectedPos);
            if (!PlayerManager.Instance.Inventory_CanAfford(buildingList.builds[_selectedBuild].cost)) {
                // Can't afford
                curPreview.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, 1f);
            }
            else if (!CanPlaceAt(_selectedPos))
            {
                // Can't place here
                curPreview.GetComponent<SpriteRenderer>().color = new Color(1f, 0.92f, 0.016f, 1f);
            } 
            else
            {
                // Valid
                curPreview.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            }
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(curPreview.gameObject);
    }
}
