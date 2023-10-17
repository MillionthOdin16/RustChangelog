using System.Collections.Generic;
using UnityEngine;

public class ClientIOLine : BaseMonoBehaviour
{
	public RendererLOD _lod;

	public LineRenderer _line;

	public Material directionalMaterial;

	public Material defaultMaterial;

	public IOEntity.IOType lineType = IOEntity.IOType.Electric;

	public static List<ClientIOLine> _allLines = new List<ClientIOLine>();

	public WireTool.WireColour colour = WireTool.WireColour.Default;

	public IOEntity ownerIOEnt = null;
}
