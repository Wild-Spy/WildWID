
#include "RndGen.h"

static uint32_t RndGen_seed; 

/* Call before first use of NextVal */
uint32_t RndGen_InitSeed(uint32_t seed)
{
	//Your code for random seed here
	RndGen_seed = seed;
	
	// Correct distribution errors in seed
	RndGen_Generate();
	RndGen_Generate();
	RndGen_Generate();
	return RndGen_Generate();
}

 /* Linear Congruential Generator 
  * Constants from  
  * "Numerical Recipes in C" 
  * by way of 
   * <http://en.wikipedia.org/wiki/Linear_congruential_generator#LCGs_in_common_use>
   * Note: Secure implementations may want to get uncommon/new LCG values
  */
uint32_t RndGen_Generate()
{
	RndGen_seed=RndGen_seed*1664525L+1013904223L;
	return RndGen_seed;
}

//Generate a number between 0 and 1
double RndGen_GenerateDouble()
{
	RndGen_seed=RndGen_seed*1664525L+1013904223L;
	return (double)RndGen_seed/0xFFFFFFFF;
}

uint16_t RndGen_Generate_Range(uint16_t min, uint16_t max)
{
	double fmin, fmax, fval;
	fmin = (double)min;
	fmax = (double)max;
	fval = RndGen_GenerateDouble();
	
	return (uint16_t)(fval*(fmax-fmin+1)+fmin);
}