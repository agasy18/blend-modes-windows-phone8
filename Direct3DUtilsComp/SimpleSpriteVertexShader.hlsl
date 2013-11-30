cbuffer ModelViewProjectionConstantBuffer : register(b0)
{
	matrix model;
	matrix view;
	matrix projection;
	int4 info; //x - drawMode
};

struct VertexShaderInput
{
	float3 pos : POSITION;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 pos : SV_POSITION;
	float2 tex : TEXCOORD0;
	float2 dtexc : TEXCOORD1;
};

VertexShaderOutput main(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 pos = float4(input.pos, 1.0f);

	// Transform the vertex position into projected space.
	pos = mul(pos, model);
	pos = mul(pos, view);
	pos = mul(pos, projection);
	output.pos = pos;

	
	
	if(info.x==0)
	{	
		output.dtexc = input.tex;
	}
	else if (info.x==1)
	{
		float2 vpos=pos.xy;
		vpos.y=(1.0f-vpos.y)/2.0f;
		vpos.x+=1.0f;
		vpos.x/=2.0f;
		output.dtexc=vpos;
	}
	else if (info.x==2)
	{
		output.dtexc = input.tex;

		output.pos.x=input.pos.x>0.0f?1.0f:-1.0f;
		output.pos.y=input.pos.y>0.0f?1.0f:-1.0f;
	}
		output.tex = input.tex;


	return output;
}
