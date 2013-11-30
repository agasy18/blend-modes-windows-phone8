#include "pch.h"
#include "DXExtension.h"

#include "DirectXHelper.h"

void GetSurfaceInfo( _In_ size_t width,
								  _In_ size_t height,
								  _In_ DXGI_FORMAT fmt,
								  _Out_opt_ size_t* outNumBytes,
								  _Out_opt_ size_t* outRowBytes,
								  _Out_opt_ size_t* outNumRows )
{
	size_t numBytes = 0;
	size_t rowBytes = 0;
	size_t numRows = 0;

	bool bc = false;
	bool packed  = false;
	size_t bcnumBytesPerBlock = 0;
	switch (fmt)
	{
	case DXGI_FORMAT_BC1_TYPELESS:
	case DXGI_FORMAT_BC1_UNORM:
	case DXGI_FORMAT_BC1_UNORM_SRGB:
	case DXGI_FORMAT_BC4_TYPELESS:
	case DXGI_FORMAT_BC4_UNORM:
	case DXGI_FORMAT_BC4_SNORM:
		bc=true;
		bcnumBytesPerBlock = 8;
		break;

	case DXGI_FORMAT_BC2_TYPELESS:
	case DXGI_FORMAT_BC2_UNORM:
	case DXGI_FORMAT_BC2_UNORM_SRGB:
	case DXGI_FORMAT_BC3_TYPELESS:
	case DXGI_FORMAT_BC3_UNORM:
	case DXGI_FORMAT_BC3_UNORM_SRGB:
	case DXGI_FORMAT_BC5_TYPELESS:
	case DXGI_FORMAT_BC5_UNORM:
	case DXGI_FORMAT_BC5_SNORM:
	case DXGI_FORMAT_BC6H_TYPELESS:
	case DXGI_FORMAT_BC6H_UF16:
	case DXGI_FORMAT_BC6H_SF16:
	case DXGI_FORMAT_BC7_TYPELESS:
	case DXGI_FORMAT_BC7_UNORM:
	case DXGI_FORMAT_BC7_UNORM_SRGB:
		bc = true;
		bcnumBytesPerBlock = 16;
		break;

	case DXGI_FORMAT_R8G8_B8G8_UNORM:
	case DXGI_FORMAT_G8R8_G8B8_UNORM:
		packed = true;
		break;
	}

	if (bc)
	{
		size_t numBlocksWide = 0;
		if (width > 0)
		{
			numBlocksWide = std::max<size_t>( 1, (width + 3) / 4 );
		}
		size_t numBlocksHigh = 0;
		if (height > 0)
		{
			numBlocksHigh = std::max<size_t>( 1, (height + 3) / 4 );
		}
		rowBytes = numBlocksWide * bcnumBytesPerBlock;
		numRows = numBlocksHigh;
	}
	else if (packed)
	{
		rowBytes = ( ( width + 1 ) >> 1 ) * 4;
		numRows = height;
	}
	else
	{
		size_t bpp = BitsPerPixel( fmt );
		rowBytes = ( width * bpp + 7 ) / 8; // round up to nearest byte
		numRows = height;
	}

	numBytes = rowBytes * numRows;
	if (outNumBytes)
	{
		*outNumBytes = numBytes;
	}
	if (outRowBytes)
	{
		*outRowBytes = rowBytes;
	}
	if (outNumRows)
	{
		*outNumRows = numRows;
	}
}

size_t   BitsPerPixel( _In_ DXGI_FORMAT fmt )
{
	switch( fmt )
	{
	case DXGI_FORMAT_R32G32B32A32_TYPELESS:
	case DXGI_FORMAT_R32G32B32A32_FLOAT:
	case DXGI_FORMAT_R32G32B32A32_UINT:
	case DXGI_FORMAT_R32G32B32A32_SINT:
		return 128;

	case DXGI_FORMAT_R32G32B32_TYPELESS:
	case DXGI_FORMAT_R32G32B32_FLOAT:
	case DXGI_FORMAT_R32G32B32_UINT:
	case DXGI_FORMAT_R32G32B32_SINT:
		return 96;

	case DXGI_FORMAT_R16G16B16A16_TYPELESS:
	case DXGI_FORMAT_R16G16B16A16_FLOAT:
	case DXGI_FORMAT_R16G16B16A16_UNORM:
	case DXGI_FORMAT_R16G16B16A16_UINT:
	case DXGI_FORMAT_R16G16B16A16_SNORM:
	case DXGI_FORMAT_R16G16B16A16_SINT:
	case DXGI_FORMAT_R32G32_TYPELESS:
	case DXGI_FORMAT_R32G32_FLOAT:
	case DXGI_FORMAT_R32G32_UINT:
	case DXGI_FORMAT_R32G32_SINT:
	case DXGI_FORMAT_R32G8X24_TYPELESS:
	case DXGI_FORMAT_D32_FLOAT_S8X24_UINT:
	case DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS:
	case DXGI_FORMAT_X32_TYPELESS_G8X24_UINT:
		return 64;

	case DXGI_FORMAT_R10G10B10A2_TYPELESS:
	case DXGI_FORMAT_R10G10B10A2_UNORM:
	case DXGI_FORMAT_R10G10B10A2_UINT:
	case DXGI_FORMAT_R11G11B10_FLOAT:
	case DXGI_FORMAT_R8G8B8A8_TYPELESS:
	case DXGI_FORMAT_R8G8B8A8_UNORM:
	case DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
	case DXGI_FORMAT_R8G8B8A8_UINT:
	case DXGI_FORMAT_R8G8B8A8_SNORM:
	case DXGI_FORMAT_R8G8B8A8_SINT:
	case DXGI_FORMAT_R16G16_TYPELESS:
	case DXGI_FORMAT_R16G16_FLOAT:
	case DXGI_FORMAT_R16G16_UNORM:
	case DXGI_FORMAT_R16G16_UINT:
	case DXGI_FORMAT_R16G16_SNORM:
	case DXGI_FORMAT_R16G16_SINT:
	case DXGI_FORMAT_R32_TYPELESS:
	case DXGI_FORMAT_D32_FLOAT:
	case DXGI_FORMAT_R32_FLOAT:
	case DXGI_FORMAT_R32_UINT:
	case DXGI_FORMAT_R32_SINT:
	case DXGI_FORMAT_R24G8_TYPELESS:
	case DXGI_FORMAT_D24_UNORM_S8_UINT:
	case DXGI_FORMAT_R24_UNORM_X8_TYPELESS:
	case DXGI_FORMAT_X24_TYPELESS_G8_UINT:
	case DXGI_FORMAT_R9G9B9E5_SHAREDEXP:
	case DXGI_FORMAT_R8G8_B8G8_UNORM:
	case DXGI_FORMAT_G8R8_G8B8_UNORM:
	case DXGI_FORMAT_B8G8R8A8_UNORM:
	case DXGI_FORMAT_B8G8R8X8_UNORM:
	case DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM:
	case DXGI_FORMAT_B8G8R8A8_TYPELESS:
	case DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
	case DXGI_FORMAT_B8G8R8X8_TYPELESS:
	case DXGI_FORMAT_B8G8R8X8_UNORM_SRGB:
		return 32;

	case DXGI_FORMAT_R8G8_TYPELESS:
	case DXGI_FORMAT_R8G8_UNORM:
	case DXGI_FORMAT_R8G8_UINT:
	case DXGI_FORMAT_R8G8_SNORM:
	case DXGI_FORMAT_R8G8_SINT:
	case DXGI_FORMAT_R16_TYPELESS:
	case DXGI_FORMAT_R16_FLOAT:
	case DXGI_FORMAT_D16_UNORM:
	case DXGI_FORMAT_R16_UNORM:
	case DXGI_FORMAT_R16_UINT:
	case DXGI_FORMAT_R16_SNORM:
	case DXGI_FORMAT_R16_SINT:
	case DXGI_FORMAT_B5G6R5_UNORM:
	case DXGI_FORMAT_B5G5R5A1_UNORM:

#ifdef DXGI_1_2_FORMATS
	case DXGI_FORMAT_B4G4R4A4_UNORM:
#endif
		return 16;

	case DXGI_FORMAT_R8_TYPELESS:
	case DXGI_FORMAT_R8_UNORM:
	case DXGI_FORMAT_R8_UINT:
	case DXGI_FORMAT_R8_SNORM:
	case DXGI_FORMAT_R8_SINT:
	case DXGI_FORMAT_A8_UNORM:
		return 8;

	case DXGI_FORMAT_R1_UNORM:
		return 1;

	case DXGI_FORMAT_BC1_TYPELESS:
	case DXGI_FORMAT_BC1_UNORM:
	case DXGI_FORMAT_BC1_UNORM_SRGB:
	case DXGI_FORMAT_BC4_TYPELESS:
	case DXGI_FORMAT_BC4_UNORM:
	case DXGI_FORMAT_BC4_SNORM:
		return 4;

	case DXGI_FORMAT_BC2_TYPELESS:
	case DXGI_FORMAT_BC2_UNORM:
	case DXGI_FORMAT_BC2_UNORM_SRGB:
	case DXGI_FORMAT_BC3_TYPELESS:
	case DXGI_FORMAT_BC3_UNORM:
	case DXGI_FORMAT_BC3_UNORM_SRGB:
	case DXGI_FORMAT_BC5_TYPELESS:
	case DXGI_FORMAT_BC5_UNORM:
	case DXGI_FORMAT_BC5_SNORM:
	case DXGI_FORMAT_BC6H_TYPELESS:
	case DXGI_FORMAT_BC6H_UF16:
	case DXGI_FORMAT_BC6H_SF16:
	case DXGI_FORMAT_BC7_TYPELESS:
	case DXGI_FORMAT_BC7_UNORM:
	case DXGI_FORMAT_BC7_UNORM_SRGB:
		return 8;

	default:
		return 0;
	}
}

bool IsCompressed( _In_ DXGI_FORMAT fmt )
{
	switch ( fmt )
	{
	case DXGI_FORMAT_BC1_TYPELESS:
	case DXGI_FORMAT_BC1_UNORM:
	case DXGI_FORMAT_BC1_UNORM_SRGB:
	case DXGI_FORMAT_BC2_TYPELESS:
	case DXGI_FORMAT_BC2_UNORM:
	case DXGI_FORMAT_BC2_UNORM_SRGB:
	case DXGI_FORMAT_BC3_TYPELESS:
	case DXGI_FORMAT_BC3_UNORM:
	case DXGI_FORMAT_BC3_UNORM_SRGB:
	case DXGI_FORMAT_BC4_TYPELESS:
	case DXGI_FORMAT_BC4_UNORM:
	case DXGI_FORMAT_BC4_SNORM:
	case DXGI_FORMAT_BC5_TYPELESS:
	case DXGI_FORMAT_BC5_UNORM:
	case DXGI_FORMAT_BC5_SNORM:
	case DXGI_FORMAT_BC6H_TYPELESS:
	case DXGI_FORMAT_BC6H_UF16:
	case DXGI_FORMAT_BC6H_SF16:
	case DXGI_FORMAT_BC7_TYPELESS:
	case DXGI_FORMAT_BC7_UNORM:
	case DXGI_FORMAT_BC7_UNORM_SRGB:
		return true;

	default:
		return false;
	}
}



void CreateTexture(_In_ ID3D11Device1* m_d3dDevice, int  *  buffer,int width,int height,ID3D11Texture2D ** m_Texture,ID3D11ShaderResourceView ** resView)
{
	Microsoft::WRL::ComPtr<ID3D11Texture2D> res_Texture;
	Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> res_TextureView;
	if (!(m_Texture||resView))
	{
		return;
	}
	if (m_Texture==nullptr)
	{
		m_Texture=&res_Texture;
	}
	if (resView==nullptr)
	{
		resView=&res_TextureView;
	}
	if (!buffer)
	{
		*m_Texture=nullptr;
		*resView=nullptr;
		return;
	}	
	CD3D11_TEXTURE2D_DESC textureDesc(
		DXGI_FORMAT_B8G8R8A8_UNORM,
		static_cast<UINT>(width),
		static_cast<UINT>(height),
		1,
		1,
		D3D11_BIND_SHADER_RESOURCE  
		);
	int pixelSize = sizeof(int); 
	D3D11_SUBRESOURCE_DATA data;
	data.pSysMem =buffer; // ARGBBuffer.data();
	data.SysMemPitch = pixelSize*width;
	data.SysMemSlicePitch =	pixelSize*width*height ;
	DX::ThrowIfFailed(
		m_d3dDevice->CreateTexture2D(
		&textureDesc,
		&data,
		m_Texture
		)
		);
	DX::ThrowIfFailed( m_d3dDevice->CreateShaderResourceView(*m_Texture,NULL,resView)); 
}


void CreateRenderTarget(_In_ ID3D11Device1* m_d3dDevice, int width,int height,ID3D11Texture2D ** texture,ID3D11RenderTargetView ** resView)
{
	Microsoft::WRL::ComPtr<ID3D11Texture2D> target;
	Microsoft::WRL::ComPtr<ID3D11RenderTargetView> targetView;
	if (texture==nullptr && resView == nullptr)
	{
		return;
	}
	if (texture==nullptr)
	{
		texture=target.GetAddressOf();
	}
	if (resView==nullptr)
	{
		resView=targetView.GetAddressOf();
	}

	CD3D11_TEXTURE2D_DESC renderTargetDesc(
		DXGI_FORMAT_B8G8R8A8_UNORM,
		static_cast<UINT>(width),
		static_cast<UINT>(height),
		1,
		1,
		D3D11_BIND_RENDER_TARGET
		);
	renderTargetDesc.MiscFlags =0 ;

	DX::ThrowIfFailed(
		m_d3dDevice->CreateTexture2D(
		&renderTargetDesc,
		nullptr,
		texture
		)
		);

	DX::ThrowIfFailed(
		m_d3dDevice->CreateRenderTargetView(
		*texture,
		nullptr,
		resView
		)
		);
}

Microsoft::WRL::ComPtr<ID3D11Texture2D> ResourceViewGetTexture2D( ID3D11View * resview )
{
	ComPtr<ID3D11Resource> resource;
	resview->GetResource(&resource);

	Microsoft::WRL::ComPtr<ID3D11Texture2D> buffer;
	DX::ThrowIfFailed(
		resource.As(&buffer)
		);
	return buffer;
}

void SaveTextureToBitmap(_In_ ID3D11Device1* m_d3dDevice, _In_ ID3D11DeviceContext1* m_d3dContext,ID3D11Texture2D * texture,int  * bitmap,int width,int height)
{
	D3D11_TEXTURE2D_DESC desc;
	texture->GetDesc(&desc);
	DX::ThrowIfFailed(desc.Width!=width||desc.Height!=height);

	Microsoft::WRL::ComPtr<ID3D11Texture2D> strongPtr = nullptr;
	if ((desc.CPUAccessFlags&D3D11_CPU_ACCESS_READ)==NULL)
	{
		strongPtr = CopyTexture(m_d3dDevice,m_d3dContext,texture,D3D11_CPU_ACCESS_READ);
		texture=strongPtr.Get();
	}

	size_t rowPitch, slicePitch, rowCount;
	GetSurfaceInfo( desc.Width, desc.Height, desc.Format, &slicePitch, &rowPitch, &rowCount );

	D3D11_MAPPED_SUBRESOURCE mapped;
	DX::ThrowIfFailed(m_d3dContext->Map(texture,0,D3D11_MAP_READ,NULL,&mapped));
	uint8_t* dptr=(uint8_t *)bitmap;
	auto sptr = reinterpret_cast<const uint8_t*>( mapped.pData );
	for( size_t h = 0; h < rowCount; ++h )
	{
		size_t msize = std::min<size_t>( rowPitch, mapped.RowPitch );
		memcpy_s( dptr, rowPitch, sptr, msize );
		sptr += mapped.RowPitch;
		dptr += rowPitch;
	}
	m_d3dContext->Unmap(texture,0);
}


Microsoft::WRL::ComPtr<ID3D11Texture2D> CopyTexture(_In_ ID3D11Device1* m_d3dDevice, _In_ ID3D11DeviceContext1* m_d3dContext,ID3D11Texture2D * res,D3D11_CPU_ACCESS_FLAG cpuFlag)
{
	D3D11_TEXTURE2D_DESC resviewTexD;
	res->GetDesc(&resviewTexD);
	if(cpuFlag != -1)
	{
		resviewTexD.CPUAccessFlags=cpuFlag;
		resviewTexD.MiscFlags&=D3D11_RESOURCE_MISC_TEXTURECUBE;
		resviewTexD.BindFlags=0;
		resviewTexD.Usage=D3D11_USAGE_STAGING;
	}
	Microsoft::WRL::ComPtr<ID3D11Texture2D> buf;

	DX::ThrowIfFailed(
		m_d3dDevice->CreateTexture2D(
		&resviewTexD,
		nullptr,
		&buf
		)
		);	 
	m_d3dContext->CopyResource(buf.Get(),res);
	return buf;
}

Microsoft::WRL::ComPtr<ID3D11Texture2D> CopyTexture(_In_ ID3D11Device1* m_d3dDevice, _In_ ID3D11DeviceContext1* m_d3dContext, ID3D11Texture2D * res,int x,int y, int w, int h,D3D11_CPU_ACCESS_FLAG cpuFlag)
{
	D3D11_TEXTURE2D_DESC resviewTexD;
	res->GetDesc(&resviewTexD);
	D3D11_BOX sourceRegion;
	sourceRegion.left =(unsigned int)MAX(0,x);
	if (w==-1)
	{
		sourceRegion.right =(unsigned int)resviewTexD.Width;
	}
	else
	{
		sourceRegion.right =(unsigned int)MIN(resviewTexD.Width,x+w);
	}
	if (h==-1)
	{
		sourceRegion.bottom =(unsigned int)resviewTexD.Height;
	}
	else
	{
		sourceRegion.bottom =(unsigned int)MIN(resviewTexD.Height,y+h);
	}	
	sourceRegion.top =(UINT)MAX(0,y);
	sourceRegion.front = 0;
	sourceRegion.back = 0;

	if (cpuFlag!=-1)
	{
		resviewTexD.CPUAccessFlags=cpuFlag;
		resviewTexD.MiscFlags&=D3D11_RESOURCE_MISC_TEXTURECUBE;
		resviewTexD.BindFlags=0;
		resviewTexD.Usage=D3D11_USAGE_STAGING;
	}

	resviewTexD.Width=MAX(w,(int)(sourceRegion.right-sourceRegion.left));
	resviewTexD.Height=MAX(h,(int)(sourceRegion.bottom - sourceRegion.top));

	Microsoft::WRL::ComPtr<ID3D11Texture2D> buf;

	DX::ThrowIfFailed(
		m_d3dDevice->CreateTexture2D(
		&resviewTexD,
		nullptr,
		&buf
		)
		);	 

	m_d3dContext->CopySubresourceRegion(buf.Get(),0,0,0,(unsigned int)sourceRegion.back,res,0,&sourceRegion);
	return buf;
}