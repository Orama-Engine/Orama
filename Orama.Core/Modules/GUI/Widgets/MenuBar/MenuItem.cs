
namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// An Item in a Menubar.
/// </summary>
public class MenuItem
{
    /// <summary> The displayed name of the item. </summary>
    public string Name { get; set; } = "Item";

    /// <summary> Called when the item is clicked. </summary>
    public Action? OnClick { get; set; }

    /// <summary> The subitems of the item. </summary>
    public List<MenuItem> Children { get; set; } = new();

    /// <summary> Whether the item has children. </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary> The label of the item. </summary>
    public MenuItem(string label, Action? onClick = null)
    {
        Name = label;
        OnClick = onClick;
    }
}
