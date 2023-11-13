using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineConvarController : PlayableAsset, ITimelineClipAsset
{
	public string convarName = string.Empty;

	public TimelineConvarPlayable template = new TimelineConvarPlayable();

	public ClipCaps clipCaps => (ClipCaps)2;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ScriptPlayable<TimelineConvarPlayable> val = ScriptPlayable<TimelineConvarPlayable>.Create(graph, template, 0);
		TimelineConvarPlayable behaviour = val.GetBehaviour();
		behaviour.convar = convarName;
		return ScriptPlayable<TimelineConvarPlayable>.op_Implicit(val);
	}
}
