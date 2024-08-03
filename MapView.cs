using System.Collections.Generic;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MapView : FacepunchBehaviour
{
	public RawImage mapImage;

	public Image cameraPositon;

	public ScrollRectEx scrollRect;

	public GameObject monumentMarkerContainer;

	public Transform clusterMarkerContainer;

	public GameObjectRef monumentMarkerPrefab;

	public GameObject missionMarkerContainer;

	public GameObjectRef missionMarkerPrefab;

	public Transform activeInteractionParent;

	public Transform localPlayerInterestPointRoot;

	public TeamMemberMapMarker[] teamPositions;

	public List<PointOfInterestMapMarker> PointOfInterestMarkers = null;

	public List<PointOfInterestMapMarker> TeamPointOfInterestMarkers = null;

	public List<PointOfInterestMapMarker> LocalPings = null;

	public List<PointOfInterestMapMarker> TeamPings = null;

	public GameObject PlayerDeathMarker = null;

	public List<SleepingBagMapMarker> SleepingBagMarkers = new List<SleepingBagMapMarker>();

	public List<SleepingBagClusterMapMarker> SleepingBagClusters = new List<SleepingBagClusterMapMarker>();

	[FormerlySerializedAs("TrainLayer")]
	public RawImage UndergroundLayer = null;

	public bool ShowGrid;

	public bool ShowPointOfInterestMarkers;

	public bool ShowDeathMarker = true;

	public bool ShowSleepingBags = true;

	public bool AllowSleepingBagDeletion = false;

	public bool ShowLocalPlayer = true;

	public bool ShowTeamMembers = true;

	public bool ShowTrainLayer = false;

	public bool ShowMissions = false;

	[FormerlySerializedAs("ShowTrainLayer")]
	public bool ShowUndergroundLayers = false;

	public bool MLRSMarkerMode = false;

	public RustImageButton LockButton;

	public RustImageButton OverworldButton;

	public RustImageButton TrainButton;

	public RustImageButton[] UnderwaterButtons;

	public RustImageButton DungeonButton;
}
