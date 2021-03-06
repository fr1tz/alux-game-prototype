//------------------------------------------------------------------------------
// Alux Ethernet Prototype
// Copyright notices are in the file named COPYING.
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Revenge Of The Cats - aiPlayer.cs
// code for (currently stupid) practice ai opponents
//------------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// AIPlayer callbacks
// The AIPlayer class implements the following callbacks:
//
//	 PlayerData::onStuck(%this,%obj)
//	 PlayerData::onUnStuck(%this,%obj)
//	 PlayerData::onStop(%this,%obj)
//	 PlayerData::onMove(%this,%obj)
//	 PlayerData::onReachDestination(%this,%obj)
//	 PlayerData::onTargetEnterLOS(%this,%obj)
//	 PlayerData::onTargetExitLOS(%this,%obj)
//	 PlayerData::onAdd(%this,%obj)
//
// Since the AIPlayer doesn't implement it's own datablock, these callbacks
// all take place in the PlayerData namespace.
//-----------------------------------------------------------------------------

function aiAdd(%teamid, %weaponNum)
{
	if( !isObject($aiPlayers) )
	{
		$aiPlayers = new Array();
		MissionCleanup.add($aiPlayers);
	}
	
	if( !isObject($aiPlayersPseudoClient) )
	{
		$aiPlayersPseudoClient = new ScriptObject();
		$aiPlayersPseudoClient.handicap = 1.0;
		GameConnection::defaultLoadout($aiPlayersPseudoClient);
		GameConnection::updateLoadout($aiPlayersPseudoClient);
		MissionCleanup.add($aiPlayersPseudoClient);
	}
	

	%nameadd = "_" @ $aiPlayers.count();
	if(isObject($aiPlayers)) {
		%nameadd = "_" @ $aiPlayers.count();
	}

	%spawnSphere = pickSpawnSphere(%teamid);
	
	if($Game::GameType == $Game::GridWars)
	{
		if(%teamid == 1)
			%playerData = RedInfantryCat;
		else
			%playerData = BlueInfantryCat;
	}
	else if($Game::GameType == $Game::Infantry)
	{
		if(%teamid == 1)
			%playerData = RedInfantryCat;
		else
			%playerData = BlueInfantryCat;
	}
	else
	{
		if(%teamid == 1)
			%playerData = FrmSoldier;
		else
			%playerData = FrmSoldier;
	}

	%player = new AiPlayer() {
		dataBlock = %playerData;
		client = $aiPlayersPseudoClient;
		path = "";
		teamId = %teamid;
	};
	MissionCleanup.add(%player);
   %player.loadoutCode = "4 0 0 0" SPC %weaponNum;
   %player.ammo[0] = 999999;
   %player.ammo[1] = 999999;

	%pos = getRandomHorizontalPos(%spawnSphere.position,%spawnSphere.radius);
	%player.setShapeName("wayne" @ %nameadd);
	%player.setTransform(%pos);

	%player.aiWeapon = %weaponNum;
   if(%weaponNum == 3 || %weaponNum == 4)
   	%player.aiCharge = 1100;
   else
   	%player.aiCharge = 100;

	$aiPlayers.push_back("",%player);
	$aiPlayersPseudoClient.weapons[0] = %weaponNum;
	$aiPlayersPseudoClient.numWeapons = 1;

	%player.useWeapon(1);

	return %player;
}

//-----
// called by user
//-----

function aiAddRed(%weaponNum)
{
	%player = aiAdd(1, %weaponNum);
	return %player;
}

function aiAddBlue(%weaponNum)
{
	%player = aiAdd(2, %weaponNum);
	return %player;
}

function aiAddBoth(%weaponNum)
{
	%player = aiAdd(1, %weaponNum);
 	%player = aiAdd(2, %weaponNum);
}

function aiAddMultiple(%count, %weaponNum, %team)
{
   for(%i=0; %i<%count; %i++)
   {
      %wn = %weaponNum;
      if(%wn $= "")
         %wn = getRandom(5)+1;
      if(%team $= "")
      {
         aiAdd(1, %wn);
         aiAdd(2, %wn);
      }
      else
         aiAdd(%team, %wn);
   }
}

function aiStartMove() {
	for( %i = 0; %i < $aiPlayers.count(); %i++ ) {
		xxx_aiStartMove($aiPlayers.getValue(%i));
	}
}
function aiStartFire() {
	for( %i = 0; %i < $aiPlayers.count(); %i++ ) {
		xxx_aiStartFire($aiPlayers.getValue(%i));
	}
}
function aiStartFight() {
	for( %i = 0; %i < $aiPlayers.count(); %i++ ) {
		xxx_aiStartMove($aiPlayers.getValue(%i));
		xxx_aiStartFire($aiPlayers.getValue(%i));
	}
}
function aiKill() {
	for( %i = 0; %i < $aiPlayers.count(); %i++ ) {
		%player = $aiPlayers.getValue(%i);
		%player.kill();
	}
	$aiPlayers.empty();
}

function aiWayneStopFire()
{

}

function aiTransformInto(%what)
{
	for( %i = 0; %i < $aiPlayers.count(); %i++ )
	{
		%player = $aiPlayers.getValue(%i);
		Team2Scout::transformInto(0,%player,%what);
	}
}


//-----
// called non-interactively
//-----

function xxx_aiStartMove(%player)
{
	%player.updateMove = schedule(1000,%player,"xxx_aiUpdateMove",%player);
}

function xxx_aiStartFire(%player)
{
	%player.setImageLoaded(0,true);
	%player.targetUpdateThread = schedule(100,%player,"xxx_aiUpdateTarget",%player);
	%player.fireThread = schedule((getRandom(3)+1)*1000,%player,"xxx_aiFire",%player);
}

function xxx_aiFire(%player)
{
	%target = %player.getAimObject();
 
   if(%player.getEnergyLevel() == 0)
      %player.reloadStart();
 
	if(isObject(%target))
	{
		%x = getRandom(3)+1;
		%y = getRandom(3)+1;
		%z = getRandom(3)+1;

	  %enemypos = %player.getAimLocation();
	  %mypos = %player.getPosition();
	  %dist = VectorDist(%mypos, %enemypos);

	  %z = %dist/30;

		%offset = %x SPC %y SPC %z;

		%player.setAimObject(%target, %offset);
		%player.setImageTrigger(0,true);
	}
	
	%player.fireReleaseThread = schedule(%player.aiCharge,%player,xxx_aiFireRelease,%player);
}

function xxx_aiFireRelease(%player)
{
	%player.setImageTrigger(0,false);
	%player.fireThread = schedule(getRandom(1000),%player,xxx_aiFire,%player);

    // try to throw disc
	%player.setImageTrigger(3, true);
    %player.fireReleaseThread = %player.schedule(0, setImageTrigger, 3, false);
}

function xxx_aiUpdateTarget(%player)
{
	%target = 0; //$aiTarget;

	%position = %player.getPosition();
	%radius = 500;

   %typeMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
	InitContainerRadiusSearch(%position, %radius, %typeMask);
	while ((%targetObject = containerSearchNext()) != 0)
	{
		if( %targetObject.teamId > 0
		 && %targetObject.getDamageState() $= "Enabled"
		 && %targetObject.teamId != %player.teamId )
		{
			%target = %targetObject;
			break;
		}
	}
	
	if(%target != 0)
		%player.setAimObject(%target, "0 0 1");
	
	%player.targetUpdateThread = schedule(2500,%player,xxx_aiUpdateTarget,%player);
}

function xxx_aiFindMoveDestination(%player)
{
   %player.zMoveDestination = "";
   %pos = %player.getPosition();
   %dist = 1;
   %dest = "";
	InitContainerRadiusSearch(%pos, %dist, $TypeMasks::TacticalSurfaceObjectType);
   %zone = containerSearchNext();
   if(%zone $= "")
      return;
      
   if(%zone.getTeamId() == %player.getTeamId())
      %dest = %zone.zNeighbour[%player.getTeamId() == 1 ? 1 : 0];
      
   //error(%zone SPC "->" SPC %dest);
      
   //error(%pos SPC "->" SPC %dest.getPosition());
   %player.zMoveDestination = %dest;
}

function xxx_aiUpdateMove(%player)
{
//	%player.moveThread = schedule((getRandom(1)+1)*1000,%player,xxx_aiUpdateMove,%player);
	%player.moveThread = schedule(500,%player,xxx_aiUpdateMove,%player);
 
   %currPos = %player.getPosition();

   %findNewMoveDestination = false;
   if(!isObject(%player.zMoveDestination))
   {
      %findNewMoveDestination = true;
   }
   else
   {
      InitContainerRadiusSearch(%currPos, 1, $TypeMasks::TacticalSurfaceObjectType);
      %zone = containerSearchNext();
      if(%zone == %player.zMoveDestination && %zone.getTeamId() == %player.getTeamId())
         %findNewMoveDestination = true;
   }

   if(%findNewMoveDestination)
   {
      xxx_aiFindMoveDestination(%player);
      if(!isObject(%player.zMoveDestination))
         return;
   }

   %targetPos = %player.zMoveDestination.getPosition();
   //%targetPos = %player.getAimObject().getPosition();
   //%targetPos = "-45 89 20";
   %targetVec = VectorNormalize(VectorSub(%targetPos, %currPos));
   %newPos = VectorAdd(%currPos, VectorScale(%targetVec, 10));

   %player.setMoveDestination(%currPos);
   %n = 2;
   while(%n-- > 0)
   {
      %dest = getTerrainLevel(%newPos);
      //error(%dest);
      InitContainerRadiusSearch(%dest, 1, $TypeMasks::TacticalSurfaceObjectType);
      %obj = containerSearchNext();
      //error(%obj);
      //error(%obj SPC "->" SPC %obj.aiIgnore);
		if(%obj == 0 || %obj.aiIgnore) continue;

	   %heightdiff = getWord(%currPos,2) - getWord(%dest,2);
      //error(%heightdiff);
	   if(%heightdiff < 100) {
		   %player.setMoveDestination(%dest);
         break;
	   }
   }
}
