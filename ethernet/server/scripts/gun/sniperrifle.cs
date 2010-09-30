//------------------------------------------------------------------------------
// Revenge Of The Cats: Ethernet
// Copyright (C) 2008, mEthLab Interactive
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Revenge Of The Cats - sniperrifle.cs
// Code for the sniper rifle
//------------------------------------------------------------------------------

exec("./sniperrifle.sfx.cs");
exec("./sniperrifle.projectile.cs");

//------------------------------------------------------------------------------

datablock ShapeBaseImageData(HolsteredSniperRifleImage)
{
	// basic item properties
	shapeFile = "~/data/weapons/sniperrifle/image_holstered.dts";
	emap = true;

	// mount point & mount offset...
	mountPoint  = 2;
	offset = "0 0 0";

	stateName[0] = "DoNothing";
};

//------------------------------------------------------------------------------
// weapon image which does all the work...
// (images do not normally exist in the world, they can only
// be mounted on ShapeBase objects)

datablock ShapeBaseImageData(RedSniperRifleImage)
{
	// add the WeaponImage namespace as a parent
	className = WeaponImage;

	// basic item properties
	shapeFile = "~/data/weapons/blaster/image.red.dts";
	emap = true;

	// mount point & mount offset...
	mountPoint  = 0;
	offset      = "0 0 0";
	rotation    = "0 0 0";
	eyeOffset   = "0.275 -0.25 -0.2";
	eyeRotation = "0 0 0";

	// Adjust firing vector to eye's LOS point?
	correctMuzzleVector = true;

	// Use energy for ammo?
	usesEnergy = true;
	minEnergy = 75;
	
    // charging...
    minCharge = 0.4;
    
	projectile = RedSniperProjectile;
    missile = RedSniperMissile;
    
	// targeting...
    targetingMask = $TargetingMask::Heat;
	targetingMaxDist = 10000;
    followTarget = true;
	
	// script fields...
	iconId = 9;
	specialWeapon = true;
	armThread = "holdrifle";  // armThread to use when holding this weapon
	crosshair = "sniperrifle"; // crosshair to display when holding this weapon
 
	//-------------------------------------------------
	// image states...
	//
		// preactivation...
		stateName[0]                     = "Preactivate";
		stateTransitionOnAmmo[0]         = "Activate";
		stateTransitionOnNoAmmo[0]       = "NoAmmo";

		// when mounted...
		stateName[1]                     = "Activate";
		stateTransitionOnTimeout[1]      = "Ready";
		stateTimeoutValue[1]             = 0.5;
		stateSequence[1]                 = "idle";

		// ready, just waiting for the trigger...
		stateName[2]                     = "Ready";
		stateTransitionOnNoAmmo[2]       = "NoAmmo";
		stateTransitionOnNotLoaded[2]    = "Disabled";
		stateTransitionOnTriggerDown[2]  = "Charge";
		stateArmThread[2]                = "holdrifle";
		stateSequence[2]                 = "idle";
		stateScript[2]                   = "onReady";
		stateSpin[2]                     = "FullSpin";
  
		// charge...
		stateName[3]                     = "Charge";
		stateTransitionOnTriggerUp[3]    = "CheckFire";
		//stateTransitionOnNoAmmo[3]       = "NoAmmo";
		stateTarget[3]                   = true;
		stateCharge[3]                   = true;
		stateAllowImageChange[3]         = false;
		stateArmThread[3]                = "aimrifle";
		stateSound[3]                    = SniperPowerUpSound;
		stateSequence[3]                 = "charge";
		stateScript[3]                   = "onCharge";
  
		// check fire...
		stateName[4]                     = "CheckFire";
		stateTransitionOnCharged[4]      = "Fire";
		stateTransitionOnNotCharged[4]   = "Ready";
		//stateFire[4]                     = true;
		stateTarget[4]                   = true;
		stateAllowImageChange[4]         = false;

		// fire!...
		stateName[5]                     = "Fire";
		stateTransitionOnTimeout[5]      = "AfterFire";
		stateTimeoutValue[5]             = 0;
		stateFire[5]                     = true;
		stateFireProjectile[5]           = RedSniperProjectile;
		stateRecoil[5]                   = NoRecoil;
		stateAllowImageChange[5]         = false;
		stateEjectShell[5]               = true;
		stateArmThread[5]                = "aimrifle";
		stateSequence[5]                 = "fire";
		stateScript[5]                   = "onFire";

		// after fire...
		stateName[8]                     = "AfterFire";
		stateTransitionOnTriggerUp[8]    = "KeepAiming";

		// keep aiming...
		stateName[9]                     = "KeepAiming";
		stateTransitionOnNoAmmo[9]       = "NoAmmo";
		stateTransitionOnTriggerDown[9]  = "Charge";
		stateTransitionOnTimeout[9]      = "Ready";
		stateTransitionOnNotLoaded[9]    = "Disabled";
		stateWaitForTimeout[9]           = false;
		stateTimeoutValue[9]             = 2.00;

		// no ammo...
		stateName[10]                    = "NoAmmo";
        stateTransitionOnTriggerDown[10] = "DryFire";
		stateTransitionOnAmmo[10]        = "Ready";
		stateSequence[10]                = "idle";
		stateTimeoutValue[10]            = 0.50;
		stateScript[10]                  = "onNoAmmo";

        // dry fire...
		stateName[11]                    = "DryFire";
		stateTransitionOnTriggerUp[11]   = "NoAmmo";
		stateTransitionOnAmmo[11]        = "Ready";
		stateSound[11]                   = WeaponEmptySound;
		stateSequence[11]                = "idle";

		// disabled...
		stateName[12]                    = "Disabled";
		stateTransitionOnLoaded[12]      = "Ready";
		stateAllowImageChange[12]        = false;
		stateSequence[12]                = "idle";
	//
	// ...end of image states
	//-------------------------------------------------
};

function RedSniperRifleImage::onMount(%this, %obj, %slot)
{
    Parent::onMount(%this, %obj, %slot);
}

function RedSniperRifleImage::onUnmount(%this, %obj, %slot)
{
    Parent::onUnmount(%this, %obj, %slot);
    %obj.setSniping(false);
}

function RedSniperRifleImage::onReady(%this, %obj, %slot)
{
	error("onReady");
    %obj.setSniping(false);
}

function RedSniperRifleImage::onCharge(%this, %obj, %slot)
{
	error("onCharge");
    %obj.sniperTarget = "";
    %obj.setSniping(true);
}

function RedSniperRifleImage::onFire(%this, %obj, %slot)
{
	error("onFire");
	%obj.playAudio(1, SniperRifleFireSound);
}

function RedSniperRifleImage::onNoAmmo(%this, %obj, %slot)
{
	error("onNoAmmo");
    %obj.setSniping(false);
}

//------------------------------------------------------------------------------

datablock ShapeBaseImageData(BlueSniperRifleImage : RedSniperRifleImage)
{
	shapeFile = "~/data/weapons/missilelauncher/image.blue.dts";

	projectile = BlueSniperProjectile;
    missile = BlueSniperMissile;

	stateFireProjectile[5] = BlueSniperProjectile;
	stateEmitter[6] = BlueMissileLauncherFireEmitter;
	stateEmitter[7] = BlueMissileLauncherFireEmitter;
};

function BlueSniperRifleImage::onMount(%this, %obj, %slot)
{
	RedSniperRifleImage::onMount(%this, %obj, %slot);
}

function BlueSniperRifleImage::onUnmount(%this, %obj, %slot)
{
	RedSniperRifleImage::onUnmount(%this, %obj, %slot);
}

function BlueSniperRifleImage::onReady(%this, %obj, %slot)
{
    RedSniperRifleImage::onReady(%this, %obj, %slot);
}

function BlueSniperRifleImage::onCharge(%this, %obj, %slot)
{
    RedSniperRifleImage::onCharge(%this, %obj, %slot);
}

function BlueSniperRifleImage::onFire(%this, %obj, %slot)
{
	RedSniperRifleImage::onFire(%this, %obj, %slot);
}

function BlueSniperRifleImage::onNoAmmo(%this, %obj, %slot)
{
    RedSniperRifleImage::onNoAmmo(%this, %obj, %slot);
}
