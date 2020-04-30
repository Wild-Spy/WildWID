#include "RndTrackPlayer.h"
#include "MX_WavTrig.h"
#include "RndGen.h"

static rnd_trk_last_played_track = 0;


//Picks a random track and plays it
//Will not play the same track twice in a row (unless there's only one track to play)
//Uses a pseudo-random generator to pick the random track.  See the file rnd_gen_test.xlsx for an analysis of the random generator.
//Min track ID is always 1.
//Max track id is 2 (RND_TRK_MAX_TRACK_ID) the first time this is called.  After that the AudioGetTrackCount() should return a sane
//value and we use that as the max track ID.  
//In this way we expect that the sd card contains audio files from 1 to N and does not contain any beyond that.
//For example, the sd could contain tracks 001, 002, 003, ... 010.  But it should not contain tracks 001, 002, 005, 006, skipping 
//tracks 3 and 4 otherwise we could try and play those at some point.
//There should never be a 0'th track on the card, the indexing starts at 1.
int32_t PlayRandomTrack() {
	uint16_t track_id_to_play;
	uint16_t min_id = RND_TRK_MIN_TRACK_ID;
	uint16_t max_id = RND_TRK_MAX_TRACK_ID;
	uint16_t track_count = AudioGetTrackCount();
	
	if (track_count > 0) max_id = track_count;
	
	if (min_id == max_id) {
		track_id_to_play = max_id;
	} else {
		do {
			track_id_to_play = RndGen_Generate_Range(min_id, max_id);
		} while (track_id_to_play == rnd_trk_last_played_track || track_id_to_play < min_id || track_id_to_play > max_id);
	}
	
	mx_err_t resp = AudioPlay(track_id_to_play);
	if (resp < 0) return resp;
	
	rnd_trk_last_played_track = track_id_to_play;
	return track_id_to_play;
}