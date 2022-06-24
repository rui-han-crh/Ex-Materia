using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using System;
using Tests;
using DataStructures;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Consultants;
using CombatSystem.Censuses;

namespace Tests
{
    public static class Warehouse 
    {
        public static UnitProperties PROPERTIES_SET_A = new UnitProperties(
            maxHealth: 100,
            currentHealth: 80,
            defence: 30,
            attack: 50,
            range: 12,
            maxActionPoints: 150,
            currentActionPoints: 77
            );

        public static UnitProperties PROPERTIES_SET_B = new UnitProperties(
            maxHealth: 100,
            currentHealth: 48,
            defence: 20,
            attack: 35,
            range: 7,
            maxActionPoints: 150,
            currentActionPoints: 41
            );

        public static UnitProperties PROPERTIES_SET_C = new UnitProperties(
            maxHealth: 120,
            currentHealth: 88,
            defence: 12,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: 98
            );

        public static UnitProperties PROPERTIES_SET_D = new UnitProperties(
            maxHealth: 80,
            currentHealth: 71,
            defence: 12,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: 98
            );

        public static UnitProperties PROPERTIES_HEALTH_OVERFLOW = new UnitProperties(
            maxHealth: 80,
            currentHealth: 100,
            defence: 15,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: 98
            );

        public static UnitProperties PROPERTIES_HEALTH_UNDERFLOW = new UnitProperties(
            maxHealth: 80,
            currentHealth: -1,
            defence: 15,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: 98
            );

        public static UnitProperties PROPERTIES_MAX_HEALTH_UNDERFLOW = new UnitProperties(
            maxHealth: -1,
            currentHealth: -1,
            defence: 15,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: 82
            );

        public static UnitProperties PROPERTIES_ACTION_POINTS_OVERFLOW = new UnitProperties(
            maxHealth: 80,
            currentHealth: 100,
            defence: 15,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: 101
            );

        public static UnitProperties PROPERTIES_ACTION_POINTS_UNDERFLOW = new UnitProperties(
            maxHealth: 80,
            currentHealth: 100,
            defence: 15,
            attack: 82,
            range: 10,
            maxActionPoints: 100,
            currentActionPoints: -1
            );

        public static UnitProperties PROPERTIES_MAX_ACTION_POINTS_UNDERFLOW = new UnitProperties(
            maxHealth: 80,
            currentHealth: 100,
            defence: 15,
            attack: 82,
            range: 10,
            maxActionPoints: -1,
            currentActionPoints: -1
            );

        public static Unit MakeUnitA()
        {
            return Unit.CreateNewUnit("DUMMY_A", PROPERTIES_SET_A, faction: Unit.UnitFaction.Friendly);
        }

        public static Unit MakeUnitB()
        {
            return Unit.CreateNewUnit("DUMMY_B", PROPERTIES_SET_B, 12, faction: Unit.UnitFaction.Enemy);
        }

        public static Unit MakeUnitC()
        {
            return Unit.CreateNewUnit("DUMMY_C", PROPERTIES_SET_C, 51);
        }

        public static Unit MakeUnitD()
        {
            return Unit.CreateNewUnit("DUMMY_D", PROPERTIES_SET_A, 22);
        }

        public static TileData TILEDATA_GROUND_10 = new TileData(10, TileData.TileType.Ground);
        public static TileData TILEDATA_GROUND_0 = new TileData(0, TileData.TileType.Ground);
        public static TileData TILEDATA_GROUND_50 = new TileData(50, TileData.TileType.Ground);

        public static TileData TILEDATA_HALFCOVER = new TileData(int.MaxValue, TileData.TileType.HalfCover);
        public static TileData TILEDATA_FULLCOVER = new TileData(int.MaxValue, TileData.TileType.FullCover);

        public static UnitCensus UNIT_CENSUS_A = new UnitCensus(new Dictionary<Vector3Int, Unit>()
        {
            [new Vector3Int(1, 1, 0)] = MakeUnitA(),
            [new Vector3Int(0, 1, 0)] = MakeUnitB(),
            [new Vector3Int(2, 1, 0)] = MakeUnitC(),
            [new Vector3Int(2, 0, 0)] = MakeUnitD(),
        });

        public static UnitCensus UNIT_CENSUS_SINGLE_010 = new UnitCensus(new Dictionary<Vector3Int, Unit>()
        {
            [new Vector3Int(0, 1, 0)] = MakeUnitA()
        });

        public static UnitCensus UNIT_CENSUS_SINGLE_120 = new UnitCensus(new Dictionary<Vector3Int, Unit>()
        {
            [new Vector3Int(1, 2, 0)] = MakeUnitA()
        });

        public static UnitCensus UNIT_CENSUS_EMPTY = new UnitCensus();

        public static TileCensus TILE_CENSUS_POPTART_3X3 = new TileCensus(new Dictionary<Vector3Int, TileData>()
        {
            [new Vector3Int(0, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 2, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(1, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 1, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(1, 2, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(2, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 2, 0)] = TILEDATA_GROUND_10,
        });

        public static TileCensus TILE_CENSUS_UNIFORM_4x4 = new TileCensus(new Dictionary<Vector3Int, TileData>()
        {
            [new Vector3Int(0, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 3, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(1, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 3, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(2, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 3, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(3, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 3, 0)] = TILEDATA_GROUND_10
        });

        public static TileCensus TILE_CENSUS_CARDINAL_CROSS_5X5 = new TileCensus(new Dictionary<Vector3Int, TileData>()
        {
            [new Vector3Int(0, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(1, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(2, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(3, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(4, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(4, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(4, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(4, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(4, 4, 0)] = TILEDATA_GROUND_10,
        });

        public static TileCensus TILE_CENSUS_CLAW_5X5 = new TileCensus(new Dictionary<Vector3Int, TileData>()
        {
            [new Vector3Int(0, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(0, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(1, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 1, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(1, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(2, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(2, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(3, 0, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(3, 4, 0)] = TILEDATA_GROUND_10,

            [new Vector3Int(4, 2, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(4, 3, 0)] = TILEDATA_GROUND_10,
            [new Vector3Int(4, 4, 0)] = TILEDATA_GROUND_10,
        });

        public static TileCensus TILE_CENSUS_DEFINED_PATH_5X5= new TileCensus(new Dictionary<Vector3Int, TileData>()
        {
            [new Vector3Int(0, 0, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(0, 1, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(0, 2, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(0, 3, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(0, 4, 0)] = TILEDATA_GROUND_50,

            [new Vector3Int(1, 0, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(1, 1, 0)] = TILEDATA_GROUND_0,
            [new Vector3Int(1, 2, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(1, 3, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(1, 4, 0)] = TILEDATA_GROUND_0,

            [new Vector3Int(2, 0, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(2, 1, 0)] = TILEDATA_GROUND_0,
            [new Vector3Int(2, 2, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(2, 3, 0)] = TILEDATA_GROUND_0,
            [new Vector3Int(2, 4, 0)] = TILEDATA_GROUND_50,

            [new Vector3Int(3, 0, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(3, 1, 0)] = TILEDATA_GROUND_0,
            [new Vector3Int(3, 2, 0)] = TILEDATA_GROUND_0,
            [new Vector3Int(3, 3, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(3, 4, 0)] = TILEDATA_GROUND_50,

            [new Vector3Int(4, 0, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(4, 1, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(4, 2, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(4, 3, 0)] = TILEDATA_GROUND_50,
            [new Vector3Int(4, 4, 0)] = TILEDATA_GROUND_50
        });

        public static GameMapData GMD_EMPTY_POPTART = new GameMapData(UNIT_CENSUS_EMPTY, TILE_CENSUS_POPTART_3X3);

        public static GameMapData GMD_POPTART_OCCUPIED_TOP_RIGHT = new GameMapData(UNIT_CENSUS_SINGLE_010, TILE_CENSUS_POPTART_3X3);

        public static GameMapData GMD_POPTART_OCCUPIED_TOP_MIDDLE = new GameMapData(UNIT_CENSUS_SINGLE_120, TILE_CENSUS_POPTART_3X3);

        public static GameMapData GMD_CARDINAL_5X5_OCCUPIED_120 = new GameMapData(UNIT_CENSUS_SINGLE_120, TILE_CENSUS_CARDINAL_CROSS_5X5);

        public static GameMapData GMD_CLAW_5X5 = new GameMapData(UNIT_CENSUS_EMPTY, TILE_CENSUS_CLAW_5X5);

        public static GameMapData GMD_DEFINED_PATH_5X5 = new GameMapData(UNIT_CENSUS_EMPTY, TILE_CENSUS_DEFINED_PATH_5X5);

        public static GameMap MakeGameMapA()
        {
            Unit.ResetClass();

            TileCensus tileCensus = Warehouse.TILE_CENSUS_DEFINED_PATH_5X5;

            Unit unitA = MakeUnitA();
            Unit unitB = MakeUnitB();

            UnitCensus unitCensus = new UnitCensus(new Dictionary<Vector3Int, Unit>()
            {
                [new Vector3Int(0, 0, 0)] = unitA,
                [new Vector3Int(1, 4, 0)] = unitB
            }
            );

            GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);
            return GameMap.MakeNewMap(gameMapData);
        }
    }
}
