#if FULL
using UnityEngine;
using System.Collections;
using ngui.ex;
using System.Collections.Generic;
using System.Text;

public class DevConsole : MonoBehaviour {

	public UITableLayout layout;
	public int maxLine = 100;
	public bool showStack;
	public LogType logLevel = LogType.Warning;

	private LinkedList<string> logs = new LinkedList<string>();

	void OnEnable() {
		if (Platform.isDebug && Application.isPlaying) {
			Application.logMessageReceived += WriteLog;
		}
	}

	void OnDisable() {
		if (Platform.isDebug && Application.isPlaying) {
			Application.logMessageReceived -= WriteLog;
		}
	}

	private StringBuilder str = new StringBuilder();
	private void WriteLog(string condition, string stack, LogType logType) {
		if (logType.CompareTo(logLevel) < 0) {
			return;
		}
		lock (str) {
			if (showStack) {
				str.Length = 0;
				str.Append(condition);
				str.Append("\n");
				str.Append(stack);
				logs.AddLast(str.ToString());
			} else {
				logs.AddLast(condition);
			}
			if (logs.Count > maxLine) {
				logs.RemoveFirst();
			}
		}
		layout.SetModel(this, logs);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Debug.Log("Test");
		}
	}
}
#endif