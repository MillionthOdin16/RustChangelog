using Network;

public class cui
{
	[ServerUserVar]
	public static void cui_test(Arg args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		CommunityEntity.ServerInstance.ClientRPCEx(new SendInfo
		{
			connection = args.Connection
		}, null, "AddUI", "[\t\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"parent\": \"Overlay\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.RawImage\",\n\t\t\t\t\t\t\t\t\t\"imagetype\": \"Tiled\",\n\t\t\t\t\t\t\t\t\t\"color\": \"1.0 1.0 1.0 1.0\",\n\t\t\t\t\t\t\t\t\t\"url\": \"http://files.facepunch.com/garry/2015/June/03/2015-06-03_12-19-17.jpg\",\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\": \"0 0\",\n\t\t\t\t\t\t\t\t\t\"anchormax\": \"1 1\"\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"NeedsCursor\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"name\": \"buttonText\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"text\":\"Do you want to press a button?\",\n\t\t\t\t\t\t\t\t\t\"fontSize\":32,\n\t\t\t\t\t\t\t\t\t\"align\": \"MiddleCenter\",\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\": \"0 0.5\",\n\t\t\t\t\t\t\t\t\t\"anchormax\": \"1 0.9\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"Button88\",\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Button\",\n\t\t\t\t\t\t\t\t\t\"close\":\"TestPanel7766\",\n\t\t\t\t\t\t\t\t\t\"command\":\"cui.endtest\",\n\t\t\t\t\t\t\t\t\t\"color\": \"0.9 0.8 0.3 0.8\",\n\t\t\t\t\t\t\t\t\t\"imagetype\": \"Tiled\"\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\": \"0.3 0.15\",\n\t\t\t\t\t\t\t\t\t\"anchormax\": \"0.7 0.2\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"parent\": \"Button88\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"text\":\"YES\",\n\t\t\t\t\t\t\t\t\t\"fontSize\":20,\n\t\t\t\t\t\t\t\t\t\"align\": \"MiddleCenter\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"ItemIcon\",\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Image\",\n\t\t\t\t\t\t\t\t\t\"color\": \"1.0 1.0 1.0 1.0\",\n\t\t\t\t\t\t\t\t\t\"imagetype\": \"Simple\",\n\t\t\t\t\t\t\t\t\t\"itemid\": -151838493,\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\":\"0.4 0.4\",\n\t\t\t\t\t\t\t\t\t\"anchormax\":\"0.4 0.4\",\n\t\t\t\t\t\t\t\t\t\"offsetmin\": \"-32 -32\",\n\t\t\t\t\t\t\t\t\t\"offsetmax\": \"32 32\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"ItemIconSkinTest\",\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Image\",\n\t\t\t\t\t\t\t\t\t\"color\": \"1.0 1.0 1.0 1.0\",\n\t\t\t\t\t\t\t\t\t\"imagetype\": \"Simple\",\n\t\t\t\t\t\t\t\t\t\"itemid\": -733625651,\n\t\t\t\t\t\t\t\t\t\"skinid\": 13035\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\":\"0.6 0.6\",\n\t\t\t\t\t\t\t\t\t\"anchormax\":\"0.6 0.6\",\n\t\t\t\t\t\t\t\t\t\"offsetmin\": \"-32 -32\",\n\t\t\t\t\t\t\t\t\t\"offsetmax\": \"32 32\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"UpdateLabelTest\",\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"text\":\"This should go away once you update!\",\n\t\t\t\t\t\t\t\t\t\"fontSize\":32,\n\t\t\t\t\t\t\t\t\t\"align\": \"MiddleRight\",\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t]\n\t\t\t\t\t");
	}

	[ServerUserVar]
	public static void cui_test_update(Arg args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		CommunityEntity.ServerInstance.ClientRPCEx(new SendInfo
		{
			connection = args.Connection
		}, null, "AddUI", "[\t\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"update\": true,\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.RawImage\",\n\t\t\t\t\t\t\t\t\t\"url\": \"https://files.facepunch.com/paddy/20220405/zipline_01.jpg\",\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"buttonText\",\n\t\t\t\t\t\t\t\"update\": true,\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"text\":\"This text just got updated!\",\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"ItemIcon\",\n\t\t\t\t\t\t\t\"update\": true,\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Image\",\n\t\t\t\t\t\t\t\t\t\"itemid\": -2067472972,\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"Button88\",\n\t\t\t\t\t\t\t\"update\": true,\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Button\",\n\t\t\t\t\t\t\t\t\t\"color\": \"0.9 0.3 0.3 0.8\",\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"UpdateLabelTest\",\n\t\t\t\t\t\t\t\"update\": true,\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"enabled\": false,\n\t\t\t\t\t\t\t\t},\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\t\t\t\t\t]\n\t\t\t\t\t");
	}

	[ServerUserVar]
	public static void endtest(Arg args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		args.ReplyWith("Ending Test!");
		CommunityEntity.ServerInstance.ClientRPCEx(new SendInfo
		{
			connection = args.Connection
		}, null, "DestroyUI", "TestPanel7766");
	}
}
