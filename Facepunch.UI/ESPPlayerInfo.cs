using Rust.UI;
using TMPro;
using UnityEngine;

namespace Facepunch.UI;

public class ESPPlayerInfo : MonoBehaviour
{
	public Vector3 WorldOffset;

	public RustText Text;

	public TextMeshProUGUI[] TextElements;

	public RustIcon Loading;

	public GameObject ClanElement;

	public RustText ClanText;

	public CanvasGroup group;

	public Gradient gradientNormal;

	public Gradient gradientTeam;

	private static Color TeamColor = new Color(0.6660359f, 127f / 136f, 0.1922578f, 0.9411765f);

	private static Color ClanColor = new Color(67f / 85f, 0.1176471f, 0.8470588f, 1f);

	private static Color AllyColor = new Color(0.06074228f, 0.6085457f, 105f / 106f, 0.9372549f);

	private static Color EnemyColor = new Color(0.6980392f, 0.2039216f, 0.003921569f, 1f);

	public QueryVis visCheck;

	public BasePlayer Entity { get; set; }
}
