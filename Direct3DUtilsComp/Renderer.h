#pragma once
#include "Sprite.h"
#include "Direct3DBase.h"
#include <list>


struct ModelViewProjectionConstantBuffer 
{
	DirectX::XMFLOAT4X4 model;
	DirectX::XMFLOAT4X4 view;
	DirectX::XMFLOAT4X4 projection;
	DirectX::XMINT4 info;//x - drawMode
};
struct PSConstantBuffer 
{
	DirectX::XMINT4 blendMode; // y - drawMode ,x - blendMode ,z - enable sprite mask ,w - fill blend mode
	DirectX::XMFLOAT4 floatInfo; //x - alpha
	DirectX::XMFLOAT4 color;
};

struct Vertex	//Overloaded Vertex Structure
{
	Vertex(){}
	Vertex(float x, float y, float z,
		float u, float v)
		: pos(x,y,z), texCoord(u, v){}

	DirectX::XMFLOAT3 pos;
	DirectX::XMFLOAT2 texCoord;
};


// This class renders a simple spinning cube.
ref class Renderer sealed : public Direct3DBase
{
public:
	Renderer();

	// Direct3DBase methods.
	virtual void CreateDeviceResources() override;
	
	virtual void CreateWindowSizeDependentResources() override;
	virtual void Render() override;	
	// Method for updating time-dependent objects.
	void Update(float timeTotal, float timeDelta);
  virtual	~Renderer();
  void Disconnect();
internal:
	virtual void Initialize(_In_ ID3D11Device1* device) override;
private:
	void Disconnect(Sprite * spr);
	void Connect(Sprite * spr);
	void CreateVertexBuffer();
	bool m_loadingComplete;
	void CreateSampler();
	void ConfigBlendModes();
	Microsoft::WRL::ComPtr<ID3D11InputLayout> m_inputLayout;
	Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexBuffer;
	Microsoft::WRL::ComPtr<ID3D11VertexShader> m_vertexShader;
	Microsoft::WRL::ComPtr<ID3D11PixelShader> m_pixelShader;
	Microsoft::WRL::ComPtr<ID3D11RasterizerState> CWcullMode;
	ModelViewProjectionConstantBuffer m_VSconstantBufferData;
	Microsoft::WRL::ComPtr<ID3D11Buffer>		m_VSconstantBuffer;
	PSConstantBuffer m_PSconstantBufferData;
	Microsoft::WRL::ComPtr<ID3D11Buffer>		m_PSconstantBuffer;
	std::vector<Sprite*> spriteVector;
	ID3D11BlendState * GetNativeBlendFunc(BlendMode blendmode);
	Microsoft::WRL::ComPtr<ID3D11BlendState> ScreenBlenFunc;
	Microsoft::WRL::ComPtr<ID3D11BlendState> AlphaBlenFunc;
	Microsoft::WRL::ComPtr<ID3D11BlendState> NoBlenFunc;
	Microsoft::WRL::ComPtr<ID3D11SamplerState> CubesTexSamplerState;
	void DrawSprite(Sprite * iter,float & zIndex);
	Microsoft::WRL::ComPtr<ID3D11Texture2D> m_blendDestTexture;
	Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_blendDestResourceView;

	void CreateViewport();
	DirectX::XMFLOAT4 SpriteGetRect(Sprite * sprite);
	DirectX::XMFLOAT4 SpriteCovertPointToWorld(Sprite * sprite,DirectX::XMFLOAT4 * point);
	void InsertPointToRect(DirectX::XMFLOAT4 * point,DirectX::XMFLOAT4 * rect);

public:
	int SpriteCreate(void);
	void SpriteDelete(int id);
	void SpriteTranslate(int id,float translateX, float translateY, float translateZ,float Rotation,float scaleX,float scaleY);
	void SizeChanged(int id,float width, float heght);
	void SpriteCreateMainTexture(int id, int  *  buffer,int width,int height);
	void SpriteCreateBlendTexture( int id, int * buffer, int width, int height );
	void SpriteCreateMaskTexture( int id, int * buffer, int width, int height );	
	void SpriteGetRect(int id, float * x,float * y, float * w,float *h);
	void SprieSetBlendMode(int id, int blend);
	void SpriteSetFillMode(int id, int blend);
	void SaveToBitmap(int * bitmap,int x,int y,int width,int height,float sx,float sy);
	void BringToFront(int id);
	void SetAlpha(int id,float value);
	void SetFillColor(int id, float red, float green, float blue, float alpha);
	int GetRemovedReason(){return m_d3dDevice->GetDeviceRemovedReason();}
};
