//using System;
//using System.Collections.Generic;
//using System.Collections;
//using SettlersEngine;
//using Object = UnityEngine.Object;

//public class IsoGrid : IPathNode<Object>
//{
//    public int X { get; set; }
//    public int Y { get; set; }
	
//    public int Cost { get; set; }
//    public bool Walkable {get;set;}

//    private TileEntity entity;
	
//    //interface
//    public bool IsWalkable(Object unused)
//    {
//        return Walkable;
//    }

//    public List<TileEntity> entities = new List<TileEntity>();
//    public Action onAddEntity;

//    public void AddEntity(TileEntity entity)
//    {
//        this.entities.Add(entity);
//        if (this.onAddEntity != null)
//        {
//            this.onAddEntity();
//        }
//    }
//    public void RemoveEntity(TileEntity entity)
//    {
//        this.entities.Remove(entity);
//    }

//    public TileEntity GetBuildingEntity()
//    {
//        if (this.entities != null)
//        {
//            foreach (TileEntity entity in this.entities)
//            {
//                if ((entity != null) && (EntityTypeUtil.IsAnyBuilding(entity.entityType)))
//                {
//                    return entity;
//                }
//            }
//        }
//        return null;
//    }

//    public TileEntity GetWallEntity()
//    {
//        if (this.entities != null)
//        {
//            foreach (TileEntity entity in this.entities)
//            {
//                if ((entity != null) && (entity.entityType == EntityType.Wall))
//                {
//                    return entity;
//                }
//            }
//        }
//        return null;
//    }

//    public List<TileEntity> GetEntities()
//    {
//        return this.entities;
//    }

//    public TileEntity GetAnyBlockageEntity()
//    {
//        if (this.entities != null)
//        {
//            foreach (TileEntity entity in this.entities)
//            {
//                if ((entity != null) && (EntityTypeUtil.IsAnyBlockage(entity.entityType)))
//                {
//                    return entity;
//                }
//            }
//        }
//        return null;
//    }
//}