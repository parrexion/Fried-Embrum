using System;

public static class Constants {

	//Dialogue
	public const int DIALOGUE_PLAYERS_COUNT = 4;


	/// UTILITY FUNCTIONS

	public static string PlayTimeFromInt(int playTime, bool useSeconds) {
		int _seconds = (playTime % 60);
		int _minutes = (playTime / 60) % 60;
		int _hours = (playTime / 3600);
		if (useSeconds)
			return string.Format("{0} : {1:D2} : {2:D2}",_hours, _minutes, _seconds);
		else
			return string.Format("{0} : {1:D2}",_hours, _minutes);
	}
}

