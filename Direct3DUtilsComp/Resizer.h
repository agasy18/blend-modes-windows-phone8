#include <vector>
#include "Direct3DInterop.h"

namespace Direct3DUtilsComp
{
public ref class HResizeLinear sealed
{
	/*inline unsigned char getpixel(int* in, 
		int src_width, int src_height, unsigned x, unsigned y, int channel);*/

public:
	void resize(int* input, int* output, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight);
};
}
