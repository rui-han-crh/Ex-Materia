using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;
using System;
using ColorLookUp;
using System.Threading;
using Banzan.Lib.Utility;

public partial class GameManagerOld : MonoBehaviour
{
    [SerializeField]
    private GameObject[] unitGameObjects;



    /////////////////////////////////////////// Tilemaps ////////////////////////////////////////////
    [Serializable]
    private class TileMapCollection
    {
        [SerializeField]
        protected Tilemap fullCoverTilemap;
        [SerializeField]
        private Tilemap halfCoverTilemap;
        [SerializeField]
        private Tilemap groundTilemap;
        [SerializeField]
        private Tilemap tileHighlights;

        [SerializeField]
        private Tile blockSelectorTile;

        [SerializeField]
        private TileCost[] tileCosts;

        public Tilemap GroundTilemap => groundTilemap;
        public Tilemap FullCoverTilemap => fullCoverTilemap;
        public Tilemap HalfCoverTilemap => halfCoverTilemap;
        public Tilemap TileHighlights => tileHighlights;

        public Tile BlockSelectorTile => blockSelectorTile;

        public TileCost[] TileCosts => tileCosts;
    }


    [SerializeField]
    private TileMapCollection tileMapCollection;

    /////////////////////////////////////////// END ////////////////////////////////////////////

    ////////////////////////////////////////// Health Bars ///////////////////////////////////////
    [Serializable]
    private class HealthBarDataBag
    {
        [SerializeField]
        private GameObject healthBarPrefab;

        [SerializeField]
        private RectTransform healthBarCollection;

        public GameObject HealthBarPrefab => healthBarPrefab;
        public RectTransform HealthBarCollection => healthBarCollection;
    }

    [SerializeField]
    private HealthBarDataBag healthBarDataBag;

    //////////////////////////////////////////// END /////////////////////////////////////////////////

    /////////////////////////////////////////// UI //////////////////////////////////////////////
    [Serializable]
    private class ScreenUI
    {
        [SerializeField]
        private CharacterStatsUIBehaviour characterStatsUIBehaviour;

        [SerializeField]
        private LinearAnimation canvasLinearAnimation;

        public CharacterStatsUIBehaviour CharacterStatsUIBehaviour => characterStatsUIBehaviour;
        public LinearAnimation CanvasLinearAnimation => canvasLinearAnimation;
    }

    [SerializeField]
    private ScreenUI screenUI;

    private CharacterStatsUIBehaviour characterStatsUIBehaviour;
    private LinearAnimation canvasLinearAnimation;
    private int characterSheetIndex;
    private LineRenderer pathLine;

    ////////////////////////////////////////// END ////////////////////////////////////////////////////
    
    
    /////////////////////////////////////////// Ungrouped /////////////////////////////////////////////
    [SerializeField] // temporary, will remove
    private GameObject mainLight;

}
