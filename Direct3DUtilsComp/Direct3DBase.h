#pragma once

#include "DirectXHelper.h"
//#define DX_DEBUG
// Helper class that initializes DirectX APIs for 3D rendering.
ref class Direct3DBase abstract
{
internal:
	Direct3DBase();

	virtual void Initialize(_In_ ID3D11Device1* device);
	virtual void CreateDeviceResources();
	virtual void UpdateDevice(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView);
	virtual void CreateWindowSizeDependentResources();
	virtual void UpdateForWindowSizeChange(float width, float height);
	virtual void Render() = 0;
protected private:
	// Direct3D Objects.
	Microsoft::WRL::ComPtr<ID3D11Device1> m_d3dDevice;
	Microsoft::WRL::ComPtr<ID3D11DeviceContext1> m_d3dContext;
	Microsoft::WRL::ComPtr<ID3D11Texture2D> m_renderTarget;
	Microsoft::WRL::ComPtr<ID3D11RenderTargetView> m_renderTargetView;
	D3D11_TEXTURE2D_DESC backBufferDesc;
	//Microsoft::WRL::ComPtr<ID3D11DepthStencilView> m_depthStencilView;
	

	// Cached renderer properties.
	Windows::Foundation::Size m_renderTargetSize;
	Windows::Foundation::Rect m_windowBounds;
#ifdef DX_DEBUG
	D3D_FEATURE_LEVEL m_featureLevel;
#endif
};