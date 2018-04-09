using System;
using UnityEngine;
using effect;
using comunity;

public class ClickEffect : comunity.Script
{
	public ParticlePool particlePool;

	void Start() {
		UICamera.onClick = SpawnClickEffect;
	}

	void OnDestroy() {
		if (UICamera.onClick == SpawnClickEffect) {
			UICamera.onClick = null;
		}
	}

	private void SpawnClickEffect(GameObject o) {
		if (UICamera.hoveredObject == null) {
			return;
		}
		Vector3 clickPos = UICamera.lastWorldPosition;
		Camera mainCam = Camera.main;
		Camera singletonCam = CameraEx.GetCamera(gameObject.layer);
		if (singletonCam != null && mainCam != null && mainCam != singletonCam) {
			clickPos = singletonCam.ScreenToWorldPoint(new Vector3(UICamera.lastEventPosition.x, UICamera.lastEventPosition.y, 0));
		}
		ParticleControl particle = particlePool.GetInstance();
		particle.ignoreTimeScale = true;
		particle.transform.position = clickPos;
		particle.Play();
	}
}
