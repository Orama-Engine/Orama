
using Orama.Core.Modules.GUI.Layouts;

namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// A Menu Bar widget.
/// </summary>
public class MenuBar : Widget
{
    /// <summary> The MenuItems in the MenuBar. </summary>
    public List<MenuItem> Items { get; set; } = new();

    /// <summary> Initializes a new instance of the <see cref="MenuBar"/> class. </summary>
    public MenuBar() : base()
    {
        Layout = new HBoxLayout();
        Layout.Spacing = 4;

        Rect = new Rect(0, 0, 0, 0);
        HorizontalSizePolicy = SizePolicy.Expand;
    }

    /// <summary> Adds a MenuItem to the MenuBar. </summary>
    public void AddMenuItem(MenuItem item)
    {
        Items.Add(item);

        Button button = new Button(item.Name);
        button.Clicked += () => item.OnClick?.Invoke();
        button.Parent = this;

        AddChild(button);
    }
}
