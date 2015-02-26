using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.Simple;
using UnityEngine;
using System.Collections;

public class IsoWorldModeBuilder : IsoWorldMode{
	private TileEntity currentBuilding;     ///<    当前操作中的建筑

    private BuildableView buildableView;
    private BuildArrowView buildArrow;
    private BuildConfirmView buildConfirm;

    private bool isConstructMulti;          ///<    是否连续建造（墙等）

    private int oldX;
    private int oldY;

    private TilePoint? lastPoint;
    
    private int dragDeltaX;
    private int dragDeltaY;
    private bool isDragging = false;
    private bool isNewBuild = false;
    private bool draged = false;            //是否拖拽了建筑

    private bool isPanned = false;

    public TileEntity CurrentSelectedBuilding
    {
        get
        {
            return currentBuilding;
        }
        set
        {
            currentBuilding = value;
        }
    }

    /// <summary>
    /// 建造 确认 or 取消
    /// </summary>
    /// <param name="confirm"></param>
    private void OnBuildConfirm(bool confirm)
    {
        if (confirm)
        {
            if (!IsoMap.Instance.CanPlaceBuilding(currentBuilding.GetTilePos().x, currentBuilding.GetTilePos().y,
                currentBuilding.width, currentBuilding.height))
            {
                return;//TODO alert sound
            }
            //send to server
            GameManager.Instance.RequestBuyBuilding(currentBuilding);
            if (isConstructMulti)
            {
                TilePoint? suggestPoint = null;
                if (lastPoint != null)
                {
                    var delta = currentBuilding.GetTilePos() - lastPoint.Value;
                    if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == currentBuilding.width) //只有在挨着的时候才处理
                    {
                        var newPoint = currentBuilding.GetTilePos() + delta;
                        if (IsoMap.Instance.CanPlaceBuilding(newPoint.x, newPoint.y, currentBuilding.width,
                            currentBuilding.height))
                        {
                            suggestPoint = newPoint;
                        }
                    }
                }
                if (suggestPoint == null && currentBuilding.model.entityType == EntityType.Wall)
                {
                    //判断当前建筑四周是否有墙
                    var currPoint = currentBuilding.GetTilePos();
                    if (IsoMap.Instance.IsWallStrict(currPoint.x - 2, currPoint.y))
                    {
                        suggestPoint = SuggestPosition(currPoint + new TilePoint(2, 0));
                    }
                    else if (IsoMap.Instance.IsWallStrict(currPoint.x + 2, currPoint.y))
                    {
                        suggestPoint = SuggestPosition(currPoint + new TilePoint(-2, 0));
                    }
                    else if (IsoMap.Instance.IsWallStrict(currPoint.x, currPoint.y - 2))
                    {
                        suggestPoint = SuggestPosition(currPoint + new TilePoint(0, 2));
                    }
                    else if (IsoMap.Instance.IsWallStrict(currPoint.x, currPoint.y + 2))
                    {
                        suggestPoint = SuggestPosition(currPoint + new TilePoint(0, -2));
                    }
                }
                if (suggestPoint == null)
                {
                    suggestPoint = SuggestPosition(currentBuilding.GetTilePos());
                }
                lastPoint = currentBuilding.GetTilePos();
                var tileEntity = TileEntity.Create(OwnerType.Defender, currentBuilding.model);
                tileEntity.Init();
                BuildCompleteCleanUp();//清除上一次的建造
                SetBuildingObject(tileEntity, isConstructMulti, suggestPoint);
            }
            else
            {
                lastPoint = currentBuilding.GetTilePos();
                BuildCompleteCleanUp();
            }
        }
        else
        {
            currentBuilding.Destroy();
            BuildCompleteCleanUp();
        }
    }

    /// <summary>
    /// 开始建造
    /// </summary>
    /// <param name="entity">将要建造的物体</param>
    public void SetBuildingObject(TileEntity entity, bool isConstructMulti = false, TilePoint? suggestPosition = null)
	{
        //如果当前选中了某个建筑
        if (currentBuilding != null)
        {
            UnselectBuilding(currentBuilding);
        }

        isNewBuild = true;
        this.isConstructMulti = isConstructMulti;

        buildableView.Init(entity.width);
        buildArrow.Init(entity.width);
        buildArrow.gameObject.SetActive(true);
        buildConfirm.Init(entity.width);
        buildConfirm.gameObject.SetActive(true);
        this.currentBuilding = entity;
        if (suggestPosition != null)
        {
            DraggingBuildingTo(suggestPosition.Value.x, suggestPosition.Value.y);
        }
        else
        {
            var point = SuggestPosition();
            DraggingBuildingTo(point.x, point.y);
        }
	}


    override protected void OnEnter()
	{
        base.OnEnter();

        buildableView =
            ((GameObject)ResourceManager.Instance.LoadAndCreate("Homeland/BuilderUI/BuildableView"))
                .GetComponent<BuildableView>();
        buildArrow =
            ((GameObject)ResourceManager.Instance.LoadAndCreate("Homeland/BuilderUI/BuildArrow"))
                .GetComponent<BuildArrowView>();
        buildConfirm =
            ((GameObject)ResourceManager.Instance.LoadAndCreate("Homeland/BuilderUI/BuildConfirm"))
                .GetComponent<BuildConfirmView>();

        buildableView.gameObject.SetActive(false);
        buildArrow.gameObject.SetActive(false);
        buildConfirm.gameObject.SetActive(false);

        buildConfirm.OnConfirmEvent += OnBuildConfirm;
	}
	override protected void OnExit()
	{
        buildConfirm.OnConfirmEvent -= OnBuildConfirm;
        GameObject.Destroy(buildConfirm.gameObject);
        GameObject.Destroy(buildArrow.gameObject);
        GameObject.Destroy(buildableView.gameObject);
	}

    private void BuildCompleteCleanUp()
    {
    	isNewBuild = false;

        buildArrow.gameObject.SetActive(false);
        buildConfirm.gameObject.SetActive(false);
        buildableView.gameObject.SetActive(false);

		currentBuilding = null;
    }

    public void SelectBuilding(TileEntity building) //临时去掉占地数据，记录原始位置，添加选择状态
    {
        building.OnSelected();
        oldX = building.GetTilePos().x;
        oldY = building.GetTilePos().y;
        currentBuilding = building;
        IsoMap.Instance.BeginMoveEntity(building);
        buildableView.Init(building.width);
        buildArrow.Init(building.width);
        buildArrow.gameObject.SetActive(true);
        buildArrow.transform.position = IsoHelper.GridToPosition(building.GetTilePos());
    }

    public void UnselectBuilding(TileEntity building) //恢复或者变更占地数据，去掉选择状态
    {
        if (!IsoMap.Instance.CanPlaceBuilding(building.GetTilePos().x, building.GetTilePos().y,
            building.width, building.height))
        {
            DraggingBuildingTo(oldX, oldY);
        }
        building.OnUnselected();
        currentBuilding = null;
        IsoMap.Instance.EndMoveEntity(building);
        buildableView.gameObject.SetActive(false);
        buildArrow.gameObject.SetActive(false);
    }

    /// <summary>
    /// 移动建筑物到指定位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void DraggingBuildingTo(int x, int y)
    {
        //REMARK 一个建筑格子占2个小格子
        if (x % 2 == 1)
        {
            x -= 1;
        }
        if (y % 2 == 1)
        {
            y -= 1;
        }
        if (currentBuilding.GetTilePos().x == x && currentBuilding.GetTilePos().y == y)
            return;
        if (!draged)
        {
            draged = true;
            EventDispather.DispatherEvent(GameEvents.BEGIN_DRAG_BUILDING);
        }
        currentBuilding.SetTilePosition(new TilePoint(x, y));
        currentBuilding.view.transform.position = currentBuilding.GetRenderPosition();
        var pos = IsoHelper.GridToPosition(x, y);
        buildableView.gameObject.SetActive(true);
        buildableView.transform.position = pos;
        IsoHelper.MoveAlongCamera(buildableView.transform,5f);
        buildArrow.transform.position = pos;
//        IsoHelper.MoveAlongCamera(buildArrow.transform,-1f);
        buildConfirm.transform.position = pos;
        IsoHelper.MoveAlongCamera(buildConfirm.transform,-1f);

        //  是否可以建造
        buildableView.SetBuildable(IsoMap.Instance.CanPlaceBuilding(x, y, currentBuilding.width, currentBuilding.height));
    }

    //suggest a building position
    private TilePoint SuggestPosition(TilePoint? suggestPoint=null)
    {
        Queue<TilePoint> notVisited = new Queue<TilePoint>();
        bool[,] visitedMap = new bool[Constants.WIDTH,Constants.HEIGHT];
        visitedMap.Initialize();
        int centerX, centerY;
        if (suggestPoint == null)
        {
            if (lastPoint != null)
            {
                var viewportPoint =
                    Camera.main.WorldToViewportPoint(new Vector3(lastPoint.Value.x, 0, lastPoint.Value.y));
                Debug.Log(viewportPoint);
                if (viewportPoint.x > 0.2f && viewportPoint.x < 0.8f && viewportPoint.y > 0.2f && viewportPoint.y < 0.8f)
                {
                    suggestPoint = lastPoint.Value;
                }
            }
            if (suggestPoint == null)
            {
                var cameraCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                IsoHelper.ScreenPositionToGrid(cameraCenter, out centerX, out centerY);
                suggestPoint = new TilePoint(centerX, centerY);
            }
        }
        centerX = suggestPoint.Value.x;
        centerY = suggestPoint.Value.y;
        //REMARK 一个建筑格子占2个小格子
        if (centerX % 2 == 1)
        {
            centerX += 1;
        }
        if (centerY % 2 == 1)
        {
            centerY += 1;
        }
        int STEP = 2;

        visitedMap[centerX, centerY] = true;
        notVisited.Enqueue(new TilePoint(centerX, centerY));
        int x, y;
        while (notVisited.Count > 0)
        {
            var point = notVisited.Dequeue();
            x = point.x;
            y = point.y;
            if(IsoMap.Instance.CanPlaceBuilding(x,y,currentBuilding.width,currentBuilding.height))
                return new TilePoint(x,y);
            int dx = Mathf.Abs(x - centerX);
            int dy = Mathf.Abs(y - centerY);
            if(dx > currentBuilding.width || dy > currentBuilding.height)//建议的点不能离中心太远
                continue;
            if (x - STEP >= 0 && !visitedMap[x - STEP, y])
            {
                visitedMap[x - STEP, y] = true;
                notVisited.Enqueue(new TilePoint(x - STEP, y));
            }
            if (x + STEP < Constants.WIDTH && !visitedMap[x + STEP, y])
            {
                visitedMap[x + STEP, y] = true;
                notVisited.Enqueue(new TilePoint(x + STEP, y));
            }
            if (y - STEP >= 0 && !visitedMap[x, y - STEP])
            {
                visitedMap[x, y - STEP] = true;
                notVisited.Enqueue(new TilePoint(x, y - STEP));
            }
            if (y + STEP < Constants.HEIGHT && !visitedMap[x, y + STEP])
            {
                visitedMap[x, y + STEP] = true;
                notVisited.Enqueue(new TilePoint(x, y + STEP));
            }
        }
        //no best point,use center of screen
        return new TilePoint(centerX,centerY);
    }

    protected override void OnPress(Vector2 screenPosition)
    {
        isDragging = false;
        isPanned = false;
        if (currentBuilding != null)
        {
            CheckDragBuilding(screenPosition);
        }
        base.OnPress(screenPosition);
    }

    protected override void OnRelease()
    {
        if (draged)
        {
            draged = false;
            EventDispather.DispatherEvent(GameEvents.END_DRAG_BUILDING);

            //隐藏可建造绿地皮
            if (!isNewBuild && currentBuilding != null)
            {
                var buildable = IsoMap.Instance.CanPlaceBuilding(currentBuilding.GetTilePos().x,
                currentBuilding.GetTilePos().y, currentBuilding.width, currentBuilding.height);
                if (buildable)
                {
                    buildableView.gameObject.SetActive(false);

                    if (currentBuilding.GetTilePos() != new TilePoint(oldX, oldY))
                    {
                        currentBuilding.buildingVO.x = currentBuilding.GetTilePos().x;
                        currentBuilding.buildingVO.y = currentBuilding.GetTilePos().y;
                        GameManager.Instance.RequestMoveBuilding(currentBuilding);
                        oldX = currentBuilding.GetTilePos().x;
                        oldY = currentBuilding.GetTilePos().y;

                        IsoMap.Instance.InitGuardAreaMap();
                        IsoMap.Instance.ShowGuardAreaMap(true);
                    }
                }
            }
        }
    }

    private void CheckDragBuilding(Vector2 screenPosition)
    {
        int mx, my;
        IsoHelper.ScreenPositionToGrid(screenPosition, out mx, out my);//REMARK 忽略边界检查
        int dx = mx - currentBuilding.GetTilePos().x;
        int dy = my - currentBuilding.GetTilePos().y;
        if (dx >= 0 && dx < currentBuilding.width && dy >= 0 && dy < currentBuilding.height)
        {
            dragDeltaX = dx;
            dragDeltaY = dy;
            isDragging = true;
        }
    }

    protected override void OnLongPress(Vector2 screenPosition)
    {
        var building = IsoMap.Instance.GetBuildingAtScreenPoint(screenPosition);
        if (!isNewBuild && building != null && !isPanned)
        {
            if (currentBuilding != null)
            {
                UnselectBuilding(currentBuilding);
            }
            building.HandleOnTap();
            SelectBuilding(building);
            CheckDragBuilding(screenPosition);
        }
    }

    protected override void OnTap(Vector2 screenPosition)
    {
        var building = IsoMap.Instance.GetBuildingAtScreenPoint(screenPosition);
        if (!isNewBuild)
        {
            var prevSelected = currentBuilding;
            if (currentBuilding != null)
            {
                UnselectBuilding(currentBuilding);
            }
            if (prevSelected != building && building != null)
            {
                if (!building.HandleOnTap())
                {
                    SelectBuilding(building);
                }
            }
        }
    }

    protected override void OnPan(Vector2 screenPosition, Vector3 deltaPosition)
    {
        isPanned = true;
        if (isDragging)
        {
            int mx, my;
            IsoHelper.ScreenPositionToGrid(screenPosition, out mx, out my);//REMARK 忽略边界检查,允许多拽出边界
            DraggingBuildingTo(mx - dragDeltaX, my - dragDeltaY);
        }
        else
        {
            base.OnPan(screenPosition,deltaPosition);
        }
    }
}
