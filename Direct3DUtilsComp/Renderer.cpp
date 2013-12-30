#include "pch.h"
#include "Renderer.h"
#include <mutex>
#include <stdio.h>
#include <wrappers\corewrappers.h>
#include "DXExtension.h"

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

#define SP(x)((Sprite*)(x))

float spriteSize=0.5;
static const Vertex spriteVertexes[] = 
{					
	Vertex(-spriteSize,spriteSize, 0, 0,0),
	Vertex(spriteSize, spriteSize, 0, 1,0),	
	Vertex(-spriteSize, -spriteSize, 0, 0,1),
	Vertex(spriteSize, -spriteSize, 0, 1,1)
};

Renderer::Renderer() 
{
	LogFormat("Loading Renderer");
}





void Renderer::CreateDeviceResources()
{
	Direct3DBase::CreateDeviceResources();
	ConfigBlendModes();

	auto loadVSTask = DX::ReadDataAsync("SimpleSpriteVertexShader.cso");
	auto loadPSTask = DX::ReadDataAsync("SimpleSpritePixelShader.cso");

	auto createVSTask = loadVSTask.then([this](Platform::Array<byte>^ fileData) {
		DX::ThrowIfFailed(
			m_d3dDevice->CreateVertexShader(
			fileData->Data,
			fileData->Length,
			nullptr,
			&m_vertexShader
			)
			);

		const D3D11_INPUT_ELEMENT_DESC vertexDesc[] = 
		{
			{ "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
			{ "TEXCOORD",    0, DXGI_FORMAT_R32G32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
		};

		DX::ThrowIfFailed(
			m_d3dDevice->CreateInputLayout(
			vertexDesc,
			ARRAYSIZE(vertexDesc),
			fileData->Data,
			fileData->Length,
			&m_inputLayout
			)
			);
		
		CD3D11_BUFFER_DESC constantVSBufferDesc(sizeof(ModelViewProjectionConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_d3dDevice->CreateBuffer(
			&constantVSBufferDesc,
			nullptr,
			&m_VSconstantBuffer
			)
			);
	});

	auto createPSTask = loadPSTask.then([this](Platform::Array<byte>^ fileData) {
		DX::ThrowIfFailed(
			m_d3dDevice->CreatePixelShader(
			fileData->Data,
			fileData->Length,
			nullptr,
			&m_pixelShader
			)
			);

		CD3D11_BUFFER_DESC constantPSBufferDesc(sizeof(PSConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_d3dDevice->CreateBuffer(
			&constantPSBufferDesc,
			nullptr,
			&m_PSconstantBuffer
			)
			);
	});

	auto createCubeTask = (createPSTask && createVSTask).then([this] () {
		CreateVertexBuffer();
	});

	createCubeTask.then([this] () {CreateSampler();

		m_loadingComplete = true;
	});
}

void Renderer::CreateWindowSizeDependentResources()
{
	Direct3DBase::CreateWindowSizeDependentResources();
	CD3D11_TEXTURE2D_DESC blendDestTextureDesc(
		DXGI_FORMAT_B8G8R8A8_UNORM,
		static_cast<UINT>(m_renderTargetSize.Width),
		static_cast<UINT>(m_renderTargetSize.Height),
		1,
		1,
		D3D11_BIND_SHADER_RESOURCE
		);
	blendDestTextureDesc.MiscFlags =0 ;

	// Allocate a 2-D surface as the render target buffer.
	DX::ThrowIfFailed(
		m_d3dDevice->CreateTexture2D(
		&blendDestTextureDesc,
		nullptr,
		&m_blendDestTexture
		)
		);
	DX::ThrowIfFailed( m_d3dDevice->CreateShaderResourceView(m_blendDestTexture.Get(),NULL,&m_blendDestResourceView)); 

	
	float aspectRatio = m_windowBounds.Width / m_windowBounds.Height;
	float fovAngleY = 70.0f * XM_PI / 180.0f;
	if (aspectRatio < 1.0f)
	{
		fovAngleY /= aspectRatio;
	}
	XMStoreFloat4x4(
		&m_VSconstantBufferData.projection,
		XMMatrixTranspose(
			XMMatrixOrthographicRH(
			m_windowBounds.Width,
			m_windowBounds.Height,
				0.01f,
				100.0f
				)
			)
		);

	CreateViewport();
}

void Renderer::Update(float timeTotal, float timeDelta)
{
	
}



void Renderer::Render()
{
	const float midnightBlue[] = { 0, 0, 0, 1.000f };
	const float clearColor[] = { 0.0f, 0.0f, 0.0f, 0.000f };

	m_d3dContext->ClearRenderTargetView(
		m_renderTargetView.Get(),
		midnightBlue
		);

	m_d3dContext->OMSetRenderTargets(
		1,
		m_renderTargetView.GetAddressOf(),
		NULL
		);
	
	
	
	// Only draw the cube once it is loaded (loading is asynchronous).
	if (!m_loadingComplete)
	{
		return;
	}


	UINT stride = sizeof(Vertex);
	UINT offset = 0;
	m_d3dContext->IASetVertexBuffers(
		0,
		1,
		m_vertexBuffer.GetAddressOf(),
		&stride,
		&offset
		);

	m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

	m_d3dContext->VSSetShader(
		m_vertexShader.Get(),
		nullptr,
		0
		);	

	m_d3dContext->PSSetShader(
		m_pixelShader.Get(),
		nullptr,
		0
		);


	m_d3dContext->VSSetConstantBuffers(
		0,
		1,
		m_VSconstantBuffer.GetAddressOf()
	);
		m_d3dContext->PSSetConstantBuffers(
		0,
		1,
		m_PSconstantBuffer.GetAddressOf()
	);
	m_d3dContext->IASetInputLayout(m_inputLayout.Get());
	m_d3dContext->PSSetSamplers(0,1,CubesTexSamplerState.GetAddressOf());
	float zIndex = 0;
	m_d3dContext->RSSetState(CWcullMode.Get());
	for (auto iter : spriteVector)
	{		
		DrawSprite(iter,zIndex);
	}
	
}

ID3D11BlendState * Renderer::GetNativeBlendFunc(BlendMode blendmode)
{
	switch (blendmode)
	{
	case BlendMode::Normal:
		return AlphaBlenFunc.Get();
	default:
		return NULL;
		break;
	}
};

void Renderer::DrawSprite(Sprite * iter,float & zIndex)
{
	__declspec(align(16)) auto modelMatrix=iter->modelMatrix;
	__declspec(align(16)) auto modelMatrixT= XMMatrixTranspose(XMMatrixMultiply(modelMatrix,XMMatrixTranslation(0.0f,0.0f,zIndex)));
	zIndex+=0.01f;
	XMStoreFloat4x4(&m_VSconstantBufferData.model,modelMatrixT);
	ID3D11BlendState * dxblendFiunc=AlphaBlenFunc.Get();
	auto desBlendMode=iter->blendMode;
	ID3D11ShaderResourceView* blendTexResView=iter->blandTextureBmpInfo.ResView.Get();	
	m_VSconstantBufferData.info.x=0;

		if (blendTexResView==NULL)
	{		
		m_VSconstantBufferData.info.x=1;
		ID3D11BlendState *  ntvblendFinc=  GetNativeBlendFunc(desBlendMode);
		if (ntvblendFinc)
		{
			desBlendMode=BlendMode::None;
			dxblendFiunc =ntvblendFinc;
		}
		else
		{
			auto sRect = SpriteGetRect(iter);
			sRect.x*=backBufferDesc.Width;
			sRect.z*=backBufferDesc.Width;
			sRect.y*=backBufferDesc.Height;
			sRect.w*=backBufferDesc.Height;
			
			blendTexResView=m_blendDestResourceView.Get();
			D3D11_BOX sourceRegion;
			sourceRegion.left =(unsigned int)MAX(0,sRect.x);
			sourceRegion.right =(unsigned int)MIN(backBufferDesc.Width,sRect.x+sRect.z);
			sourceRegion.top =(UINT)MAX(0,sRect.y) ;
			sourceRegion.bottom =(unsigned int)MIN(backBufferDesc.Height,sRect.y+sRect.w);
			sourceRegion.front = 0;
			sourceRegion.back = 0;
			m_d3dContext->CopySubresourceRegion(m_blendDestTexture.Get(),0,(unsigned int)sourceRegion.left,(unsigned int)sourceRegion.top,(unsigned int)sourceRegion.back,m_renderTarget.Get(),0,&sourceRegion);
		}
	}

	ID3D11ShaderResourceView* shaderResources[]={iter->mainTextureBmpInfo.ResView.Get(),blendTexResView,iter->maskTextureBmpInfo.ResView.Get()};
	
	if (iter->drawMode==SpriteDrawMode::CopyMainTexture)
	{
		m_PSconstantBufferData.blendMode.y=(int)SpriteDrawMode::CopyMainTexture;
	}
	else if(iter->drawMode==SpriteDrawMode::CopyBlendTexture)
	{
		m_PSconstantBufferData.blendMode.y=(int)SpriteDrawMode::CopyBlendTexture;
		shaderResources[0]=0;
		shaderResources[2]=0;
		shaderResources[1]=m_blendDestResourceView.Get();
		m_VSconstantBufferData.info.x=2;
		dxblendFiunc=NoBlenFunc.Get();
	}
	else if(iter->drawMode==SpriteDrawMode::BlendMode )
	{
		m_PSconstantBufferData.blendMode.y=(int)SpriteDrawMode::BlendMode;
	}
	else if (iter->drawMode==SpriteDrawMode::Auto)
	{
		if (desBlendMode==BlendMode::None)
		{
			m_PSconstantBufferData.blendMode.y=(int)SpriteDrawMode::CopyMainTexture;
		}
		else
		{
			m_PSconstantBufferData.blendMode.y=(int)SpriteDrawMode::BlendMode;
		}
	}
	m_PSconstantBufferData.blendMode.z=iter->maskTextureBmpInfo.ResView.Get()!=nullptr;
	m_PSconstantBufferData.blendMode.w=(int)iter->fillMode;
	m_PSconstantBufferData.color = iter->color;
	m_PSconstantBufferData.floatInfo.x=iter->alpha;
	m_d3dContext->OMSetBlendState(dxblendFiunc, 0, 0xffffffff);	
	m_PSconstantBufferData.blendMode.x=(int)desBlendMode;
	m_d3dContext->PSSetShaderResources( 0, 3,shaderResources);
	m_d3dContext->UpdateSubresource(
		m_VSconstantBuffer.Get(),
		0,
		NULL,
		&m_VSconstantBufferData,
		0,
		0
		);

	m_d3dContext->UpdateSubresource(
		m_PSconstantBuffer.Get(),
		0,
		NULL,
		&m_PSconstantBufferData,
		0,
		0
		);
	m_d3dContext->Draw(4,0);
}

void Renderer::CreateSampler()
{
		D3D11_SAMPLER_DESC sampDesc;
		ZeroMemory( &sampDesc, sizeof(sampDesc) );
		sampDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
		sampDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
		sampDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
		sampDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
		sampDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
		sampDesc.MinLOD = 0;
		sampDesc.MaxLOD = D3D11_FLOAT32_MAX;
		m_d3dDevice->CreateSamplerState( &sampDesc, &CubesTexSamplerState);
}

void Renderer::CreateViewport()
{
	XMVECTOR eye = XMVectorSet(0, 0, 100, 0);
	XMVECTOR at = XMVectorSet(0, 0, 0, 0);
	XMVECTOR up = XMVectorSet(0, 1, 0, 0);

	XMStoreFloat4x4(&m_VSconstantBufferData.view, XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up)));
	//XMStoreFloat4x4(&m_VSconstantBufferData.model, XMMatrixTranspose(XMMatrixIdentity()));
}

void Renderer::ConfigBlendModes()
{

	//OutputPixel = ( SourceColor.rgba * SrcBlend ) __BlendOp__ ( DestColor.rgba * DestBlend )
	D3D11_BLEND_DESC blendDesc;
	ZeroMemory( &blendDesc, sizeof(blendDesc) );

	D3D11_RENDER_TARGET_BLEND_DESC alphaDes;
	ZeroMemory( &alphaDes, sizeof(alphaDes) );


	alphaDes.BlendEnable = true;
	alphaDes.SrcBlend = D3D11_BLEND_SRC_ALPHA;
	alphaDes.DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
	alphaDes.BlendOp = D3D11_BLEND_OP_ADD;
	alphaDes.SrcBlendAlpha = D3D11_BLEND_ONE;
	alphaDes.DestBlendAlpha = D3D11_BLEND_INV_SRC_ALPHA;
	alphaDes.BlendOpAlpha = D3D11_BLEND_OP_ADD;
	alphaDes.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;



	blendDesc.AlphaToCoverageEnable = false;
	blendDesc.RenderTarget[0] = alphaDes;	

	m_d3dDevice->CreateBlendState(&blendDesc, &AlphaBlenFunc);



	D3D11_RENDER_TARGET_BLEND_DESC rtbd;
	ZeroMemory( &rtbd, sizeof(rtbd) );

	ZeroMemory( &blendDesc, sizeof(blendDesc) );

	ZeroMemory( &rtbd, sizeof(rtbd) );


	rtbd.BlendEnable = true;
	rtbd.SrcBlend = D3D11_BLEND_ONE;
	rtbd.DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
	rtbd.BlendOp = D3D11_BLEND_OP_ADD;
	rtbd.SrcBlendAlpha = D3D11_BLEND_ONE;
	rtbd.DestBlendAlpha = D3D11_BLEND_INV_SRC_ALPHA;
	rtbd.BlendOpAlpha = D3D11_BLEND_OP_ADD;
	rtbd.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;



	blendDesc.AlphaToCoverageEnable = false;
	blendDesc.RenderTarget[0] = rtbd;

	m_d3dDevice->CreateBlendState(&blendDesc, &NoBlenFunc);



		//Screen

	ZeroMemory( &blendDesc, sizeof(blendDesc) );

	ZeroMemory( &rtbd, sizeof(rtbd) );


	rtbd.SrcBlend = D3D11_BLEND_DEST_COLOR;	
	rtbd.BlendOp = D3D11_BLEND_OP_SUBTRACT;
	rtbd.DestBlend = D3D11_BLEND_ONE;

	rtbd.SrcBlendAlpha = D3D11_BLEND_DEST_ALPHA;	
	rtbd.BlendOpAlpha = D3D11_BLEND_OP_SUBTRACT;
	rtbd.DestBlendAlpha = D3D11_BLEND_ONE;



	rtbd.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;



	blendDesc.AlphaToCoverageEnable = false;
	blendDesc.RenderTarget[0] = rtbd;

	ZeroMemory( &blendDesc, sizeof(blendDesc) );

	ZeroMemory( &rtbd, sizeof(rtbd) );

	rtbd.BlendEnable = true;


	rtbd.SrcBlend = D3D11_BLEND_ONE;	
	rtbd.BlendOp = D3D11_BLEND_OP_SUBTRACT;
	rtbd.DestBlend = D3D11_BLEND_ONE;

	rtbd.SrcBlendAlpha = D3D11_BLEND_ONE;	
	rtbd.BlendOpAlpha = D3D11_BLEND_OP_SUBTRACT;
	rtbd.DestBlendAlpha = D3D11_BLEND_ONE;

	rtbd.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

	blendDesc.AlphaToCoverageEnable = true;
	blendDesc.RenderTarget[1] = rtbd;

	m_d3dDevice->CreateBlendState(&blendDesc, &ScreenBlenFunc);

	//Rester

	D3D11_RASTERIZER_DESC cmdesc;
	ZeroMemory(&cmdesc, sizeof(D3D11_RASTERIZER_DESC));

	cmdesc.FillMode = D3D11_FILL_SOLID;
	cmdesc.CullMode = D3D11_CULL_NONE;
	cmdesc.DepthClipEnable = true;

	cmdesc.FrontCounterClockwise = false;
	m_d3dDevice->CreateRasterizerState(&cmdesc, &CWcullMode);




	
}

void Renderer::CreateVertexBuffer()
{

	D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
	vertexBufferData.pSysMem = spriteVertexes;
	vertexBufferData.SysMemPitch = 0;
	vertexBufferData.SysMemSlicePitch = 0;
	CD3D11_BUFFER_DESC vertexBufferDesc(sizeof(spriteVertexes), D3D11_BIND_VERTEX_BUFFER);
	DX::ThrowIfFailed(
		m_d3dDevice->CreateBuffer(
		&vertexBufferDesc,
		&vertexBufferData,
		&m_vertexBuffer
		)
		);
}

#pragma  region Sprite


void Renderer::SetAlpha(int id,float value)
{
	auto sp =SP(id);
	sp->alpha=value;
}
void Renderer::BringToFront(int id)
{
	auto sp =SP(id);
	spriteVector.erase(std::find(spriteVector.begin(),spriteVector.end(),sp));
	spriteVector.push_back(sp);
}

void Renderer::SpriteSetTransform(int id,float translateX=0, float translateY=0, float translateZ=0,float Rotation=0,float scaleX=1,float scaleY=1)
{

	float x=m_windowBounds.Width/2;
	float y=m_windowBounds.Height/2;
	auto sp= SP(id);
	sp->modelMatrix = 
		XMMatrixScaling(scaleX,scaleY,1)*
		XMMatrixRotationZ(-Rotation* XM_PI/180)*
		XMMatrixTranslation(translateX -x ,-translateY+y,translateZ/10);


}



void Renderer::SprieSetBlendMode(int id, int blend)
{
	SP(id)->blendMode=(BlendMode) blend;
}
void Renderer::SpriteSetFillMode(int id, int fillMode)
{
	SP(id)->fillMode =(SpriteFillMode) fillMode;
}




DirectX::XMFLOAT4 Renderer::SpriteGetRect(Sprite * sprite)
{
	//LogFormat("SpriteRect");
	XMFLOAT4 rect(0,0,-1,0);
	for (auto & spriteVx:spriteVertexes)
	{
		XMFLOAT4 vertexPoint(spriteVx.pos.x,spriteVx.pos.y,spriteVx.pos.z,1);
		auto vpos= SpriteCovertPointToWorld(sprite,&vertexPoint);
		vpos.y=(1.0f-vpos.y)/2.0f;
		vpos.x+=1.0f;
		vpos.x/=2.0f;
		if (rect.z==-1)
		{
			rect=XMFLOAT4(vpos.x,vpos.y,0,0);		
		}
		InsertPointToRect(&vpos,&rect);
	}
	//Float4(&rect,"rect");
	return rect;
}

void Renderer::InsertPointToRect(DirectX::XMFLOAT4 * point,DirectX::XMFLOAT4 * rect)
{
	auto x=rect->x;
	auto y=rect->y;
	rect->x=MIN(point->x,rect->x);
	rect->y=MIN(point->y,rect->y);
	rect->z+=x-rect->x;
	rect->w+=y-rect->y;
	rect->z=MAX(point->x-rect->x,rect->z);
	rect->w=MAX(point->y-rect->y,rect->w);
	//LogFloat4(point,"point");
	
}

DirectX::XMFLOAT4 Renderer::SpriteCovertPointToWorld(Sprite * sprite,XMFLOAT4 * point)
{
	XMVECTOR vector=DirectX::XMLoadFloat4(point);
	vector =  DirectX::XMVector4Transform(vector,sprite->modelMatrix);
	vector =  DirectX::XMVector4Transform(vector,(DirectX::XMLoadFloat4x4(&m_VSconstantBufferData.view)));
	vector =  DirectX::XMVector4Transform(vector,(DirectX::XMLoadFloat4x4(&m_VSconstantBufferData.projection)));
	DirectX::XMFLOAT4 res;
	DirectX::XMStoreFloat4(&res,vector);
	return res; 
}


void Renderer::SpriteGetRect(int id, float * x,float * y, float * w,float *h)
{
	auto a = SpriteGetRect(SP(id));
	*x=a.x;
	*y=a.y;
	*w=a.z;
	*h=a.w;
}


int Renderer::SpriteCreate(void)
{
	Sprite * sp= new Sprite();	
	spriteVector.push_back(sp);
	return (int)sp;
}

void Renderer::SpriteDelete(int id)
{
	delete SP(id);
	spriteVector.erase(std::find(spriteVector.begin(),spriteVector.end(),SP(id)));
}



void Renderer::SpriteCreateMainTexture(int id, int  *  buffer,int width,int height)
{
	auto sp= SP(id);
	sp->mainTextureBmpInfo.Connect(m_d3dDevice.Get(),buffer,width,height);
}


void Renderer::SpriteCreateBlendTexture( int id, int * buffer, int width, int height )
{
	auto sp=SP(id);
	sp->blandTextureBmpInfo.Connect(m_d3dDevice.Get(),buffer,width,height);
}


void Renderer::SpriteCreateMaskTexture( int id, int * buffer, int width, int height )
{
	auto sp=SP(id);
	sp->maskTextureBmpInfo.Connect(m_d3dDevice.Get(),buffer,width,height);
}

void Renderer::SaveToBitmap( int * bitmap,int x,int y,int width,int height,float sx,float sy )
{
	Microsoft::WRL::ComPtr<ID3D11Texture2D> target;
	Microsoft::WRL::ComPtr<ID3D11RenderTargetView> targetView;
	CreateRenderTarget(m_d3dDevice.Get(),m_renderTargetSize.Width*sx,m_renderTargetSize.Height*sy,target.GetAddressOf(),targetView.GetAddressOf());
	UpdateDevice(m_d3dDevice.Get(),m_d3dContext.Get(),targetView.Get());
	Render();
	auto tex=CopyTexture(m_d3dDevice.Get(),m_d3dContext.Get(), target.Get(),x*sx,y*sy,width*sx,height*sy,D3D11_CPU_ACCESS_READ);
	SaveTextureToBitmap(m_d3dDevice.Get(),m_d3dContext.Get(),tex.Get(),bitmap,width,height);
}

Renderer::~Renderer()
{
	for (auto i:spriteVector)
	{
		delete i;
	}
	spriteVector.clear();
}

void Renderer::Disconnect()
{
	for (auto i:spriteVector)
	{
		Disconnect(i);
	}
	m_loadingComplete =false;
	m_d3dDevice = nullptr;
	m_d3dContext = nullptr;
	 m_renderTargetSize.Width  = -1;
 m_renderTargetSize.Height = -1;
}

void Renderer::Initialize( _In_ ID3D11Device1* device )
{	
	m_loadingComplete =false;
	Direct3DBase::Initialize(device);
}

void Renderer::SetFillColor(int id, float red, float green, float blue, float alpha)
{
	auto & color = SP(id)->color;
	color.w = alpha;
	color.x = red;
	color.y = green;
	color.z = blue;
}

void Renderer::Disconnect(Sprite * spr)
{
	spr->mainTextureBmpInfo.Disconnect();
	spr->blandTextureBmpInfo.Disconnect();
	spr->maskTextureBmpInfo.Disconnect();
}