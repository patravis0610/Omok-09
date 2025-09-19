using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite black_Marker;
    [SerializeField] private Sprite white_Marker;
    [SerializeField] private Image marker_img;
    [SerializeField] private GameObject lastPosition;
    [SerializeField] private GameObject curPosition;
    [SerializeField] private GameObject x_Marker;

    public delegate void OnBlockClicked(int index);
    private OnBlockClicked _onBlockClicked;

    // 마커 타입
    public enum MarkerType { None, Black, White }

    // Block Index
    private int _blockIndex;

    private bool _isForbidden = false;

    // 1. 초기화
    public void InitMarker(int blockIndex, OnBlockClicked onBlockClicked)
    {
        _blockIndex = blockIndex;
        SetMarker(MarkerType.None);
       // SetBlockColor(_defaultBlockColor);
        _onBlockClicked = onBlockClicked;
    }

    // 2. 마커 설정
    public void SetMarker(MarkerType markerType)
    {
        switch (markerType)
        {
            case MarkerType.None:
                marker_img.sprite = null;
                break;
            case MarkerType.Black:
                marker_img.sprite = black_Marker;
                marker_img.gameObject.SetActive(true);
                break;
            case MarkerType.White:
                marker_img.sprite = white_Marker;
                marker_img.gameObject.SetActive(true);
                break;
        }
    }

    //canvas 안에 ui로 했기 때문에 이걸로 
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isForbidden) return;

        _onBlockClicked?.Invoke(_blockIndex);
    }
    public void SetLastPosition(bool isLast)
    {
        lastPosition.SetActive(isLast);
    }

    //금수 마커 표시
    public void SetForbiddenMarker(bool setForbidden)
    {
        x_Marker.SetActive(setForbidden);
        _isForbidden = setForbidden;
    }
}
