using FairyGUI;

public static class GComponentExpand
{
	public static GComponent GetCom(this GComponent com, string name)
	{
		return com.GetChild(name) as GComponent;
	}
	public static GTextField GetText(this GComponent com,string name)
	{
		return com.GetChild(name)as GTextField;
	}
    public static GImage GetImage(this GComponent com, string name)
    {
        return com.GetChild(name) as GImage;
    }
    public static GList GetList(this GComponent com, string name)
	{
		return com.GetChild(name) as GList;
	}
    public static GTree GetTree(this GComponent com, string name)
    {
        return com.GetChild(name) as GTree;
    }
    public static GButton GetButton(this GComponent com, string name)
	{
		return com.GetChild(name) as GButton;
	}
	public static GTextInput GetTextInput(this GComponent com, string name)
	{
		return com.GetChild(name) as GTextInput;
	}
	public static GGraph GetGraph(this GComponent com, string name)
	{
		return com.GetChild(name) as GGraph;
	}
	public static GLoader GetGLoader(this GComponent com, string name)
	{
		return com.GetChild(name) as GLoader;
	}
    public static GLoader3D GetGLoader3D(this GComponent com, string name)
    {
        return com.GetChild(name) as GLoader3D;
    }
    public static GGroup GetGroup(this GComponent com, string name)
	{
		return com.GetChild(name) as GGroup;
	}
	public static GComboBox GetComboBox(this GComponent com, string name)
	{
		return com.GetChild(name) as GComboBox;
	}
	public static GProgressBar GetProgressBar(this GComponent com, string name)
	{
		return com.GetChild(name) as GProgressBar;
	}
    public static GSlider GetSlider(this GComponent com,string name)
    {
        return com.GetChild(name) as GSlider;
    }
    public static void SetItemRender(this GList self, ListItemRenderer itemRender,bool isVirtual = true)
    {
        self.itemRenderer = itemRender;
        if (isVirtual)
            self.SetVirtual();
    }
}
