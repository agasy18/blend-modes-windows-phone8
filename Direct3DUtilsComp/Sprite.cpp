#include "pch.h"
#include "Sprite.h"
#include <vector>
using namespace DirectX;
Sprite::Sprite()
{
	modelMatrix=XMMatrixIdentity();
	blendMode=BlendMode::Normal;
	drawMode=SpriteDrawMode::Auto;
	fillMode = SpriteFillMode::None;
	alpha = 1;
	
}



BitmapInfo::BitmapInfo():
	ResView(nullptr)
{}

void BitmapInfo::Disconnect()
{
	ResView=nullptr;
}


void BitmapInfo::Connect(ID3D11Device1 * device,int * bitmapData,int width,int heigth )
{
	if (bitmapData)
	{
		ResView= nullptr;
		CreateTexture(device,bitmapData,width,heigth,nullptr,ResView.GetAddressOf());
	}
}

BitmapInfo::~BitmapInfo()
{
	ResView = nullptr;
}






