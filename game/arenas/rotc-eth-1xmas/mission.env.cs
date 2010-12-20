// Mission environment script
// This script is executed on the server

$ENV_ROOT = "game/arenas/rotc-eth-1xmas/";

//------------------------------------------------------------------------------
// Lights
//------------------------------------------------------------------------------

$sgLightEditor::lightDBPath = $ENV_ROOT @ "lights/";
$sgLightEditor::filterDBPath = $ENV_ROOT @ "filters/";
sgLoadDataBlocks($sgLightEditor::lightDBPath);
sgLoadDataBlocks($sgLightEditor::filterDBPath);

//------------------------------------------------------------------------------
// Material mappings
//------------------------------------------------------------------------------

%mapping = createMaterialMapping("eth1/SnowIce");
%mapping.sound = $MaterialMapping::Sound::Hard;
%mapping.color = "0.3 0.3 0.3 0.4 0.0";
%mapping.envmap = "share/skies/dragonmoon/dragonmoon_up 0.5";
%mapping.detail = "share/textures/eth1/detail1";

%mapping = createMaterialMapping("Snow");
%mapping.sound = $MaterialMapping::Sound::Snow;
%mapping.color = "0.3 0.3 0.3 0.4 0.0";

//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

function GameConnection::updateSkyColor(%this)
{
	cancel(%this.skyColorThread);
	%this.setSkyColor("1 1 1");
	%this.skyColorThread = %this.schedule(500, "updateSkyColor");
}


datablock PrecipitationData(MissionSnow)
{
   dropTexture = "share/textures/eth1/snowflakes";
   //splashTexture = "~/data/precipitation/water_splash";
   dropSize = 0.40;
   //splashSize = 0.2;
   useTrueBillboards = true;
   splashMS = 200;
};

datablock ShapeBaseImageData(RedCatLightImage)
{
	// basic item properties
	shapeFile = "share/shapes/eth1/santahat.red.dts";
	emap = true;

	// mount point & mount offset...
	mountPoint  = 3;
	offset = "0 -0.25 0.05";
	rotation = "1 0 0 -20";
	eyeOffset = "0 0 -9999"; // hide in 1st person
	
	// light properties...
	lightType = "ConstantLight";
	lightColor = "1 0 0";
	lightTime = 1000;
	lightRadius = 4;
	lightCastsShadows = false;
	lightAffectsShapes = false;

	stateName[0] = "DoNothing";
};

datablock ShapeBaseImageData(BlueCatLightImage)
{
	// basic item properties
	shapeFile = "share/shapes/eth1/santahat.blue.dts";
	emap = true;

	// mount point & mount offset...
	mountPoint  = 3;
	offset = "0 -0.25 0.05";
	rotation = "1 0 0 -20";
	eyeOffset = "0 0 -9999"; // hide in 1st person
	
	// light properties...
	lightType = "ConstantLight";
	lightColor = "0 0.5 1";
	lightTime = 1000;
	lightRadius = 4;
	lightCastsShadows = false;
	lightAffectsShapes = false;

	stateName[0] = "DoNothing";
};

datablock ParticleData(TextureDummyFile_share_shapes_eth1_red)
{
	textureName = "share/shapes/eth1/red";
};

datablock ParticleData(TextureDummyFile_share_shapes_eth1_blue)
{
	textureName = "share/shapes/eth1/blue";
};

datablock ParticleData(TextureDummyFile_share_shapes_eth1_white)
{
	textureName = "share/shapes/eth1/white";
};






