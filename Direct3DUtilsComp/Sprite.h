#include "Direct3DBase.h"
#include <d3d11.h>
#include <DirectXMath.h>
#include "DXExtension.h"
#pragma once

class BitmapInfo 
{
	public:
	BitmapInfo();
	void Disconnect();
	void Connect(ID3D11Device1 * device,int * bitmapData,int width,int heigth );
	Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> ResView;
    virtual	~BitmapInfo();
};

 enum BlendMode
    {
        BlendModeNormal = 1,
        BlendModeMultiply = 2,
        BlendModeScreen = 3,
        BlendModeOverlay = 4,
        BlendModeDarken = 5,
        BlendModeLighten = 6,
        BlendModeColorDodge = 7,
        BlendModeColorBurn = 8,
        BlendModeSoftLight = 9,
        BlendModeHardLight = 10,
        BlendModeDifference = 11,
        BlendModeExclusion = 12,
        /*BlendModeHue,
        BlendModeSaturation,
        BlendModeColor,
        BlendModeLuminosity,*/
        BlendModeClear = 17,
        BlendModeCopy = 18,
        BlendModeSourceIn = 19,
        BlendModeSourceOut = 20,
        BlendModeSourceAtop = 21,
        BlendModeDestinationOver = 22,
        BlendModeDestinationIn = 23,
        BlendModeDestinationOut = 24,
        BlendModeDestinationAtop = 25,
        BlendModeXOR = 26,
        BlendModePlusDarker = 27,
        BlendModePlusLighter = 28,
		
    };


 enum SpriteDrawMode
 {	
	SpriteDrawModeAuto=-1,
	SpriteDrawModeCopyMainTexture,
	SpriteDrawModeCopyBlendTexture,
	SpriteDrawModeBlendMode,	
 };

 class Sprite 
{	
public:
	Sprite();
	int blendMode;
	int drawMode;
	float alpha;
	int fillMode;
	__declspec(align(16)) DirectX::XMMATRIX modelMatrix;
	BitmapInfo mainTextureBmpInfo;
	BitmapInfo blandTextureBmpInfo;
	BitmapInfo maskTextureBmpInfo;	
	DirectX::XMFLOAT4 color;
 };