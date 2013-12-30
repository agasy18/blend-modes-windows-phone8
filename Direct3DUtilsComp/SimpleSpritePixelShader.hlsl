Texture2D shaderTexture[3];
SamplerState SampleType ;

cbuffer PSConst : register(b0)
{
	int4 blendModeInfo; // y - drawMode ,x - blendMode  z- enableMask
	float4 floatInfo; //x - alpha
	float4 color;
};

struct PixelShaderInput
{
	float4 pos : SV_POSITION;
	float2 tex : TEXCOORD0;
	float2 dtexc : TEXCOORD1;
};

float4 main(PixelShaderInput input) : SV_TARGET
{	
	float4 textureColor =shaderTexture[0].Sample(SampleType, input.tex);
		float alpha = floatInfo.x;
	if(blendModeInfo.z!=0)
	{
		alpha*=shaderTexture[2].Sample(SampleType, input.tex).a;
	}

	if(blendModeInfo.w != -1)
	{
		float4 tex = textureColor;
		float3 Rca;
		float Ra;
		float3 Sca = tex.rgb;
		float Sa  = tex.a;
		float3 Dca = color.rgb;
		float Da  = color.a;
		if(blendModeInfo.w == 0)
		{
			Rca = Dca*Da*Sa + Sca * (1.0 - Da);
			Ra  = Sa;
		}
		else if(blendModeInfo.w == 1)
		{
			Rca = Sca*Sa + Dca * (1.0 - Sa);
			Ra  = Sa + Da * (1.0 - Sa);
		}
		textureColor = float4(Rca, Ra);				
	}


	if (blendModeInfo.y==0) //copy main text
	{
		textureColor.a*=alpha;
	} 
	else if (blendModeInfo.y==1) // copy blendTex
	{
		textureColor = shaderTexture[1].Sample(SampleType, input.tex); 
		textureColor.a*=alpha;
	}
	else if (blendModeInfo.y==2)
	{	
		float4 tex = textureColor;
		int tBlendMode = blendModeInfo[0];
		float3  Sca =tex.rgb;
		float Sa  = tex.a;
		float4  des =shaderTexture[1].Sample(SampleType, input.dtexc);
		float3  Dca = des.rgb;
		float Da  = des.a;
		float3 Rca;
		float Ra;

		if (tBlendMode == 1) { // Normal
			Rca = Sca*Sa + Dca * (1.0 - Sa);
			Ra  = Sa + Da * (1.0 - Sa);
		}else if (tBlendMode == 2) { // Multiply
			Rca = Sca * Dca + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 3) { // Screen
			Rca = Sca + Dca - Sca * Dca;
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 4) { // Overlay
			float3 cf = float3(float(2.0 * Dca.r <= Da), float(2.0 * Dca.g <= Da), float(2.0 * Dca.b <= Da));
			float3 c0 = 2.0 * Sca * Dca + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			float3 c1 = Sca * (1.0 + Da) + Dca * (1.0 + Sa) - 2.0 * Dca * Sca - Da * Sa;
			Rca = c0 * cf + c1 * (1.0 - cf);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 5) { // Darken
			Rca = min(Sca * Da, Dca * Sa) + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 6) { // Lighten
			Rca = max(Sca * Da, Dca * Sa) + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 7) { // ColorDodge
			Rca = Sa * Da * min(Dca * Sa / max(Da * (Sa - Sca), 0.001), 1.0) + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 8) { // ColorBurn
			Rca = Sa * Da * (1.0 - min(Sa * (Da - Dca) / max(Sca * Da, 0.001), 1.0)) + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 9) { // SoftLight
			Rca = Dca * (Sa + (2.0 * Sca - Sa) * (1.0 - Dca / max(Da, 0.001))) + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 10) { // HardLight
			float3 cf = float3(float(2.0 * Sca.r <= Sa), float(2.0 * Sca.g <= Sa), float(2.0 * Sca.b <= Sa));
			float3 c0 = 2.0 * Sca * Dca + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			float3 c1 = Sca * (1.0 + Da) + Dca * (1.0 + Sa) - 2.0 * Dca * Sca - Da * Sa;
			Rca = c0 * cf + c1 * (1.0 - cf);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 11) { // Difference
			Rca = Sca + Dca - 2.0 * min(Sca * Da, Dca * Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 12) { // Exclusion
			Rca = (Sca * Da + Dca * Sa - 2.0 * Sca * Dca) + Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 17) { // Clear
			Rca = float3(0.0f,0.0f,0.0f);
			Ra  = 0.0;
		}else if (tBlendMode == 18) { // Copy
			Rca = Sca;
			Ra  = Sa;
		}else if (tBlendMode == 19) { // SourceIn
			Rca = Sca * Da;
			Ra  = Sa * Da;
		}else if (tBlendMode == 20) { // SourceOut
			Rca = Sca * (1.0 - Da);
			Ra  = Sa * (1.0 - Da);
		}else if (tBlendMode == 21) { // SourceAtop
			Rca = Sca * Da + Dca * (1.0 - Sa);
			Ra  = Da;
		}else if (tBlendMode == 22) { // DestinationOver
			Rca = Dca + Sca * (1.0 - Da);
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 23) { // DestinationIn
			Rca = Dca * Sa;
			Ra  = Sa * Da;
		}else if (tBlendMode == 24) { // DestinationOut
			Rca = Dca * (1.0 - Sa);
			Ra  = Da * (1.0 - Sa);
		}else if (tBlendMode == 25) { // DestinationAtop
			Rca = Dca * Sa + Sca * (1.0 - Da);
			Ra  = Sa;
		}else if (tBlendMode == 26) { // XOR
			Rca = Sca * (1.0 - Da) + Dca * (1.0 - Sa);
			Ra  = Sa + Da - 2.0 * Sa * Da;
		}else if (tBlendMode == 27) { // PlusDarker
			Rca = Sca + Dca - Sa * Da;
			Ra  = Sa + Da - Sa * Da;
		}else if (tBlendMode == 28) { // PlusLighter
			Rca = Sca + Dca;
			Ra  = Sa + Da;
		}
		textureColor = float4(Rca, Ra);
	}
	textureColor.a*=alpha;
	return textureColor;	
}