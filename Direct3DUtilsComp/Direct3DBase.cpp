#include "pch.h"
#include "Direct3DBase.h"



using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;

// Constructor.
Direct3DBase::Direct3DBase()
{
}

// Initialize the Direct3D resources required to run.
void Direct3DBase::Initialize(_In_ ID3D11Device1* device)
{
	m_d3dDevice = device;
	CreateDeviceResources();
}

// These are the resources that depend on the device.
void Direct3DBase::CreateDeviceResources()
{
}

void Direct3DBase::UpdateDevice(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView)
{

#ifdef DX_DEBUG





	ComPtr<ID3D11Resource> renderTargetViewResource;
	renderTargetView->GetResource(&renderTargetViewResource);


	// Cache the rendertarget dimensions in our helper class for convenient use.

	static_cast<ID3D11Texture2D*>(renderTargetViewResource.Get())->GetDesc(&backBufferDesc);

	if (m_renderTargetSize.Width  != static_cast<float>(backBufferDesc.Width) ||
		m_renderTargetSize.Height != static_cast<float>(backBufferDesc.Height))
	{
		m_renderTargetSize.Width  = static_cast<float>(backBufferDesc.Width);
		m_renderTargetSize.Height = static_cast<float>(backBufferDesc.Height);
	}

	// This flag adds support for surfaces with a different color channel ordering
	// than the API default. It is required for compatibility with Direct2D.
	UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

#if defined(DX_DEBUG)
	// If the project is in a debug build, enable debugging via SDK Layers with this flag.
	creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

	// This array defines the set of DirectX hardware feature levels this app will support.
	// Note the ordering should be preserved.
	// Don't forget to declare your application's minimum required feature level in its
	// description.  All applications are assumed to support 9.1 unless otherwise stated.
	D3D_FEATURE_LEVEL featureLevels[] = 
	{
		D3D_FEATURE_LEVEL_11_1,
		D3D_FEATURE_LEVEL_11_0,
		D3D_FEATURE_LEVEL_10_1,
		D3D_FEATURE_LEVEL_10_0,
		D3D_FEATURE_LEVEL_9_3
	};

	// Create the Direct3D 11 API device object and a corresponding context.
	ComPtr<ID3D11Device> deviceX;
	ComPtr<ID3D11DeviceContext> contextX;
	DX::ThrowIfFailed(
		D3D11CreateDevice(
		nullptr, // Specify nullptr to use the default adapter.
		D3D_DRIVER_TYPE_HARDWARE,
		nullptr,
		creationFlags, // Set set debug and Direct2D compatibility flags.
		featureLevels, // List of feature levels this app can support.
		ARRAYSIZE(featureLevels),
		D3D11_SDK_VERSION, // Always set this to D3D11_SDK_VERSION.
		&deviceX, // Returns the Direct3D device created.
		&m_featureLevel, // Returns feature level of device created.
		&contextX // Returns the device immediate context.
		)
		);

	// Get the Direct3D 11.1 API device and context interfaces.
	DX::ThrowIfFailed(
		deviceX.As(&m_d3dDevice)
		);

	DX::ThrowIfFailed(
		contextX.As(&m_d3dContext)
		);
	CreateWindowSizeDependentResources();
#else


	m_d3dContext = context;
	m_renderTargetView = renderTargetView;

	if (m_d3dDevice.Get() != device)
	{
		m_d3dDevice = device;
		CreateDeviceResources();

		// Force call to CreateWindowSizeDependentResources.
		m_renderTargetSize.Width  = -1;
		m_renderTargetSize.Height = -1;
	}

	ComPtr<ID3D11Resource> renderTargetViewResource;
	m_renderTargetView->GetResource(&renderTargetViewResource);


	DX::ThrowIfFailed(
		renderTargetViewResource.As(&m_renderTarget)
		);

	// Cache the rendertarget dimensions in our helper class for convenient use.

	m_renderTarget->GetDesc(&backBufferDesc);

	if (m_renderTargetSize.Width  != static_cast<float>(backBufferDesc.Width) ||
		m_renderTargetSize.Height != static_cast<float>(backBufferDesc.Height))
	{
		m_renderTargetSize.Width  = static_cast<float>(backBufferDesc.Width);
		m_renderTargetSize.Height = static_cast<float>(backBufferDesc.Height);
		CreateWindowSizeDependentResources();
	}

	// Set the rendering viewport to target the entire window.
	CD3D11_VIEWPORT viewport(
		0.0f,
		0.0f,
		m_renderTargetSize.Width,
		m_renderTargetSize.Height
		);

	m_d3dContext->RSSetViewports(1, &viewport);
#endif // DX_DEBUG
}

// Allocate all memory resources that depend on the window size.
void Direct3DBase::CreateWindowSizeDependentResources()
{
#ifdef DX_DEBUG
	// Create a descriptor for the render target buffer.
	CD3D11_TEXTURE2D_DESC renderTargetDesc(
		DXGI_FORMAT_B8G8R8A8_UNORM,
		static_cast<UINT>(m_renderTargetSize.Width),
		static_cast<UINT>(m_renderTargetSize.Height),
		1,
		1,
		D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE
		);
	renderTargetDesc.MiscFlags = D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX | D3D11_RESOURCE_MISC_SHARED_NTHANDLE;

	// Allocate a 2-D surface as the render target buffer.
	DX::ThrowIfFailed(
		m_d3dDevice->CreateTexture2D(
		&renderTargetDesc,
		nullptr,
		&m_renderTarget
		)
		);

	DX::ThrowIfFailed(
		m_d3dDevice->CreateRenderTargetView(
		m_renderTarget.Get(),
		nullptr,
		&m_renderTargetView
		)
		);



	// Set the rendering viewport to target the entire window.
	CD3D11_VIEWPORT viewport(
		0.0f,
		0.0f,
		m_renderTargetSize.Width,
		m_renderTargetSize.Height
		);

	m_d3dContext->RSSetViewports(1, &viewport);
#endif
}



void Direct3DBase::UpdateForWindowSizeChange(float width, float height)
{
	m_windowBounds.Width  = width;
	m_windowBounds.Height = height;
}