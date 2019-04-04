namespace WinCad
{
    public interface IDrawingView
    {
        string Status { set; }
        Canvas Canvas { get; set; }
    }
}
