#pragma once

#include "pch.h"
#include "BasicTimer.h"
#include "Renderer.h"
#include <DrawingSurfaceNative.h>
#include "Sprite.h"

namespace Direct3DUtilsComp
{

public delegate void RequestAdditionalFrameHandler();

public delegate void ConnectionEventHandler();

[Windows::Foundation::Metadata::WebHostHidden]
public ref class Direct3DInterop sealed : public Windows::Phone::Input::Interop::IDrawingSurfaceManipulationHandler
{
public:
	

	Direct3DInterop();

	Windows::Phone::Graphics::Interop::IDrawingSurfaceBackgroundContentProvider^ CreateContentProvider();

	// IDrawingSurfaceManipulationHandler
	virtual void SetManipulationHost(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ manipulationHost);

	event ConnectionEventHandler^ ConnectEvent;
	event ConnectionEventHandler^ DisconnectEvent;
	event RequestAdditionalFrameHandler^ RequestAdditionalFrame;


	property Windows::Foundation::Size WindowBounds;
	property Windows::Foundation::Size NativeResolution;
	property Windows::Foundation::Size RenderResolution;

internal:
	HRESULT Connect(_In_ IDrawingSurfaceRuntimeHostNative* host, _In_ ID3D11Device1* device);
	void Disconnect();

	HRESULT PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Inout_ DrawingSurfaceSizeF* desiredRenderTargetSize);
	HRESULT Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView);

private:
	Renderer^ m_renderer;
	BasicTimer^ m_timer;
public:
	int SpriteCreate(void);
	void SpriteTranslate(int id,float translateX, float translateY, float translateZ,float Rotation,float scaleX,float scaleY);
	void SizeChanged(int id,float width, float heght);
	void SpriteCreateMainTexture(int id, int  *  buffer,int width,int height);
	void SpriteCreateBlendTexture(int id, int  *  buffer,int width,int height);
	void SpriteCreateMaskTexture(int id, int  *  buffer,int width,int height);
	void SpriteGetRect(int id, int * x,int * y, int * w,int *h);
	void SprieSetBlendMode(int id, int blend);
	void SpriteDelete(int id);
	void BringToFront(int id);
	void SetAlpha(int id,float value);
	void SpriteSetFillMode(int id, int fill);
	void SetFillColor(int id, float red, float green, float blue, float alpha);
	///x,y,width,height - rect , the final size of image
	void SaveToBitmap(int * bitmap,int imageWidth,int imageHeight,int x,int y,int width,int height);
	int GetRemovedReason() {return m_renderer->GetRemovedReason();}
};


}
