using System.Collections.Generic;
using UnityEngine;

public class GameMapDataOld
{
    private readonly BidirectionalDictionary<Vector3Int, UnitOld> positionUnitMapping;

    private readonly HashSet<Vector3Int> fullCoverPositions;
    private readonly HashSet<Vector3Int> halfCoverPositions;
    private readonly HashSet<Vector3Int> groundPositions;

    private readonly Dictionary<Vector3Int, int> tileCosts;

    public BidirectionalDictionary<Vector3Int, UnitOld>.Indexer<UnitOld, Vector3Int> UnitPositionMapping => positionUnitMapping.Reverse;
    public BidirectionalDictionary<Vector3Int, UnitOld>.Indexer<Vector3Int, UnitOld> PositionUnitMapping => positionUnitMapping.Forward;

    public GameMapDataOld(Dictionary<Vector3Int, UnitOld> positionUnitMap, 
                IEnumerable<Vector3Int> fullCoverPositions,
                IEnumerable<Vector3Int> halfCoverPositions,
                IEnumerable<Vector3Int> groundPositions,
                Dictionary<Vector3Int, int> groundTileCosts)
    {
        positionUnitMapping = new BidirectionalDictionary<Vector3Int, UnitOld>();

        foreach (KeyValuePair<Vector3Int, UnitOld> pair in positionUnitMap)
        {
            positionUnitMapping.Add(pair.Key, pair.Value);
        }

        this.fullCoverPositions = new HashSet<Vector3Int>(fullCoverPositions);
        this.halfCoverPositions = new HashSet<Vector3Int>(halfCoverPositions);
        this.groundPositions = new HashSet<Vector3Int>(groundPositions);
        this.tileCosts = new Dictionary<Vector3Int, int>(groundTileCosts);
    }

    public GameMapDataOld(Dictionary<Vector3Int, UnitOld> positionUnitMap, GameMapDataOld oldData)
    {
        positionUnitMapping = new BidirectionalDictionary<Vector3Int, UnitOld>();

        foreach (KeyValuePair<Vector3Int, UnitOld> pair in positionUnitMap)
        {
            positionUnitMapping.Add(pair.Key, pair.Value);
        }

        this.fullCoverPositions = new HashSet<Vector3Int>(oldData.fullCoverPositions);
        this.halfCoverPositions = new HashSet<Vector3Int>(oldData.halfCoverPositions);
        this.groundPositions = new HashSet<Vector3Int>(oldData.groundPositions);
        this.tileCosts = new Dictionary<Vector3Int, int>(oldData.tileCosts);
    }

    public bool HasFullCoverAt(Vector3Int position)
    {
        return fullCoverPositions.Contains(position);
    }

    public bool HasHalfCoverAt(Vector3Int position)
    {
        return halfCoverPositions.Contains(position);
    }

    public bool IsWalkableOn(Vector3Int position)
    {
        return groundPositions.Contains(position)
            && !halfCoverPositions.Contains(position)
            && !fullCoverPositions.Contains(position)
            && !positionUnitMapping.Forward.ContainsKey(position);
    }

    public int GetTileCost(Vector3Int position)
    {
        return tileCosts[position];
    }

    public Vector3Int GetPositionByUnit(UnitOld unit)
    {
        return UnitPositionMapping[unit];
    }

    public UnitOld GetUnitByPosition(Vector3Int position)
    {
        return PositionUnitMapping[position];
    }

    public bool ExistsUnitAt(Vector3Int position)
    {
        return PositionUnitMapping.ContainsKey(position);
    }
}