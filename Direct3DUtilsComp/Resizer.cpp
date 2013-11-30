#include "pch.h"
#include <vector>
#include "Resizer.h"

using namespace Direct3DUtilsComp;

 /*inline unsigned char HResizeLinear::getpixel(int* in, 
    int src_width, int src_height, unsigned x, unsigned y, int channel)
{
    if (x < src_width && y < src_height)
        return in[(x * 3 * src_width) + (3 * y) + channel];

    return 0;
}*/

void HResizeLinear::resize(int* input, int* output, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight) 
{    
    int a, b, c, d, x, y, index;
    float x_ratio = ((float)(sourceWidth - 1)) / targetWidth;
    float y_ratio = ((float)(sourceHeight - 1)) / targetHeight;
    float x_diff, y_diff, blue, red, green,alpha ;
    int offset = 0 ;

    for (int i = 0; i < targetHeight; i++) 
    {
        for (int j = 0; j < targetWidth; j++) 
        {
            x = (int)(x_ratio * j) ;
            y = (int)(y_ratio * i) ;
            x_diff = (x_ratio * j) - x ;
            y_diff = (y_ratio * i) - y ;
            index = (y * sourceWidth + x) ;                
            a = input[index] ;
            b = input[index + 1] ;
            c = input[index + sourceWidth] ;
            d = input[index + sourceWidth + 1] ;

            // blue element
            blue = (a&0xff)*(1-x_diff)*(1-y_diff) + (b&0xff)*(x_diff)*(1-y_diff) +
                   (c&0xff)*(y_diff)*(1-x_diff)   + (d&0xff)*(x_diff*y_diff);

            // green element
            green = ((a>>8)&0xff)*(1-x_diff)*(1-y_diff) + ((b>>8)&0xff)*(x_diff)*(1-y_diff) +
                    ((c>>8)&0xff)*(y_diff)*(1-x_diff)   + ((d>>8)&0xff)*(x_diff*y_diff);

            // red element
            red = ((a>>16)&0xff)*(1-x_diff)*(1-y_diff) + ((b>>16)&0xff)*(x_diff)*(1-y_diff) +
                  ((c>>16)&0xff)*(y_diff)*(1-x_diff)   + ((d>>16)&0xff)*(x_diff*y_diff);

			 // red element
            alpha = ((a>>24)&0xff)*(1-x_diff)*(1-y_diff) + ((b>>24)&0xff)*(x_diff)*(1-y_diff) +
                  ((c>>24)&0xff)*(y_diff)*(1-x_diff)   + ((d>>24)&0xff)*(x_diff*y_diff);

            output [offset++] = 
				  ((((int)alpha)   << 24)&0xff000000)|
                    ((((int)red)   << 16)&0xff0000) |
                    ((((int)green) << 8)&0xff00) |
                    ((((int)blue)  << 0)&0xff);
        }
    }
}
	
	/*bicubicresize(int* in, int* dest, 
    int src_width, int src_height, int dest_width, int dest_height)
{
    int* out = new int[dest_width * dest_height * 3];

    const float tx = float(src_width) / dest_width;
    const float ty = float(src_height) / dest_height;
    const int channels = 3;
    const std::size_t row_stride = dest_width * channels;

    unsigned char C[5] = { 0 };

    for (int i = 0; i < dest_width; ++i)
    {
        for (int j = 0; j < dest_height; ++j)
        {
            const int x = int(tx * j);
            const int y = int(ty * i);
            const float dx = tx * j - x;
            const float dy = ty * i - y;

            for (int k = 0; k < 3; ++k)
            {
                for (int jj = 0; jj < 4; ++jj)
                {
                    const int z = y - 1 + jj;
                    unsigned char a0 = getpixel(in, src_width, src_height, z, x, k);
                    unsigned char d0 = getpixel(in, src_width, src_height, z, x - 1, k) - a0;
                    unsigned char d2 = getpixel(in, src_width, src_height, z, x + 1, k) - a0;
                    unsigned char d3 = getpixel(in, src_width, src_height, z, x + 2, k) - a0;
                    unsigned char a1 = -1.0 / 3 * d0 + d2 - 1.0 / 6 * d3;
                    unsigned char a2 = 1.0 / 2 * d0 + 1.0 / 2 * d2;
                    unsigned char a3 = -1.0 / 6 * d0 - 1.0 / 2 * d2 + 1.0 / 6 * d3;
                    C[jj] = a0 + a1 * dx + a2 * dx * dx + a3 * dx * dx * dx;

                    d0 = C[0] - C[1];
                    d2 = C[2] - C[1];
                    d3 = C[3] - C[1];
                    a0 = C[1];
                    a1 = -1.0 / 3 * d0 + d2 -1.0 / 6 * d3;
                    a2 = 1.0 / 2 * d0 + 1.0 / 2 * d2;
                    a3 = -1.0 / 6 * d0 - 1.0 / 2 * d2 + 1.0 / 6 * d3;
                    out[i * row_stride + j * channels + k] = a0 + a1 * dy + a2 * dy * dy + a3 * dy * dy * dy;
                }
            }
        }
    }

	dest = out;
}*/