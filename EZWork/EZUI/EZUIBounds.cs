using System;
using Cinemachine.Utility;
using UnityEngine;

/// <summary>
/// 实现UI元素包围盒功能：检测某个坐标是否在 RectTransform 包围盒内
/// </summary>
public class EZUIBounds : MonoBehaviour
{
    enum PivotEnum
    {
        Center, Left, Right, Up, Down
    }
    
    private RectTransform rectTransform;
    private Bounds bounds;
    // 用于对齐方式的偏移校正
    private PivotEnum pivotX = PivotEnum.Center, pivotY = PivotEnum.Center;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        InitPivotType();
    }

    private void InitPivotType()
    {
        Vector2 pivotValue = rectTransform.pivot;
        if (Math.Abs(pivotValue.x) < 0.001f) {
            pivotX = PivotEnum.Left;
        }
        else if (Math.Abs(pivotValue.x - 1) < 0.001f) {
            pivotX = PivotEnum.Right;
        }
        
        if (Math.Abs(pivotValue.y) < 0.001f) {
            pivotY = PivotEnum.Down;
        }
        else if (Math.Abs(pivotValue.y - 1) < 0.001f) {
            pivotY = PivotEnum.Up;
        }
    }

    /// <summary>
    /// 初始化 UIBounds
    /// </summary>
    /// <param name="scale">屏幕分辨率 / 设计分辨率，通常为1；举例：如果为像素游戏，且分辨率为屏幕分辨率的四分之一，则这里填入4</param>
    public void InitBounds(float scale = 1)
    {
        Vector3 position = rectTransform.position;
        var rect = rectTransform.rect;
        // X轴校正
        switch (pivotX) {
            case PivotEnum.Center:
                bounds.center = new Vector3(position.x, position.y, position.z);
                break;
            case PivotEnum.Left:
                bounds.center = new Vector3(position.x + rect.width / 2f * scale, position.y, position.z);
                break;
            case PivotEnum.Right:
                bounds.center = new Vector3(position.x - rect.width / 2f * scale, position.y, position.z);
                break;
        }
        // Y轴校正（在前者的基础上）
        position = bounds.center;
        switch (pivotY) {
            case PivotEnum.Center:
                bounds.center = new Vector3(position.x, position.y, position.z);
                break;
            case PivotEnum.Down:
                bounds.center = new Vector3(position.x, position.y + rect.height / 2f * scale, position.z);
                break;
            case PivotEnum.Up:
                bounds.center = new Vector3(position.x, position.y - rect.height / 2f * scale, position.z);
                break;
        }
        bounds.extents = new Vector3(rect.width / 2f * scale, rect.height / 2f * scale, 0);
    }

    /// <summary>
    /// UIBounds 是否包含该坐标
    /// </summary>
    /// <param name="position">坐标</param>
    /// <returns></returns>
    public bool Contains(Vector3 position)
    {
        return bounds.Contains(position);
    }
}