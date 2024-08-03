using UnityEngine;

public class ChippyArcadeGame : BaseArcadeGame
{
	public ChippyMainCharacter mainChar;

	public SpriteArcadeEntity mainCharAim;

	public ChippyBoss currentBoss;

	public ChippyBoss[] bossPrefabs;

	public SpriteArcadeEntity mainMenuLogo;

	public Transform respawnPoint;

	public Vector2 mouseAim = new Vector2(0f, 1f);

	public TextArcadeEntity levelIndicator;

	public TextArcadeEntity gameOverIndicator;

	public TextArcadeEntity playGameButton;

	public TextArcadeEntity highScoresButton;

	public bool OnMainMenu = false;

	public bool GameActive = false;

	public int level = 0;

	public TextArcadeEntity[] scoreDisplays;

	public MenuButtonArcadeEntity[] mainMenuButtons;

	public int selectedButtonIndex = 0;

	public bool OnHighScores = false;
}
