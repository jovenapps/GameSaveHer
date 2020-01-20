using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HighScores 
{
	private const string KEY = "cblocker_hs_data";
	private const string DELIMITER = ",";

	public HighScoreData[] scoreList = new HighScoreData[5];
	private string log = "";


	public HighScores ()
	{
		scoreList = new HighScoreData[5];
	}

	public void Save ()
	{
		// Saving is done by getting the strings of all high score data and creating the csv
		string saveText = "";
		string t = "";

		for(int i=0; i < scoreList.Length; i++)
		{
			t = scoreList[i].GetString();
			if(!string.IsNullOrEmpty(t))
			{
				if(i > 0)
				{
					saveText +=DELIMITER;
				}
				saveText += t;
			}
		}

		PlayerPrefs.SetString(KEY, saveText);
	}

	public void Load ()
	{
		string saveText = UnityEngine.PlayerPrefs.GetString(KEY, string.Empty);
		if (!string.IsNullOrEmpty(saveText))
		{
			string[] parts = saveText.Split(DELIMITER.ToCharArray());
			if(parts.Length > 0)
			{
				for(int i=0; i < scoreList.Length; i++)
				{
					scoreList[i].Reset();
				}

				int scIdx = 0;
				HighScoreData tempData = new HighScoreData();
				for(int i=0; i < parts.Length; i++)
				{
					tempData.SetData(parts[i]);
					if(tempData.score > 0 && !string.IsNullOrEmpty(tempData.name))
					{
						// Check if loaded a valid data
						scoreList[scIdx].name = tempData.name;
						scoreList[scIdx].score = tempData.score;
						tempData.Reset();
						scIdx++;
					}
				}
				return;
			}
		}

	   // No data found
	   scoreList = new HighScoreData[5];
		UpdateLog();
	}

	public int CheckScoreSlot (long newScore)
	{
		int scoreSlot = -1;
		for(int i=0; i < scoreList.Length; i++)
		{
			if(newScore > scoreList[i].score || scoreList[i].score == 0)
			{
				scoreSlot = i;
				break;
			}
		}
		return scoreSlot;
	}

	public void AddHighScore (long newScore, string playerName)
	{
		int scoreSlot = CheckScoreSlot(newScore);
		if(scoreSlot < 0 || scoreSlot > scoreList.Length)
			return;

		if(string.IsNullOrEmpty(playerName))
			playerName = "Player?";

		if(playerName.Length > 10)
			playerName = playerName.Substring(0, 10);

		string tempName = scoreList[scoreSlot].name;
		long tempScore = scoreList[scoreSlot].score;
		scoreList[scoreSlot].name = playerName;
		scoreList[scoreSlot].score = newScore;

		string prevName = tempName;
		long prevScore = 0;
		for(int i = scoreSlot+1; i < scoreList.Length; i++)
		{
			prevName = scoreList[i].name;
			prevScore = scoreList[i].score;
			scoreList[i].name = tempName;
			scoreList[i].score = tempScore;
			tempName = prevName;
			tempScore = prevScore;
		}
		Save();
		UpdateLog();
	}

	public string GetLog ()
	{
		return log;
	}

	private void UpdateLog ()
	{
		log = string.Empty;
		string newLine = "\n";
		string format = "{0}{1}{2}\t{3:n0}";
		for(int i=0; i < scoreList.Length; i++)
		{
			if(i>0)
			{
				log = string.Format(format, log, newLine, scoreList[i].name, scoreList[i].score);
			}
			else
			{
				log = string.Format(format, log, "", scoreList[i].name, scoreList[i].score);
			}
		}
	}
}


[System.Serializable]
public struct HighScoreData
{
	private const string DELIMITER = ">>>";

	public string name;
	public long score;

	public string GetString ()
	{
		return string.Format("{0}{1}{2}", name, DELIMITER, score.ToString());
	}

	public void Reset ()
	{
		name = "---";
		score = 0;
	}

	public void SetData (string dataString)
	{
		name = "";
		score = 0;

		if(!string.IsNullOrEmpty(dataString))
		{
			string[] parts = dataString.Split(DELIMITER.ToCharArray());
			if(parts.Length >= 2)
			{
				long temp = score;
				if(long.TryParse(parts[1], out temp))
				{
					score = temp;
					name = parts[0];
				}
			}
		}
	}
}

