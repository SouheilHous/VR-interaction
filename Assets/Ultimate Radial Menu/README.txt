||/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/||
||-------------------------------------------------------------------------------------------||
||Thank you for downloading the Ultimate Radial Menu UnityPackage from the Unity Asset Store!||
||-------------------------------------------------------------------------------------------||
||\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\||

First off, please view the Ultimate Radial Menu Introduction video on YouTube to get a quick start on the basics of how
to use the Ultimate Radial Menu. 

VIDEO LINK: https://youtu.be/n9Mt9vCfmf4

Also, here is a link to the online documentation, which has extensive documentation of the classes, as well as helpful
tutorials of how to understand and get the Ultimate Radial Menu working in your project.

ONLINE DOCUMENTATION: https://www.tankandhealerstudio.com/ultimate-radial-menu_documentation.html

If you are not able to watch the video or read the docs, please read on!

The Ultimate Radial Menu has been built from the ground up with being easy to use and customize to make it work the way
that you want. However, that being said, the Ultimate Radial Menu Asset is a beast of a menu, and can be a bit tricky to
understand how to use at first. To begin, let's take a look at the Ultimate Radial Menu inspector. After that we will go
over how to reference the Ultimate Radial Menu in your custom scripts. If you have to use the restroom, do it now,
because this is gonna get intense!

Let's begin!

  -----------------
// THE INSPECTOR //
-----------------
The Ultimate Radial Menu Inspector is divided up into 4 different sections: Radial Menu Positioning, Radial Menu Settings,
Radial Button Settings, and Script Reference. Here is a quick overview of the sections.

Radial Menu Positioning
	The Radial Menu Positioning section is where all of the variables are located that take of sizing and positioning the
radial menu on the screen. It handles the size of the buttons, the radius of the menu, and your input settings for your
menu.

Radial Menu Settings
	Here is where you will find options that affect the radial menu as a whole. Things like color of the buttons, how the
radial menu is enabled and disabled visually on the screen, and any text that is displayed in the center of the menu.

Radial Button Settings
	Ah, now here is where we get to the fun stuff! The Radial Button Settings section handles all of the individual
information about each button. You can find options for things like: icons, text, creating and deleting buttons, and more!

Script Reference
	This section provides example code that can be copied and pasted into your own scripts! Please keep in mind that you
will need to change some of the generated code to your own functions and variables, but we'll go deeper into that later.

And thats all the sections on the Ultimate Radial Menu inspector! To learn more about each variable and option in the
inspector, please hover your cursor over the option and read the tooltip. This should help explain what each variable 
is used for.

  --------------------
// HOW TO IMPLEMENT //
--------------------
To understand how to implement the Ultimate Radial Menu into your scripts, first let us talk about how it actually works
to see how we can best implement the radial menu. We will be going over the Callback System that the radial menu uses, as
well as a certain class that you will be using to send information to the radial button in order to update the information
that is displayed on the radial buttons. Below are the topics mentioned.

Callback System
	The Callback System that is used by the radial menu is used to send information to your scripts about which button has
been interacted with so that you can call the method or code to execute the desired effect for the user. When you get to
implementing the code, you will see a parameter named: ButtonCallback. This is where you will pass the function that you
want the button to call when being interacted with. For example, let's say you want to use the radial menu as a pause menu.
Likely, in your pause menu script, you have a function named: PauseGame(), or something similar. When subscribing to the
radial pause button, you will want to pass your PauseGame function as the ButtonCallback parameter. Then, when the user
clicks on the Pause Button on the radial menu, the radial button will can the PauseGame function, therefore pausing your
game.

UltimateRadialButtonInfo Class
	The UltimateRadialButtonInfo class is public and will be used inside of your own custom scripts. It is used just like
any other variable inside of your own scripts, but has a custom property drawer so that the information is a little easier
to work with. This class is what you will send to the Ultimate Radial Menu to update the information like: Name, Description,
Key, ID, Icon, and so forth. After sending in your UltimateRadialButtonInfo to the radial menu, it will then have access
to a few functions that you can use to keep information updated, without referencing back to the Ultimate Radial Menu.
For example, let continue on the example above with using the radial menu as a pause menu. In your variables, you will
want to have a public UltimateRadialButtonInfo pauseButtonInfo; variable. Then, after customizing the variable in the
inspector you will want to past it to the radial menu. Below is some example code to help show how it works:

public UltimateRadialButtonInfo pauseButtonInfo;

void Start ()
{
	// Call the UpdateRadialButton function on the PauseMenu radial menu, and pass in the PauseGame function as the ButtonCallback
	// and then the pauseButtonInfo as the new button information to the first index of the pause menu buttons.
	UltimateRadialMenu.UpdateRadialButton( "PauseMenu ", 0, PauseGame, pauseButtonInfo );
}

After this code, the radial menu's first index button will be your pause button, and will call the PauseGame function when
being interacted with. We are not done though! The UltimateRadialButtonInfo class has so much more functionality than first
meets the eye! After passing in the pauseButtonInfo to the radial menu, it has actually be authorized control over the
radial button that it has been assigned to, giving you access to more useful functions to keep the button updated! Let's
discuss one more example of how to update the radial button using this class! In the same scenario, inside your PauseGame()
function, now you can update the icon of the button by simply using the pauseButtonInfo class! Let's see how:

void PauseGame ()
{
	// Here is where your custom logic would be to pause the game, but this is just a simple way to pause your game.

	// If the timescale is 1, then the game is playing right now...
	if( Time.timeScale == 1.0f )
	{
		// So set the timescale to 0 to pause the game.
		Time.timeScale = 0.0f;

		// Since the game is now paused, update the icon to being a play button, instead of pause.
		pauseButtonInfo.UpdateIcon( playButtonIcon );

		// Update the text of the button to say "Play" now instead of pause.
		pauseButtonInfo.UpdateText( "Play" );
	}
	// Else the timescale is not 1, and therefore is paused...
	else
	{
		// So set the timescale to 1 to play the game again.
		Time.timeScale = 1.0f;

		// Since the game is playing again, update the icon back to being a pause button.
		pauseButtonInfo.UpdateIcon( pauseButtonIcon );

		// Update the text to now say "Pause" instead of play.
		pauseButtonInfo.UpdateText( "Pause" );
	}
}

As you can see, the pauseButtonInfo can now be used to update information about the button that it has been assigned. For
a full list of the functions available, please see the ONLINE DOCUMENTATION linked above.

Again, we want to thank you for downloading the Ultimate Radial Menu. Hopefully the above information made sense, and you
are ready to get out there and implement the Ultimate Radial Menu into your project. However, if you need a little more
guidance, please check out our YouTube tutorials on the Ultimate Radial Menu and of course check out the ONLINE DOCUMENTATION.
Both of these are linked above. If none of those help you with your issue, pleas don't hesitate to contact us at:
tankandhealerstudio@outlook.com and we will try to assist you as much as we can. Tank & Healer Studio has got your back!

Happy Game Making,
	Tank & Healer Studio