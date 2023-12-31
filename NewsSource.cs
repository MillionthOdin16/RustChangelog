using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Models;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewsSource : MonoBehaviour
{
	private struct ParagraphBuilder
	{
		public StringBuilder StringBuilder;

		public List<string> Links;

		public static ParagraphBuilder New()
		{
			ParagraphBuilder result = default(ParagraphBuilder);
			result.StringBuilder = new StringBuilder();
			result.Links = new List<string>();
			return result;
		}

		public void AppendLine()
		{
			StringBuilder.AppendLine();
		}

		public void Append(string text)
		{
			StringBuilder.Append(text);
		}
	}

	private static readonly Regex BbcodeParse = new Regex("([^\\[]*)(?:\\[(\\w+)(?:=([^\\]]+))?\\](.*?)\\[\\/\\2\\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

	public RustText title;

	public RustText date;

	public RustText authorName;

	public HttpImage coverImage;

	public RectTransform container;

	public Button button;

	public RustText paragraphTemplate;

	public HttpImage imageTemplate;

	public HttpImage youtubeTemplate;

	private static readonly string[] BulletSeparators = new string[1] { "[*]" };

	public void Awake()
	{
		GA.DesignEvent("news:view");
	}

	public void OnEnable()
	{
		if (SteamNewsSource.Stories != null && SteamNewsSource.Stories.Length != 0)
		{
			SetStory(SteamNewsSource.Stories[0]);
		}
	}

	public void SetStory(SteamNewsSource.Story story)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		PlayerPrefs.SetInt("lastNewsDate", story.date);
		TransformEx.DestroyAllChildren((Transform)(object)container, false);
		((TMP_Text)title).text = story.name;
		((TMP_Text)authorName).text = "by " + story.author;
		string text = NumberExtensions.FormatSecondsLong((long)(Epoch.Current - story.date));
		((TMP_Text)date).text = "Posted " + text + " ago";
		((UnityEventBase)button.onClick).RemoveAllListeners();
		((UnityEvent)button.onClick).AddListener((UnityAction)delegate
		{
			string text2 = GetBlogPost()?.Url ?? story.url;
			Debug.Log((object)("Opening URL: " + text2));
			Application.OpenURL(text2);
		});
		string firstImage = GetBlogPost()?.HeaderImage;
		ParagraphBuilder currentParagraph = ParagraphBuilder.New();
		ParseBbcode(ref currentParagraph, story.text, ref firstImage);
		AppendParagraph(ref currentParagraph);
		if (firstImage != null)
		{
			coverImage.Load(firstImage);
		}
		RustText[] componentsInChildren = ((Component)container).GetComponentsInChildren<RustText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoAutoSize();
		}
		BlogInfo GetBlogPost()
		{
			Manifest manifest = Application.Manifest;
			if (manifest == null)
			{
				return null;
			}
			NewsInfo news = manifest.News;
			if (news == null)
			{
				return null;
			}
			BlogInfo[] blogs = news.Blogs;
			if (blogs == null)
			{
				return null;
			}
			return List.FindWith<BlogInfo, string>((IReadOnlyCollection<BlogInfo>)(object)blogs, (Func<BlogInfo, string>)((BlogInfo b) => b.Title), story.name, (IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
		}
	}

	private void ParseBbcode(ref ParagraphBuilder currentParagraph, string bbcode, ref string firstImage, int depth = 0)
	{
		foreach (Match item in BbcodeParse.Matches(bbcode))
		{
			string value = item.Groups[1].Value;
			string value2 = item.Groups[2].Value;
			string value3 = item.Groups[3].Value;
			string value4 = item.Groups[4].Value;
			currentParagraph.Append(value);
			switch (value2.ToLowerInvariant())
			{
			case "previewyoutube":
				if (depth == 0)
				{
					string[] array2 = value3.Split(';', StringSplitOptions.None);
					AppendYouTube(ref currentParagraph, array2[0]);
				}
				break;
			case "h1":
			case "h2":
				currentParagraph.Append("<size=200%>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</size>");
				break;
			case "h3":
				currentParagraph.Append("<size=175%>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</size>");
				break;
			case "h4":
				currentParagraph.Append("<size=150%>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</size>");
				break;
			case "b":
				currentParagraph.Append("<b>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</b>");
				break;
			case "u":
				currentParagraph.Append("<u>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</u>");
				break;
			case "i":
				currentParagraph.Append("<i>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</i>");
				break;
			case "strike":
				currentParagraph.Append("<s>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</s>");
				break;
			case "noparse":
				currentParagraph.Append(value4);
				break;
			case "url":
			{
				if (value4.Contains("[img]", StringComparison.InvariantCultureIgnoreCase))
				{
					ParseBbcode(ref currentParagraph, value4, ref firstImage, depth);
					break;
				}
				int count = currentParagraph.Links.Count;
				currentParagraph.Links.Add(value3);
				currentParagraph.Append($"<link={count}><u>");
				ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</u></link>");
				break;
			}
			case "list":
			{
				currentParagraph.AppendLine();
				string[] array = GetBulletPoints(value4);
				foreach (string text3 in array)
				{
					if (!string.IsNullOrWhiteSpace(text3))
					{
						currentParagraph.Append("\t• ");
						currentParagraph.Append(text3.Trim());
						currentParagraph.AppendLine();
					}
				}
				break;
			}
			case "olist":
			{
				currentParagraph.AppendLine();
				string[] bulletPoints = GetBulletPoints(value4);
				int num = 1;
				string[] array = bulletPoints;
				foreach (string text2 in array)
				{
					if (!string.IsNullOrWhiteSpace(text2))
					{
						currentParagraph.Append($"\t{num++} ");
						currentParagraph.Append(text2.Trim());
						currentParagraph.AppendLine();
					}
				}
				break;
			}
			case "img":
				if (depth == 0)
				{
					string text = value4.Trim();
					if (firstImage == null)
					{
						firstImage = text;
					}
					AppendImage(ref currentParagraph, text);
				}
				break;
			}
		}
	}

	private static string[] GetBulletPoints(string listContent)
	{
		return listContent?.Split(BulletSeparators, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
	}

	private void AppendParagraph(ref ParagraphBuilder currentParagraph)
	{
		if (currentParagraph.StringBuilder.Length > 0)
		{
			string text = currentParagraph.StringBuilder.ToString();
			RustText obj = Object.Instantiate<RustText>(paragraphTemplate, (Transform)(object)container);
			ComponentExtensions.SetActive<RustText>(obj, true);
			obj.SetText(text);
			NewsParagraph newsParagraph = default(NewsParagraph);
			if (((Component)obj).TryGetComponent<NewsParagraph>(ref newsParagraph))
			{
				newsParagraph.Links = currentParagraph.Links;
			}
		}
		currentParagraph = ParagraphBuilder.New();
	}

	private void AppendImage(ref ParagraphBuilder currentParagraph, string url)
	{
		AppendParagraph(ref currentParagraph);
		HttpImage obj = Object.Instantiate<HttpImage>(imageTemplate, (Transform)(object)container);
		ComponentExtensions.SetActive<HttpImage>(obj, true);
		obj.Load(url);
	}

	private void AppendYouTube(ref ParagraphBuilder currentParagraph, string videoId)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		AppendParagraph(ref currentParagraph);
		HttpImage obj = Object.Instantiate<HttpImage>(youtubeTemplate, (Transform)(object)container);
		ComponentExtensions.SetActive<HttpImage>(obj, true);
		obj.Load("https://img.youtube.com/vi/" + videoId + "/maxresdefault.jpg");
		RustButton component = ((Component)obj).GetComponent<RustButton>();
		if ((Object)(object)component != (Object)null)
		{
			string videoUrl = "https://www.youtube.com/watch?v=" + videoId;
			component.OnReleased.AddListener((UnityAction)delegate
			{
				Debug.Log((object)("Opening URL: " + videoUrl));
				Application.OpenURL(videoUrl);
			});
		}
	}
}
