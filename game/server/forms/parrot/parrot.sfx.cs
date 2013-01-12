//------------------------------------------------------------------------------
// Alux
// Copyright (C) 2013 Michael Goldener <mg@wasted.ch>
//------------------------------------------------------------------------------

datablock AudioProfile(FrmParrotEngineSound)
{
   filename = "share/sounds/rotc/slide3.wav";
   description = AudioDefaultLooping3D;
	preload = true;
};

datablock AudioProfile(FrmParrotExplosionSound)
{
   filename = "~/data/vehicles/FrmParrot/sound_explode.wav";
   description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(FrmParrotSoftImpactSound)
{
   filename = "~/data/vehicles/tank/sound_softimpact.wav";
   description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(FrmParrotHardImpactSound)
{
   filename = "~/data/vehicles/tank/sound_hardimpact.wav";
   description = AudioDefault3D;
	preload = true;
};
