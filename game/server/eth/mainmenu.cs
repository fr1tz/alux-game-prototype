//------------------------------------------------------------------------------
// Alux Ethernet Prototype
// Copyright notices are in the file named COPYING.
//------------------------------------------------------------------------------

function showMainMenu(%client)
{
	%newtxt = om_init();

	%bg = %newtext @ ""; //\n\n\n<bitmap:share/ui/rotc/logo>

	%mutators = "";
	for(%i = 0; %i < getRecordCount($MissionInfo::MutatorDesc); %i++)
	{
		%line1 = getRecord($MissionInfo::MutatorDesc, %i);
		%mut1 = getWord(%line1, 0);
		for(%j = 0; %j < getWordCount($Pref::Server::Mutators); %j++)
		{
			%mut2 = getWord($Pref::Server::Mutators, %j);
			if(%mut2 $= %mut1)
			{
				%mutators = %mutators @ "\t\t * " @ %line1 @ "\n";
				break;
			}
		}
	}

	if(%mutators $= "")
		%mutators = "Mutators: None (Standard Game)\n";
	else
		%mutators = ""
			@ "<spush><color:FF8888>"
			@ "Game has been modified with the following mutators:\n"
			@ "<spop>" @ %mutators;

	%fg = %newtxt @
		om_head(%client, "Server Info") @
      "<just:center><spush><font:Arial:24>" @
		"\n\n<a:cmd Manual 1>Click here to read the manual!</a>\n" @
      "<spop>" @
//		"<just:left>\n" @
//		"Welcome to" SPC $Pref::Server::Name @
//		"<spop>\n\n" @
//		"<tab:15,30,130>Game:\n" @
//		"\t" @ $Server::MissionType SPC "<a:cmd News>Show Changelog</a>\n" @
//		"\t" @ %mutators @ "\n" @
//		"Environment:" SPC $Server::MissionName @ "\n\n" @
//		"<spush>" @ $Pref::Server::Info @ "<spop>\n\n" @
		"";

//	if(%client.loadingMission || %client.menu $= "mainmenu")
   if(false)
	{
		%fg = %fg @
			"If you're playing this arena for the first time, loading" SPC
			"might take\nsome time while the game downloads needed" SPC
			"art from the server.\nConsider using the time to read" SPC
			"the <a:cmd Help>Help section</a>.\n" @
			"";
	}

	%client.beginMenuText(%client.menu $= "mainmenu");
	%client.addMenuText(%fg, 1);
	%client.addMenuText(%bg, 2);
	%client.endMenuText();
}
