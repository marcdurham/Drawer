namespace WinCad
{
    public interface IDrawingView
    {
        string Title { set; }
        string Status { set; }
        string SecondStatus { set; }
        Canvas Canvas { get; set; }
        bool OrthoIsOn { get; }
        System.Drawing.Size PictureSize { get; }

        void RefreshImage();
        void RenderLayers();
        UserAnswer AskUser(string question);
    }
}
