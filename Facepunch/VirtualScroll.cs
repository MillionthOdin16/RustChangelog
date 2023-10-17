using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Facepunch;

public class VirtualScroll : MonoBehaviour
{
	public interface IDataSource
	{
		int GetItemCount();

		void SetItemData(int i, GameObject obj);
	}

	public interface IVisualUpdate
	{
		void OnVisualUpdate(int i, GameObject obj);
	}

	public int ItemHeight = 40;

	public int ItemSpacing = 10;

	public RectOffset Padding;

	[Tooltip("Optional, we'll try to GetComponent IDataSource from this object on awake")]
	public GameObject DataSourceObject;

	public GameObject SourceObject;

	public ScrollRect ScrollRect;

	public RectTransform OverrideContentRoot;

	private IDataSource dataSource;

	private Dictionary<int, GameObject> ActivePool = new Dictionary<int, GameObject>();

	private Stack<GameObject> InactivePool = new Stack<GameObject>();

	private int BlockHeight => ItemHeight + ItemSpacing;

	public void Awake()
	{
		((UnityEvent<Vector2>)(object)ScrollRect.onValueChanged).AddListener((UnityAction<Vector2>)OnScrollChanged);
		if ((Object)(object)DataSourceObject != (Object)null)
		{
			SetDataSource(DataSourceObject.GetComponent<IDataSource>());
		}
	}

	public void OnDestroy()
	{
		((UnityEvent<Vector2>)(object)ScrollRect.onValueChanged).RemoveListener((UnityAction<Vector2>)OnScrollChanged);
	}

	private void OnScrollChanged(Vector2 pos)
	{
		Rebuild();
	}

	public void SetDataSource(IDataSource source, bool forceRebuild = false)
	{
		if (dataSource != source || forceRebuild)
		{
			dataSource = source;
			FullRebuild();
		}
	}

	public void FullRebuild()
	{
		int[] array = ActivePool.Keys.ToArray();
		foreach (int key in array)
		{
			Recycle(key);
		}
		Rebuild();
	}

	public void DataChanged()
	{
		foreach (KeyValuePair<int, GameObject> item in ActivePool)
		{
			dataSource.SetItemData(item.Key, item.Value);
		}
		Rebuild();
	}

	public void Rebuild()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (dataSource == null)
		{
			return;
		}
		int itemCount = dataSource.GetItemCount();
		object obj = (((Object)(object)OverrideContentRoot != (Object)null) ? ((object)OverrideContentRoot) : ((object)/*isinst with value type is only supported in some contexts*/));
		((RectTransform)obj).SetSizeWithCurrentAnchors((Axis)1, (float)(BlockHeight * itemCount - ItemSpacing + Padding.top + Padding.bottom));
		Rect rect = ScrollRect.viewport.rect;
		int num = Mathf.Max(2, Mathf.CeilToInt(((Rect)(ref rect)).height / (float)BlockHeight));
		int num2 = Mathf.FloorToInt((((RectTransform)obj).anchoredPosition.y - (float)Padding.top) / (float)BlockHeight);
		int num3 = num2 + num;
		RecycleOutOfRange(num2, num3);
		for (int i = num2; i <= num3; i++)
		{
			if (i >= 0 && i < itemCount)
			{
				BuildItem(i);
			}
		}
	}

	public void Update()
	{
		if (!(dataSource is IVisualUpdate visualUpdate))
		{
			return;
		}
		foreach (KeyValuePair<int, GameObject> item in ActivePool)
		{
			visualUpdate.OnVisualUpdate(item.Key, item.Value);
		}
	}

	private void RecycleOutOfRange(int startVisible, float endVisible)
	{
		int[] array = (from x in ActivePool.Keys
			where x < startVisible || (float)x > endVisible
			select (x)).ToArray();
		foreach (int key in array)
		{
			Recycle(key);
		}
	}

	private void Recycle(int key)
	{
		GameObject val = ActivePool[key];
		val.SetActive(false);
		ActivePool.Remove(key);
		InactivePool.Push(val);
	}

	private void BuildItem(int i)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (i >= 0 && !ActivePool.ContainsKey(i))
		{
			GameObject item = GetItem();
			item.SetActive(true);
			dataSource.SetItemData(i, item);
			Transform transform = item.transform;
			Transform obj = ((transform is RectTransform) ? transform : null);
			((RectTransform)obj).anchorMin = new Vector2(0f, 1f);
			((RectTransform)obj).anchorMax = new Vector2(1f, 1f);
			((RectTransform)obj).pivot = new Vector2(0.5f, 1f);
			((RectTransform)obj).offsetMin = new Vector2(0f, 0f);
			((RectTransform)obj).offsetMax = new Vector2(0f, (float)ItemHeight);
			((RectTransform)obj).sizeDelta = new Vector2((float)((Padding.left + Padding.right) * -1), (float)ItemHeight);
			((RectTransform)obj).anchoredPosition = new Vector2((float)(Padding.left - Padding.right) * 0.5f, (float)(-1 * (i * BlockHeight + Padding.top)));
			ActivePool[i] = item;
		}
	}

	private GameObject GetItem()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (InactivePool.Count == 0)
		{
			GameObject val = Object.Instantiate<GameObject>(SourceObject);
			val.transform.SetParent((Transform)(((Object)(object)OverrideContentRoot != (Object)null) ? ((object)OverrideContentRoot) : ((object)((Transform)ScrollRect.viewport).GetChild(0))), false);
			val.transform.localScale = Vector3.one;
			val.SetActive(false);
			InactivePool.Push(val);
		}
		return InactivePool.Pop();
	}
}
