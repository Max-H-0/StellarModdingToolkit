using UnityEngine;
using UnityEngine.UIElements;

namespace StellarModdingToolkit.UI;

internal partial class ResizeableElement : VisualElement
{
    protected ResizeMode CurrentResizeMode { get; private set; }
    
    private Rect _startRect;
    private Vector2 _startPoint;

    
    public ResizeableElement()
    {
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<PointerMoveEvent>(OnPointerMoved);
        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }


    public int ResizeHandleWidth { get; set; }

    protected virtual VisualElement MoveHandle => this;

    
    private void OnPointerDown(PointerDownEvent e)
    {
        if (e.button != (int)MouseButton.LeftMouse) return;

        e.StopPropagation();
        this.CapturePointer(e.pointerId);

        CurrentResizeMode = GetResizeMode(e.localPosition);
        _startRect = layout;
        _startPoint = e.position;

        BringToFront();
    }

    private void OnPointerMoved(PointerMoveEvent e)
    {
        Rect newRect = layout;
        Vector2 delta = (Vector2)e.position - _startPoint;
        
        
        var minWidth = resolvedStyle.minWidth.value;
        var minHeight = resolvedStyle.minHeight.value;

        var maxWidth = resolvedStyle.maxWidth.keyword != StyleKeyword.None? resolvedStyle.maxWidth.value : float.PositiveInfinity;
        var maxHeight = resolvedStyle.maxHeight.keyword != StyleKeyword.None? resolvedStyle.maxHeight.value : float.PositiveInfinity;
        
        
        if (CurrentResizeMode.HasFlag(ResizeMode.Left))
        {
            newRect.xMin = Mathf.Clamp(_startRect.xMin + delta.x, _startRect.xMax - maxWidth, _startRect.xMax - minWidth);
        }

        if (CurrentResizeMode.HasFlag(ResizeMode.Right))
        {
            newRect.xMax = Mathf.Clamp(_startRect.xMax + delta.x, _startRect.xMin + minWidth, _startRect.xMin + maxWidth);
        }

        if (CurrentResizeMode.HasFlag(ResizeMode.Top))
        {
            newRect.yMin = Mathf.Clamp(_startRect.yMin + delta.y, _startRect.yMax - maxHeight, _startRect.yMax - minHeight);
        }

        if (CurrentResizeMode.HasFlag(ResizeMode.Bottom))
        {
            newRect.yMax = Mathf.Clamp(_startRect.yMax + delta.y, _startRect.yMin + minHeight, _startRect.yMin + maxHeight);
        }


        if (CurrentResizeMode.HasFlag(ResizeMode.Move))
        {
            newRect.position += (Vector2)e.deltaPosition;
        }


        UpdateRect(newRect);
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        this.ReleasePointer(e.pointerId);
        CurrentResizeMode = ResizeMode.None;
    }

    
    public void UpdateRect(Rect rect)
    {
        if (parent is null) return;

        Rect parentRect = parent.contentRect;

        if (CurrentResizeMode == ResizeMode.Move)
        {
            rect.position = new Vector2
            (
                Mathf.Clamp(rect.position.x, parentRect.xMin, parentRect.xMax - rect.size.x),
                Mathf.Clamp(rect.position.y, parentRect.yMin, parentRect.yMax - rect.size.y)
            );
        }

        rect.min = Vector2.Max(rect.min, parentRect.min);
        rect.max = Vector2.Min(rect.max, parentRect.max);

        GetType().GetProperty("layout")?.SetValue(this, rect);
    }


    protected ResizeMode GetResizeMode(Vector2 localPosition)
    {
        Rect rect = Rect.MinMaxRect
        (
            ResizeHandleWidth,
            ResizeHandleWidth,
            resolvedStyle.width - ResizeHandleWidth,
            resolvedStyle.height - ResizeHandleWidth
        );

        bool isLeft = localPosition.x <= rect.xMin;
        bool isRight = localPosition.x >= rect.xMax;
        bool isTop = localPosition.y <= rect.yMin;
        bool isBottom = localPosition.y >= rect.yMax;

        var resizeModeInt = (isLeft ? 0b_1000 : 0) + (isRight ? 0b_0100 : 0) + (isTop ? 0b_0010 : 0) + (isBottom ? 0b_0001 : 0);
        var resizeMode = (ResizeMode)resizeModeInt;

        if (resizeMode == ResizeMode.None && MoveHandle.worldBound.Contains(this.LocalToWorld(localPosition))) return ResizeMode.Move;

        return resizeMode;
    }
}