namespace WinCad
{
    public enum DrawModes
    {
        Ready,
        InsertingImageFirstCorner,
        InsertingImageSecondCorner,
        DrawingRectangleFirstCorner,
        DrawingRectangleSecondCorner,
        StartDrawing,
        DrawingPolylineFirstVertex,
        DrawingPolylineExtraVertices,
        DrawingPolylineSecondVertex,
        StartInsertingBlock,
        SelectEntity,
        Panning
    }
}
