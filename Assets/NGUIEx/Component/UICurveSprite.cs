#if FULL
//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;

using UnityEngine;

namespace ngui.ex {
}
public class UICurveSprite : MonoBehaviour {
	
	public delegate void SpriteSetter(UISprite sprite, int index);
	
	private List<GameObject> spriteList = new List<GameObject>();
	private QuadBezSpline worldSpline;
	private GameObject spriteObj01;
	
	public SpriteSetter spriteSetter;
	public UISprite sprite;
	
	void Start () {
	}
	
	public Vector3 Center() {
		return worldSpline == null ? Vector3.zero : worldSpline.SplineList[worldSpline.SplineList.Count/2];
	}
	
	private void InitSprite(int interpCount) {
		spriteObj01 = sprite.gameObject;
		spriteObj01.SetActive(false);
		
		if (interpCount > spriteList.Count)
		{
			int count = spriteList.Count;
			for(int i=count; i<interpCount; i++)
			{
				GameObject newSprite = spriteObj01.InstantiateEx();
				//GameObject newSprite = new GameObject("line_" + i);
				//newSprite.AddComponent<UIGraphWidget>();
				spriteList.Add(newSprite);
			}
		}
		else
		{
			for(int i=interpCount; i<spriteList.Count; i++)
			{
				spriteList[i].SetActive(false);
			}
		}
	}
	
	public void DrawSpline(Vector3 start, Vector3 end, Vector3 controllVector, float scaleRatio, int interpCount) {
		worldSpline = new QuadBezSpline(start, end, controllVector, interpCount);
		List<Vector3> splineList = worldSpline.SplineList;
		
		InitSprite(interpCount);
		
		for(int i=1; i<splineList.Count; i++)
		{
			int spriteIndex = i-1;
			GameObject newSprite = spriteList[spriteIndex];
			newSprite.SetActive(true);
			
			Transform nTrans = newSprite.transform;
			transform.SetParent(nTrans, false);
			Vector3 localPos = nTrans.localPosition;
			
			Vector3 point0 = splineList[i-1];
			Vector3 point1 = splineList[i];
			
			Vector3 center = (point0 + point1)/2f;
			Vector3 localScale = nTrans.localScale;
			localScale.y = Vector3.Distance(point0, point1) * scaleRatio;
			localScale.y += (localScale.y * 0.1f);
			
			nTrans.position = center;
			nTrans.localScale = localScale;
			
			Vector3 curLocalPos = nTrans.localPosition;
			curLocalPos.z = localPos.z;
			nTrans.localPosition = curLocalPos;
			
			RotateAngle(point0, point1, nTrans);
			
			if (spriteSetter != null)
			{
				spriteSetter(newSprite.GetComponent<UISprite>(), spriteIndex);
			}
		}
	}
	
	private void RotateAngle(Vector3 point0, Vector3 point1, Transform lineSprite) {
		Vector3 direction = Vector3.Normalize(point0 - point1);
		float angle = Vector3.Angle(Vector3.up, direction);
		//		float length = Vector3.Distance(point1, point0);
		
		if (direction.x > 0)
		{
			angle *= -1;
		}
		lineSprite.rotation = Quaternion.identity;
		lineSprite.RotateAround(lineSprite.position, Vector3.forward, angle);
	}
	
	void Update () {
		
	}
}

#endif