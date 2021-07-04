using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    [SerializeField] private GameObject _markerPrefab;
    private List<GameObject> _markers = new List<GameObject>();

    public void SpawnMarker(Vector3 position)
    {
        var newMarker = Instantiate(_markerPrefab, position, Quaternion.identity, transform);
        _markers.Add(newMarker);
    }

    public void ClearMarkers()
    {
        while(_markers.Count > 0)
        {
            GameObject marker = _markers[0];
            _markers.RemoveAt(0);
            Destroy(marker.gameObject);
        }
    }
}
