#pragma once

#include <wrl/client.h>
#include <d3d11_1.h>
#include <DirectXMath.h>
#include <memory>
#include <agile.h>

#define MIN(a,b) (((a)<(b))?(a):(b))
#define MAX(a,b) (((a)>(b))?(a):(b))
#define NORMALIZE(x,a1,a2)(MAX(MIN(x,a1),a2))


extern char __logbuf[1000];
extern char __logbufFormat[160];
#ifdef DEBUG
#define  LogFormat(format,...) 									  \
{																				  \
	sprintf_s(__logbufFormat,"%d: %s \n",__LINE__,format);					  \
	sprintf_s(__logbuf,__logbufFormat,__VA_ARGS__);									  \
	OutputDebugStringA(__logbuf);													  \
}
#else
#define  LogFormat(format,...) 
#endif // DEBUG

void  LogFloat4(DirectX::XMFLOAT4 * point,const char * des);