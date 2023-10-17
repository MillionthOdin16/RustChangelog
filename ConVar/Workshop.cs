namespace ConVar;

[Factory("workshop")]
public class Workshop : ConsoleSystem
{
	[ServerVar]
	public static void print_approved_skins(Arg arg)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (!PlatformService.Instance.IsValid || PlatformService.Instance.ItemDefinitions == null)
		{
			return;
		}
		TextTable val = new TextTable();
		val.AddColumn("name");
		val.AddColumn("itemshortname");
		val.AddColumn("workshopid");
		val.AddColumn("workshopdownload");
		foreach (IPlayerItemDefinition itemDefinition in PlatformService.Instance.ItemDefinitions)
		{
			string name = itemDefinition.Name;
			string itemShortName = itemDefinition.ItemShortName;
			string text = itemDefinition.WorkshopId.ToString();
			string text2 = itemDefinition.WorkshopDownload.ToString();
			val.AddRow(new string[4] { name, itemShortName, text, text2 });
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}
}
