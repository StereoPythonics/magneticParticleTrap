
float3 getPDeriv(float3 p)
{
	float x = p.s0;
	float y = p.s1 -960;
	float z = p.s2;

	float I = 1.0;
	float invh = 0.002;
	float fec = 4.0;
	float iyyzz = 1.0/(y*y + z*z);
	float dx = 0;
	float dy = y*fec*I*((2.0*invh*z)-1.0)*iyyzz*iyyzz*rsqrt(invh*invh + (iyyzz)*(1.0 - 2.0*z*invh));
	float dz = fec*I*(iyyzz*iyyzz)*(-2*z - 2*invh*(y*y - z*z))*rsqrt(invh*invh + iyyzz*(1.0 - 2.0*z*invh));

	return (float3)(dx,dy,dz);
}


__kernel void magnitate(__global float* mols)
{
	// mols are x,y,z,vx,vy,vz
	const float tstep = 0.8;
    int x = get_global_id(0)*6;

	float3 ipos = (float3)(mols[x],mols[x+1],mols[x+2]);
	float3 ivel = (float3) (mols[x+3],mols[x+4],mols[x+5]);
	float3 k1pos;
	float3 k1vel;
	float3 k2pos;
	float3 k2vel;
	float3 k3pos;
	float3 k3vel;
	float3 k4pos;
	float3 k4vel;
	for(int i = 0; i<20; i++)
    {

		k1pos = tstep*ivel;
		k1vel = -tstep*getPDeriv(ipos);
		//k2pos = tstep*(ivel + 0.5*k1vel);
		//k2vel = -tstep*getPDeriv(ipos + 0.5*k1pos);
		//k3pos = tstep*(ivel + 0.5*k2vel);
		//k3vel = -tstep*getPDeriv(ipos + 0.5*k2pos);
		//k4pos = tstep*(ivel + k3vel);
		//k4vel = -tstep*getPDeriv(ipos + k3pos);

		ipos += 6*0.1666666*(k1pos);// + 2*k2pos + 2*k3pos + k4pos);
		ivel += 6*0.1666666*(k1vel);// + 2*k2vel + 2*k3vel + k4vel);
    }
	float hundred = 0.1;
	mols[x] = ipos.s0;
	mols[x+1] = ipos.s1;
	mols[x+2] = ipos.s2;
	mols[x+3] = ivel.s0;//*0.99999;
	mols[x+4] = clamp(ivel.s1,-hundred,hundred);//*0.99999;
	mols[x+5] = clamp(ivel.s2,-hundred,hundred);//*0.99999;

}