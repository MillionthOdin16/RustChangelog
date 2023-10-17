using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TmProEmojiRedirector : MonoBehaviour
{
	public struct EmojiSub
	{
		public int targetCharIndex;

		public int targetCharIndexWithRichText;

		public string targetEmoji;

		public RustEmojiLibrary.EmojiSource targetEmojiResult;

		public TMP_CharacterInfo charToReplace;
	}

	public GameObjectRef SpritePrefab;

	public float EmojiScale = 1.5f;

	public bool NonDestructiveChange;

	public static void FindEmojiSubstitutions(string text, RustEmojiLibrary library, List<(EmojiSub, int)> foundSubs, bool isServer = false, int messageLength = 0)
	{
		if (!text.Contains(":"))
		{
			return;
		}
		EmojiSub item = default(EmojiSub);
		bool flag = false;
		int num = 0;
		int num2 = 0;
		bool flag2 = false;
		int num3 = 0;
		if (messageLength > 0)
		{
			num3 = text.Length - messageLength;
		}
		foundSubs.Clear();
		using CharEnumerator charEnumerator = text.GetEnumerator();
		while (charEnumerator.MoveNext())
		{
			num2++;
			if (charEnumerator.Current == '<')
			{
				flag2 = true;
			}
			else if (flag2 && charEnumerator.Current == '>')
			{
				flag2 = false;
			}
			else
			{
				if (flag2)
				{
					continue;
				}
				if (num2 < num3)
				{
					num++;
					continue;
				}
				if (charEnumerator.Current == ':')
				{
					if (!flag)
					{
						flag = true;
						item.targetCharIndex = num;
						item.targetCharIndexWithRichText = num2 - 1;
					}
					else
					{
						if (library.TryGetEmoji(item.targetEmoji, out item.targetEmojiResult, out var skinVariantIndex, out var _, isServer))
						{
							foundSubs.Add((item, skinVariantIndex));
						}
						item = default(EmojiSub);
						flag = false;
					}
				}
				else if (flag)
				{
					item.targetEmoji += charEnumerator.Current;
					if (charEnumerator.Current == ' ')
					{
						item = default(EmojiSub);
						flag = false;
					}
				}
				num++;
			}
		}
	}
}
