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

 enum class BlendMode
    {
		None = 0,
        Normal = 1,
        Multiply = 2,
        Screen = 3,
        Overlay = 4,
        Darken = 5,
        Lighten = 6,
        ColorDodge = 7,
        ColorBurn = 8,
        SoftLight = 9,
        HardLight = 10,
        Difference = 11,
        Exclusion = 12,
        Clear = 17,
        Copy = 18,
        SourceIn = 19,
        SourceOut = 20,
        SourceAtop = 21,
        DestinationOver = 22,
        DestinationIn = 23,
        DestinationOut = 24,
        DestinationAtop = 25,
        XOR = 26,
        PlusDarker = 27,
        PlusLighter = 28,
		
    };


 enum class SpriteDrawMode
 {	
	Auto=-1,
	CopyMainTexture,
	CopyBlendTexture,
	BlendMode,	
 };
 enum class SpriteFillMode
 {	
	 None = -1,
	 Foreground = 0,
	 Background = 1
 };

 struct  Sprite 
{	
	Sprite();
	BlendMode blendMode;
	SpriteDrawMode drawMode;
	float alpha;
	SpriteFillMode fillMode;
	__declspec(align(16)) DirectX::XMMATRIX modelMatrix;
	BitmapInfo mainTextureBmpInfo;
	BitmapInfo blandTextureBmpInfo;
	BitmapInfo maskTextureBmpInfo;	
	DirectX::XMFLOAT4 color;
 };