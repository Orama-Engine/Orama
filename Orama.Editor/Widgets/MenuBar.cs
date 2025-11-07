using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Layouts;
using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Editor.Widgets;

internal class MenuBar : Widget
{
    public MenuBar() : base()
    {
        Layout = new HBoxLayout();
        Layout.Spacing = 4;

        Button file = new Button("File");
        Button edit = new Button("Edit");
        Button view = new Button("View");

        AddChild(file);
        AddChild(edit);
        AddChild(view);

        Rect = new Rect(0, 0, 0, 0);
        HorizontalSizePolicy = SizePolicy.Expand;
    }
}
