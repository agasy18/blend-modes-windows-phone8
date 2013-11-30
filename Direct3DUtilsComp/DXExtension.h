#pragma  once
#include "pch.h"
using Microsoft::WRL::ComPtr;

size_t   BitsPerPixel( _In_ DXGI_FORMAT fmt );

void GetSurfaceInfo( _In_ size_t width,	
					_In_ size_t height,
					_In_ DXGI_FORMAT fmt,
					_Out_opt_ size_t* outNumBytes,
					_Out_opt_ size_t* outRowBytes,
					_Out_opt_ size_t* outNumRows );

bool IsCompressed( _In_ DXGI_FORMAT fmt );

void CreateTexture(_In_ ID3D11Device1* m_d3dDevice, 
				   _In_ int  *  buffer,
				   _In_ int width,
				   _In_ int height,
				   _Out_opt_ ID3D11Texture2D ** m_Texture,
				   _Out_opt_ ID3D11ShaderResourceView ** resView);

void CreateRenderTarget(_In_ ID3D11Device1* m_d3dDevice,
						_In_ int width,
						_In_ int height,
						_Out_opt_ ID3D11Texture2D ** texture,
						_Out_opt_ ID3D11RenderTargetView ** resView);

Microsoft::WRL::ComPtr<ID3D11Texture2D> ResourceViewGetTexture2D(_In_ ID3D11View * resview );

void SaveTextureToBitmap(_In_ ID3D11Device1* m_d3dDevice,
						 _In_ ID3D11DeviceContext1* m_d3dContext,
						 _In_ ID3D11Texture2D * texture,
						 _Out_ int  * bitmap,
						 _In_ int width,
						 _In_ int height);

Microsoft::WRL::ComPtr<ID3D11Texture2D> CopyTexture(_In_ ID3D11Device1* m_d3dDevice,
													_In_ ID3D11DeviceContext1* m_d3dContext,
													_In_ ID3D11Texture2D * res,
													_In_opt_ D3D11_CPU_ACCESS_FLAG cpuFlag=(D3D11_CPU_ACCESS_FLAG)-1);

Microsoft::WRL::ComPtr<ID3D11Texture2D> CopyTexture(_In_ ID3D11Device1* m_d3dDevice,
													_In_ ID3D11DeviceContext1* m_d3dContext,
													_In_ ID3D11Texture2D * res,
													_In_ int x,
													_In_ int y,
													_In_opt_ int w=-1,
													_In_opt_ int h=-1,
													_In_opt_ D3D11_CPU_ACCESS_FLAG cpuFlag=(D3D11_CPU_ACCESS_FLAG)-1);