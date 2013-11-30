#include "pch.h"

char __logbuf[1000];
char __logbufFormat[160];
void LogFloat4(DirectX::XMFLOAT4 * point,const char * des)
{
	LogFormat("%s = %f,%f,%f,%f",des,point->x,point->y,point->z,point->w);
}