using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class EZUIDrag : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Color OriginColor = Color.white, DragColor = Color.white;
    // 拖拽时，为了保持图片在最上层，需要临时将图片放到另一个父物体下
    // 在拖拽结束后，再将图片放回原来父物体下
    public Transform DragParent;
    private Transform _originParent;
    
    private Image _img;
    private RectTransform _imgTransform;
    //存储按下鼠标时的图片-鼠标位置差
    private Vector3 _originPos, _offsetPos;

    void Start()
    {
        //获取图片，因为要获取他的RectTransform
        _img = GetComponent<Image>();
        _imgTransform = _img.rectTransform;
        _originPos = _imgTransform.position;
        _originParent = _imgTransform.parent;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _offsetPos = _imgTransform.position - Input.mousePosition;
    }

    /// OnDrag 执行前的额外处理，如果返回值为 false，则 OnDrag 不执行
    public abstract bool DragEnableProcess();
    public void OnDrag(PointerEventData eventData)
    {
        if (!DragEnableProcess()) {
            return;
        }
        
        _img.color = DragColor;
        _imgTransform.parent = DragParent;
        //将鼠标的位置坐标进行钳制，然后加上位置差再赋值给图片position
        _imgTransform.position = new Vector3(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width), Mathf.Clamp(Input.mousePosition.y, 0, Screen.height), 0) + _offsetPos;
    }

    /// OnPointerUp 执行后的额外处理，参数为点击相关数据
    public abstract void PointerUpProcess(PointerEventData eventData);
    public void OnPointerUp(PointerEventData eventData)
    {
        _img.color = OriginColor;
        _imgTransform.parent = _originParent;
        _imgTransform.position = _originPos;

        PointerUpProcess(eventData);
    }

    public void BeDragColor()
    {
        _img.color = DragColor;
    }

}