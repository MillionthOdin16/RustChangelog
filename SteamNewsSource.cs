using System.Collections;
using System.Collections.Generic;
using JSON;
using UnityEngine;

public static class SteamNewsSource
{
	public struct Story
	{
		public string name;

		public string url;

		public int date;

		public string text;

		public string author;
	}

	public static Story[] Stories;

	public static IEnumerator GetStories()
	{
		WWW www = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return www;
		Object json = Object.Parse(www.text);
		www.Dispose();
		if (json == null)
		{
			yield break;
		}
		Array items = json.GetObject("appnews").GetArray("newsitems");
		List<Story> storyList = new List<Story>();
		foreach (Value item in items)
		{
			string text2 = item.Obj.GetString("contents", "Missing Contents");
			text2 = text2.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
			storyList.Add(new Story
			{
				name = item.Obj.GetString("title", "Missing Title"),
				url = item.Obj.GetString("url", "Missing URL"),
				date = item.Obj.GetInt("date", 0),
				text = text2,
				author = item.Obj.GetString("author", "Missing Author")
			});
		}
		Stories = storyList.ToArray();
	}
}
