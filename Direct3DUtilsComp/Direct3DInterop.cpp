#include "pch.h"
#include "Direct3DInterop.h"
#include "Direct3DContentProvider.h"

using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Microsoft::WRL;
using namespace Windows::Phone::Graphics::Interop;
using namespace Windows::Phone::Input::Interop;

namespace Direct3DUtilsComp
{

Direct3DInterop::Direct3DInterop() :
	m_timer(ref new BasicTimer())
{
}

IDrawingSurfaceBackgroundContentProvider^ Direct3DInterop::CreateContentProvider()
{
	ComPtr<Direct3DContentProvider> provider = Make<Direct3DContentProvider>(this);
	return reinterpret_cast<IDrawingSurfaceBackgroundContentProvider^>(provider.Detach());
}

// IDrawingSurfaceManipulationHandler
void Direct3DInterop::SetManipulationHost(DrawingSurfaceManipulationHost^ manipulationHost)
{

}



int Direct3DUtilsComp::Direct3DInterop::SpriteCreate(void)
{
	return m_renderer->SpriteCreate();
}



HRESULT Direct3DInterop::Connect(_In_ IDrawingSurfaceRuntimeHostNative* host, _In_ ID3D11Device1* device)
{
	if (m_renderer==nullptr)
	{
		m_renderer = ref new Renderer();
	}
	m_renderer->Initialize(device);
	m_renderer->UpdateForWindowSizeChange(WindowBounds.Width, WindowBounds.Height);

	// Restart timer after renderer has finished initializing.
	m_timer->Reset();
	ConnectEvent();
	return S_OK;

}

void Direct3DInterop::Disconnect()
{
	
	m_renderer->Disconnect();
	DisconnectEvent();
	//m_renderer=nullptr;
}

HRESULT Direct3DInterop::PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Inout_ DrawingSurfaceSizeF* desiredRenderTargetSize)
{
	m_timer->Update();
	m_renderer->Update(m_timer->Total, m_timer->Delta);

	desiredRenderTargetSize->width = RenderResolution.Width;
	desiredRenderTargetSize->height = RenderResolution.Height;

	return S_OK;
}

HRESULT Direct3DInterop::Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView)
{
	m_renderer->UpdateDevice(device, context, renderTargetView);
	m_renderer->Render();

	RequestAdditionalFrame();

	return S_OK;
}

void Direct3DInterop::SpriteSetTransform(int id,float translateX, float translateY, float translateZ,float Rotation,float scaleX,float scaleY)
{
	m_renderer->SpriteSetTransform(id,translateX,translateY,translateZ,Rotation,scaleX,scaleY);
}

void Direct3DInterop::SpriteCreateMainTexture(int id, int  *  buffer,int width,int height)
{
	m_renderer->SpriteCreateMainTexture(id,buffer,width,height);
}
void Direct3DInterop::SpriteCreateBlendTexture(int id, int  *  buffer,int width,int height)
{
	m_renderer->SpriteCreateBlendTexture(id,buffer,width,height);
}
void Direct3DInterop::SpriteCreateMaskTexture(int id, int  *  buffer,int width,int height)
{
	m_renderer->SpriteCreateMaskTexture(id,buffer,width,height);
}
void Direct3DInterop::SpriteGetRect(int id, float * x,float * y, float * w,float *h)
{
	m_renderer->SpriteGetRect(id,x,y,w,h);
}

void Direct3DInterop::SprieSetBlendMode(int id, int blend)
{
	m_renderer->SprieSetBlendMode(id, blend);
}
void Direct3DInterop::SpriteSetFillMode(int id, int fill)
{
	m_renderer->SpriteSetFillMode(id,fill);	
}
void Direct3DInterop::SpriteDelete(int id)
{
	m_renderer->SpriteDelete(id);
}
void Direct3DInterop::BringToFront(int id)
{
	m_renderer->BringToFront(id);
}
void Direct3DInterop::SetAlpha(int id,float value)
{
	m_renderer->SetAlpha(id,value);
}
void Direct3DInterop::SaveToBitmap(int * bitmap,int imageWidth,int imageHeight,int x,int y,int width,int height)
{
	m_renderer->SaveToBitmap(bitmap,x,y,width,height,((float)imageWidth)/width,((float)imageHeight)/height);
}
void Direct3DInterop::SetFillColor(int id, float red, float green, float blue, float alpha)
{
	m_renderer->SetFillColor(id, red,green,blue,alpha);
}

}