



__kernel void Fade(__read_only image2d_t rimg,__write_only image2d_t wimg)
{

    const sampler_t smp = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_NEAREST;
    int x = get_global_id(0);
    int y = get_global_id(1);
    

}

__kernel void draw(__global float* things,__write_only image2d_t wimg)
{
    int x = get_global_id(0)*6;
	int2 coord = (int2)((int)(things[x+1]),(int)things[x+2]);
	uint4 col = (uint4)((-50000*things[x+4]),(50000*things[x+5]),(-50000*things[x+5]),255);
    write_imageui(wimg,coord,col);
}

__kernel void black(__read_only image2d_t rimage,__write_only image2d_t wimage)
{

	const sampler_t smp = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_NEAREST;
	int x = get_global_id(0);
	int y = get_global_id(1);
    
	int2 coord = (int2)(x,y);
	uint4 col = (uint4)(255,0,0,0);
	
	write_imageui(wimage,coord,col);
}






