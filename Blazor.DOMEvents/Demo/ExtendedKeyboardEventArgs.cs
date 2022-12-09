using Microsoft.AspNetCore.Components.Web;

namespace Blazor.DOMEvents.Demo;

public class Element
{
    public string ID { get; set; } = "";
}

public class ExtendedMouseEventArgs : KeyboardEventArgs
{
    public Element Target { get; set; }
    public int ClientX { get; set; }
    public int ClientY { get; set; }    
    public int LayerX { get; set; }    
    public int LayerY { get; set; }    
    public int PageX { get; set; }    
    public int PageY { get; set; }    
    public int OffsetX { get; set; }    
    public int OffsetY { get; set; }    
    public int X { get; set; }    
    public int Y { get; set; }    

    public ExtendedMouseEventArgs()
    {
        Target = new Element();
    }
}
