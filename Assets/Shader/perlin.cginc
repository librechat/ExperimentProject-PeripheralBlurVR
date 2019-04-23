#ifndef PERLIN_NOISE
#define PERLIN_NOISE

static const fixed2 g2[8] = {
	fixed2(1,2), fixed2(2,1), fixed2(-1, 2), fixed2(-2,1), fixed2(-2,-1), fixed2(-1,-2), fixed2(1,-2), fixed2(2,-1)
};
static const fixed3 g3[12] = {
	fixed3(0,1,1), fixed3(0,1,-1), fixed3(0,-1,-1), fixed3(0,-1,1), fixed3(1,0,1), fixed3(1,0,-1), fixed3(-1,0,1), fixed3(-1,0,-1),
	fixed3(1,1,0), fixed3(1,-1,0), fixed3(-1,1,0), fixed3(-1,-1,0)
};
static const int p[256] = {151,160,137,91,90,15,
  131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
  190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
  88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,
  77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
  102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
  135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
  5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
  223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
  129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
  251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
  49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
  138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};


inline fixed interpolation(fixed x, fixed y, fixed t){
	return x + t * (y - x);
}
inline fixed curve(fixed t){
	// 6t^6 - 15t^5 + 10t^4
	return t * t * t * (t * (t * 6 - 15) +10);
}
int g2index(int x, int y){
	//pseudo randomly choose between
	//(1,2),(2,1),(-1, 2),(-2,1),(-2,-1),(-1,-2),(1,-2),(2,-1)
	return p[(p[x]+y) &255 ] & 7;
}
int g3index(int x, int y, int z){
	//pseudo randomly choose between
	//(0,1,1),(0,1,-1),(0,-1,-1),(0,-1,1)
	//(1,0,1),(1,0,-1),(-1,0,1),(-1,0,-1)
	//(1,1,0),(1,-1,0),(-1,1,0),(-1,-1,0)
	return p[(p[(p[x]+y) & 255]+z) &255] % 12;
}
inline fixed perlin2(fixed2 pos){
	fixed len = sqrt(5.0f);
	fixed x = pos.x; fixed y = pos.y;
	int x0 = (int)x;
	int x1 = x0 + 1;
	
	int y0 = (int)y;
	int y1 = x0 + 1;
	
	fixed dx0 = x - x0;
	fixed dx1 = x - x1; // dx0 - 1
	
	fixed dy0 = y - y0;
	fixed dy1 = y - y1;
	
	fixed smooth_x = curve(dx0);
	fixed smooth_y = curve(dy0);
	
	fixed2 delta;
	delta.x = dx0; delta.y = dy0;
	fixed p00 = dot(delta, g2[g2index(x0, y0)]) / len;
	delta.x = dx1;
	fixed p10 = dot(delta, g2[g2index(x1, y0)]) / len;
	fixed p0 = interpolation(p00, p10, smooth_x);	
	
	delta.y = dy1;
	fixed p11 = dot(delta, g2[g2index(x1, y1)]) / len;
	delta.x = dx0;
	fixed p01 = dot(delta, g2[g2index(x0, y1)]) / len;
	fixed p1 = interpolation(p01, p11, smooth_x);
	
	return interpolation(p0, p1, smooth_y);
}
inline fixed perlin3(fixed3 pos){
	fixed x = pos.x; fixed y = pos.y; fixed z = pos.z;
	fixed len = sqrt(2.0f);
	int x0 = (int)x;
	int x1 = x0 + 1;
	
	int y0 = (int)y;
	int y1 = y0 + 1;
	
	int z0 = (int)z;
	int z1 = z0 + 1;
	
	fixed dx0 = x - x0; fixed dx1 = x - x1;
	fixed dy0 = y - y0; fixed dy1 = y - y1;
	fixed dz0 = z - z0; fixed dz1 = z - z1;
	
	fixed smooth_x = curve(dx0);
	fixed smooth_y = curve(dy0);
	fixed smooth_z = curve(dz0);
	
	fixed3 delta;
	delta.x = dx0; delta.y = dy0; delta.z = dz0;
	fixed p000 = dot(delta, g3[g3index(x0, y0, z0)]) / len;
	delta.x = dx1;
	fixed p100 = dot(delta, g3[g3index(x1, y0, z0)]) / len;
	fixed p00 = interpolation(p000, p100, smooth_x);
	
	delta.y = dy1;
	fixed p110 = dot(delta, g3[g3index(x1, y1, z0)]) / len;
	delta.x = dx0;
	fixed p010 = dot(delta, g3[g3index(x0, y1, z0)]) / len;
	fixed p10 = interpolation(p010, p110, smooth_x);
	
	fixed p0 = interpolation(p00, p10, smooth_y);
	
	delta.z = dz1;
	fixed p011 = dot(delta, g3[g3index(x0, y1, z1)]) / len;
	delta.x = dx1;
	fixed p111 = dot(delta, g3[g3index(x1, y1, z1)]) / len;
	fixed p11 = interpolation(p011, p111, smooth_x);
	
	delta.y = dy0;
	fixed p101 = dot(delta, g3[g3index(x1, y0, z1)]) / len;
	delta.x = dx0;
	fixed p001 = dot(delta, g3[g3index(x0, y0, z1)]) / len;
	fixed p01 = interpolation(p001, p101, smooth_x);
	
	fixed p1 = interpolation(p01, p11, smooth_y);
	
	return interpolation(p0, p1, smooth_z);
}
//advance: octaves
inline fixed octave2(fixed2 pos, int power){
	fixed sum = 0;
	for(int i=0; i<=power;i++){
		fixed p = pow(2, power);
		sum += perlin2(pos * p) / p;
	}
	return sum;
}

inline fixed octave3(fixed3 pos, int power){
	fixed sum = 0;
	for(int i=0; i<=power;i++){
		fixed p = pow(2, power);
		sum += perlin3(pos * p) / p;
	}
	return sum;
}

#endif

//https://zh.wikipedia.org/wiki/Perlin%E5%99%AA%E5%A3%B0#%E7%BB%8F%E5%85%B8Perlin%E5%99%AA%E5%A3%B0
//http://blog.csdn.net/Mahabharata_/article/details/54743672
//http://blog.csdn.net/seamanj/article/details/11271097