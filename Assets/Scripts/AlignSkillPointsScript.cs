using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlignSkillPointsScript : MonoBehaviour
{

	public PlayerScript player;

	public enum skills
	{
		Vitality,
		Strength,
		Agility,
		Speed,
		Defence
	}
	public skills SkillStat = skills.Vitality;

	private Button butt;


	void Start()
	{
		butt = GetComponent<Button>();
	}

	void FixedUpdate()
	{
		if (player.skillPointsRemaining > 0)
		{
			butt.image.enabled = true;
		}
		else
		{
			butt.image.enabled = false;
		}
	}

	public void GotClicked()
	{
		if (player.skillPointsRemaining > 0)
		{
			switch (SkillStat)
			{
				case skills.Vitality:
					player.skill_vitality += 1;
					player.skillPointsRemaining -= 1;
					break;
				case skills.Strength:
					player.skill_strength += 1;
					player.skillPointsRemaining -= 1;
					break;
				case skills.Agility:
					player.skill_agility += 1;
					player.skillPointsRemaining -= 1;
					break;
				case skills.Speed:
					player.skill_speed += 1;
					player.skillPointsRemaining -= 1;
					break;
				case skills.Defence:
					break;
				default:
					break;
			}

		}
	}

}
