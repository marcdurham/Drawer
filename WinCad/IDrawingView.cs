namespace WinCad
{
    public interface IDrawingView
    {
        string Status { set; }
        string SecondStatus { set; }
        Canvas Canvas { get; set; }
        bool OrthoIsOn { get; }

        void InvalidateImage();
        void RenderLayers();
    }
}
