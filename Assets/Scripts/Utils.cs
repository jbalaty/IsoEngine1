using UnityEngine;
using System.Collections;

public class Utils
{
	public static float ColorSize (Color c)
	{
		return c.r * c.r + c.g * c.g + c.b * c.b;
	}
}
