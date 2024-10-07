//2016 Spyblood Games

using UnityEngine;
using System.Collections;

[System.Serializable]
public class DayColors
{
	public Color skyColor;
	public Color equatorColor;
	public Color horizonColor;
}

public class DayAndNightControl : MonoBehaviour {
	public bool StartDay; //start game as day time
	public GameObject StarDome;
	public GameObject moonState;
	public GameObject moon;
	public DayColors dawnColors;
	public DayColors dayColors;
	public DayColors nightColors;
	public int currentDay = 0; //day 8287... still stuck in this grass prison... no esacape... no freedom...
	public Light directionalLight; //the directional light in the scene we're going to work with
	public float SecondsInAFullDay = 120f; //in realtime, this is about two minutes by default. (every 1 minute/60 seconds is day in game)
	[Range(0,1)]
	public float currentTime = 0; //at default when you press play, it will be nightTime. (0 = night, 1 = day)
	[HideInInspector]
	public float timeMultiplier = 1f; //how fast the day goes by regardless of the secondsInAFullDay var. lower values will make the days go by longer, while higher values make it go faster. This may be useful if you're siumulating seasons where daylight and night times are altered.
	public bool showUI;
	float lightIntensity; //static variable to see what the current light's insensity is in the inspector
	Material starMat;

	Camera targetCam;

	// Use this for initialization
	void Start () {
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
		foreach (Camera c in GameObject.FindObjectsOfType<Camera>())
		{
			if (c.isActiveAndEnabled) {
				targetCam = c;
			}
		}
		lightIntensity = directionalLight.intensity; //what's the current intensity of the light
		starMat = StarDome.GetComponentInChildren<MeshRenderer> ().material;
		if (StartDay) {
			currentTime = 0.3f; //start at morning
			starMat.color = new Color(1f,1f,1f,0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateLight();
		currentTime += (Time.deltaTime / SecondsInAFullDay) * timeMultiplier;
		if (currentTime >= 1) {
			currentTime = 0;//once we hit "midnight"; any time after that sunrise will begin.
			currentDay++; //make the day counter go up
		}
	}
	void UpdateLight()
{
    StarDome.transform.Rotate(new Vector3(0, 2f * Time.deltaTime, 0));
    moon.transform.LookAt(targetCam.transform);
    directionalLight.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 90, 170, 0);
    moonState.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 100, 170, 0);

    float intensityMultiplier = 1;

    // 조명 밝기 변경: 밤에는 0, 낮에는 1
    if (currentTime <= 0.25f || currentTime >= 0.75f)
    {
        intensityMultiplier = 0; // 밤에 조명 밝기를 0으로 설정
        starMat.color = Color.Lerp(starMat.color, new Color(1, 1, 1, 1), Time.deltaTime); // 밤에는 별의 색상
    }
    else if (currentTime > 0.25f && currentTime < 0.75f)
    {
        // 아침부터 낮까지는 조명 밝기 서서히 증가, 낮부터 저녁까지는 감소
        if (currentTime <= 0.5f)
        {
            intensityMultiplier = Mathf.Lerp(0, 1, (currentTime - 0.25f) * 4); // 0.25f ~ 0.5f 사이에서 0 -> 1
        }
        else
        {
            intensityMultiplier = Mathf.Lerp(1, 0, (currentTime - 0.5f) * 4); // 0.5f ~ 0.75f 사이에서 1 -> 0
        }
        starMat.color = Color.Lerp(starMat.color, new Color(1, 1, 1, 0), Time.deltaTime); // 낮에는 별이 투명해짐
    }

    // 안개 색상 변경: 빠른 변화를 위해 저녁부터 어두워지도록 변경
    Color fogColor;

    if (currentTime > 0.25f && currentTime < 0.4f) // 새벽에서 낮으로
    {
        // 밝은 회색으로 빠르게 변화
        fogColor = Color.Lerp(new Color(0.3f, 0.3f, 0.3f), new Color(0.7f, 0.7f, 0.7f), (currentTime - 0.25f) * 6); // 빠르게 변화
    }
    else if (currentTime > 0.5f && currentTime < 0.75f) // 낮에서 저녁으로
    {
        // 어두운 회색으로 빠르게 변화
        fogColor = Color.Lerp(new Color(0.7f, 0.7f, 0.7f), new Color(0.2f, 0.2f, 0.2f), (currentTime - 0.5f) * 4); // 저녁부터 천천히 어두워짐
    }
    else if (currentTime >= 0.75f) // 밤으로 전환 시 완전히 어두워짐
    {
        fogColor = Color.Lerp(new Color(0.2f, 0.2f, 0.2f), new Color(0.1f, 0.1f, 0.1f), (currentTime - 0.75f) * 4); // 완전히 어두워짐
    }
    else
    {
        // 그 외 시간대: 낮과 밤 사이의 부드러운 변화
        fogColor = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), new Color(0.7f, 0.7f, 0.7f), Mathf.Sin(currentTime * Mathf.PI));
    }

    RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColor, Time.deltaTime);

    // 환경 색상 변경: 밤 -> 새벽 -> 낮 -> 저녁 -> 밤 순환
    RenderSettings.ambientSkyColor = Color.Lerp(nightColors.skyColor, dayColors.skyColor, Mathf.Sin(currentTime * Mathf.PI));
    RenderSettings.ambientEquatorColor = Color.Lerp(nightColors.equatorColor, dayColors.equatorColor, Mathf.Sin(currentTime * Mathf.PI));
    RenderSettings.ambientGroundColor = Color.Lerp(nightColors.horizonColor, dayColors.horizonColor, Mathf.Sin(currentTime * Mathf.PI));

    // 조명 밝기 적용
    directionalLight.intensity = lightIntensity * intensityMultiplier;
}


	public string TimeOfDay ()
	{
	string dayState = "";
		if (currentTime > 0f && currentTime < 0.1f) {
			dayState = "Midnight";
		}
		if (currentTime < 0.5f && currentTime > 0.1f)
		{
			dayState = "Morning";

		}
		if (currentTime > 0.5f && currentTime < 0.6f)
		{
			dayState = "Mid Noon";
		}
		if (currentTime > 0.6f && currentTime < 0.8f)
		{
			dayState = "Evening";

		}
		if (currentTime > 0.8f && currentTime < 1f)
		{
			dayState = "Night";
		}
		return dayState;
	}

	void OnGUI()
	{
		//debug GUI on screen visuals
		if (showUI) {
			GUILayout.Box ("Day: " + currentDay);
			GUILayout.Box (TimeOfDay ());
			GUILayout.Box ("Time slider");
			GUILayout.VerticalSlider (currentTime, 0f, 1f);
		}
	}
}
