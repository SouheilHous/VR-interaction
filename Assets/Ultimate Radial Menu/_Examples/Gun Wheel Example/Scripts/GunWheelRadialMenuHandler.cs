using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunWheelRadialMenuHandler : MonoBehaviour
{
	[System.Serializable]
	public class WeaponBase
	{
		public string name, description;
		public string key;

		public int damage = 10;
		public int roundsPerMinute = 100;

		public Sprite weaponIcon;

		// In a custom class with your own information, simply add a UltimateRadialButtonInfo variable to store information to send to the Ultimate Radial Menu.
		public UltimateRadialButtonInfo radialButtonInfo;
	}
	
	// Weapons List Info
	public WeaponBase[] LightWeapons;
	public WeaponBase[] HeavyWeapons;
	public WeaponBase[] UtilityWeapons;
	Dictionary<string, WeaponBase> WeaponDictionary = new Dictionary<string, WeaponBase>();

	// Pause Menu
	public GameObject backgroundPanel;
	public UltimateRadialMenu weaponRadialMenu;

	// Text For Displaying Information
	public Text radialMenuTitleText;
	public Text nameText, descriptionText, damageText, roundsPerMiinute;
	public Image gunIcon;

	// Calculations
	float horizontalInputMin, horizontalInputMax;
	enum CurrentMenu
	{
		LightWeapons,
		HeavyWeapons,
		UtilityWeapons
	}
	CurrentMenu currentMenu = CurrentMenu.LightWeapons;


	void Start ()
	{
		// Clear the radial buttons here in start so that the menu has no existing items.
		weaponRadialMenu.ClearRadialButtons();

		// For each of the light weapons...
		for( int i = 0; i < LightWeapons.Length; i++ )
		{
			// Assign the information inside the WeaponBase class to the radialButtonInfo to supply to the radial menu.
			LightWeapons[ i ].radialButtonInfo.name = LightWeapons[ i ].name;
			LightWeapons[ i ].radialButtonInfo.key = LightWeapons[ i ].key;
			LightWeapons[ i ].radialButtonInfo.description = LightWeapons[ i ].description;
			LightWeapons[ i ].radialButtonInfo.icon = LightWeapons[ i ].weaponIcon;

			// Add a radial button to the menu with the current Light Weapon information.
			weaponRadialMenu.AddRadialButton( ShowCurrentWeaponInfo, LightWeapons[ i ].radialButtonInfo );

			// Register this weapon to the dictionary with the current key.
			WeaponDictionary.Add( LightWeapons[ i ].key, LightWeapons[ i ] );
		}

		// For each of the Utility weapons...
		for( int i = 0; i < UtilityWeapons.Length; i++ )
		{
			// Assign the information inside the WeaponBase class to the radialButtonInfo to supply to the radial menu.
			UtilityWeapons[ i ].radialButtonInfo.name = UtilityWeapons[ i ].name;
			UtilityWeapons[ i ].radialButtonInfo.key = UtilityWeapons[ i ].key;
			UtilityWeapons[ i ].radialButtonInfo.description = UtilityWeapons[ i ].description;
			UtilityWeapons[ i ].radialButtonInfo.icon = UtilityWeapons[ i ].weaponIcon;

			// Register this weapon to the dictionary with the current key.
			WeaponDictionary.Add( UtilityWeapons[ i ].key, UtilityWeapons[ i ] );
		}

		// For each of the Heavy weapons...
		for( int i = 0; i < HeavyWeapons.Length; i++ )
		{
			// Assign the information inside the WeaponBase class to the radialButtonInfo to supply to the radial menu.
			HeavyWeapons[ i ].radialButtonInfo.name = HeavyWeapons[ i ].name;
			HeavyWeapons[ i ].radialButtonInfo.key = HeavyWeapons[ i ].key;
			HeavyWeapons[ i ].radialButtonInfo.description = HeavyWeapons[ i ].description;
			HeavyWeapons[ i ].radialButtonInfo.icon = HeavyWeapons[ i ].weaponIcon;

			// Register this weapon to the dictionary with the current key.
			WeaponDictionary.Add( HeavyWeapons[ i ].key, HeavyWeapons[ i ] );
		}

		// Subscribe to the On Enabled and On Disabled functions of the radial menu.
		weaponRadialMenu.OnRadialMenuEnabled += OnRadialMenuEnabled;
		weaponRadialMenu.OnRadialMenuDisabled += OnRadialMenuDisabled;

		// Turn off the background panel game object.
		backgroundPanel.SetActive( false );

		// Configure the horizontal input information for calculations.
		horizontalInputMin = weaponRadialMenu.BasePosition.x - ( weaponRadialMenu.GetComponent<RectTransform>().sizeDelta.x / 2 );
		horizontalInputMax = weaponRadialMenu.BasePosition.x + ( weaponRadialMenu.GetComponent<RectTransform>().sizeDelta.x / 2 );
	}

	void Update ()
	{
		// If the menu is active, but the input is not within range...
		if( weaponRadialMenu.RadialMenuActive && !weaponRadialMenu.InputInRange )
		{
			// And the left mouse button is caught down...
			if( Input.GetMouseButtonDown( 0 ) )
			{
				// Store the mouse position.
				Vector2 mousePosition = Input.mousePosition;

				// If the mouse position is less than the min input range, then the user is wanting to move to the menu on the left...
				if( mousePosition.x < horizontalInputMin )
				{
					// If the current menu is the Light Weapons...
					if( currentMenu == CurrentMenu.LightWeapons )
					{
						// Then set the current menu to Utility since it is the one that we are moving to.
						currentMenu = CurrentMenu.UtilityWeapons;

						// Populate the radial menu with the Utility Weapons.
						PopulateRadialMenu( UtilityWeapons, "Utility Weapons" );
					}
					// Else if the current menu is the Heavy Weapons...
					else if( currentMenu == CurrentMenu.HeavyWeapons )
					{
						// Set the current menu to Light Weapons.
						currentMenu = CurrentMenu.LightWeapons;

						// Populate the radial menu with the Light Weapons.
						PopulateRadialMenu( LightWeapons, "Light Weapons" );
					}
				}
				// Else if the mouse position is greater than the max input range, then the user wants to move to the right, so...
				else if( mousePosition.x > horizontalInputMax )
				{
					// If the current menu is the Light Weapons...
					if( currentMenu == CurrentMenu.LightWeapons )
					{
						// Set the current menu to heavy.
						currentMenu = CurrentMenu.HeavyWeapons;

						// Populate the radial menu with the heavy weapons.
						PopulateRadialMenu( HeavyWeapons, "Heavy Weapons" );
					}
					// Else if the current menu is the Utility weapons...
					else if( currentMenu == CurrentMenu.UtilityWeapons )
					{
						// Change the current menu to Light.
						currentMenu = CurrentMenu.LightWeapons;

						// Populate the radial menu with the light weapons.
						PopulateRadialMenu( LightWeapons, "Light Weapons" );
					}
				}
			}
		}
	}

	void PopulateRadialMenu ( WeaponBase[] weapons, string menuName )
	{
		// Since we want to populate the radial menu with new items, clear the current radial menu buttons.
		weaponRadialMenu.ClearRadialButtons();

		// Loop through each of the weapons in the array and add them to the radial menu.
		for( int i = 0; i < weapons.Length; i++ )
			weaponRadialMenu.AddRadialButton( ShowCurrentWeaponInfo, weapons[ i ].radialButtonInfo );

		// Change the title of the radial menu to the new menu name.
		radialMenuTitleText.text = menuName;
	}

	// This function is subscribed to the Ultimate Radial Menu, so when the radial menu is enabled it will call this function.
	void OnRadialMenuEnabled ()
	{
		// Set the background panel to visible.
		backgroundPanel.SetActive( true );
	}

	// This function will be called when the radial menu is disabled.
	void OnRadialMenuDisabled ()
	{
		// Disable the background panel.
		backgroundPanel.SetActive( false );

		// Reset the radial menu to the Light Weapons.
		currentMenu = CurrentMenu.LightWeapons;
		PopulateRadialMenu( LightWeapons, "Light Weapons" );
	}

	void ShowCurrentWeaponInfo ( string key )
	{
		// If the dictionary does not contain this key, then return.
		if( !WeaponDictionary.ContainsKey( key ) )
			return;

		// Update the information in the scene so that the user can see the values changing.
		nameText.text = WeaponDictionary[ key ].name;
		descriptionText.text = "Description:\n" + WeaponDictionary[ key ].description;
		damageText.text = "Damage: " + WeaponDictionary[ key ].damage.ToString();
		roundsPerMiinute.text = "Rounds Per Minute: " + WeaponDictionary[ key ].roundsPerMinute.ToString();
		gunIcon.sprite = WeaponDictionary[ key ].weaponIcon;
	}
}