using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System;
using mulova.commons;


namespace ngui.ex
{
	public static class EventDelegateUtil {
		public static readonly Loggerx log = LogManager.GetLogger(typeof(EventDelegateUtil));
		
		public static EventDelegate GetEventDelegate(List<EventDelegate> list, MonoBehaviour script, Action<object> method) {
			return GetEventDelegate(list, script, method.Method.Name);
		}
		
		public static EventDelegate GetEventDelegate(List<EventDelegate> list, MonoBehaviour script, string methodName) {
			foreach (EventDelegate d in list) {
				if (d.target == script && d.methodName == methodName) {
					return d;
				}
			}
			return null;
		}
		
		public static void SetCallback<T>(List<EventDelegate> callbackList, Action<T> method, T param) where T:Object{
			callbackList.Clear();
			AddCallback(callbackList, method, param);
		}
		
		/// <summary>
		/// Adds the callback with Object param type
		/// </summary>
		/// <returns><c>true</c>, if callback was added, <c>false</c> otherwise.</returns>
		public static void AddCallback<T>(List<EventDelegate> callbackList, Action<T> method, T param) where T:Object{
			EventDelegate del = GetCallback(callbackList, method.Target as MonoBehaviour, method);
			if (del != null && del.parameters.Length == 1 && del.parameters[0].obj == param) 
			{
				return;
			}
			if (del != null)
			{
				EventDelegate.Remove(callbackList, del);
			} else 
			{
				del = new EventDelegate((MonoBehaviour)method.Target, method.Method.Name);
			}
			del.parameters[0].obj = param;
			EventDelegate.Add(callbackList, del);
		}
		
		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <returns><c>true</c>, if callback was added, <c>false</c> otherwise.</returns>
		/// <param name="callbackList">Callback list.</param>
		/// <param name="target">Target.</param>
		/// <param name="method">Method.</param>
		/// <param name="src">Source.</param>
		/// <param name="paramValue">Property value.</param>
		public static bool AddCallback<T>(List<EventDelegate> callbackList, Action<T> method, MonoBehaviour src, T param) where T:Object{
			MonoBehaviour target = (MonoBehaviour)method.Target;
			string methodName = method.Method.Name;
			EventDelegate del = GetCallback(callbackList, target, methodName);
			if (del != null && del.parameters.Length == 1 && del.parameters[0].obj == param) 
			{
				return false;
			}
			if (del != null)
			{
				EventDelegate.Remove(callbackList, del);
				log.Info("Callback '{0}()' is overrided", methodName);
			} else 
			{
				del = CreateDelegate(target, methodName, src, param);
			}
			EventDelegate.Add(callbackList, del);
			return true;
		}
		
		public static EventDelegate GetCallback<T>(List<EventDelegate> callbackList, MonoBehaviour target, Action<T> method) {
			return GetCallback(callbackList, target, method.Method.Name);
		}
		
		public static EventDelegate GetCallback(List<EventDelegate> callbackList, MonoBehaviour target, string methodName) {
			if (callbackList == null) {
				return null;
			}
			foreach (EventDelegate d in callbackList) {
				if (d.target == target && d.methodName == methodName) {
					return d;
				}
			}
			return null;
		}
		
		public static void RemoveCallback<T>(List<EventDelegate> callbackList, MonoBehaviour target, Action<T> method, MonoBehaviour src) where T:Object{
			RemoveCallback(callbackList, target, method.Method.Name, src);
		}
		
		public static void RemoveCallback(List<EventDelegate> callbackList, MonoBehaviour target, string methodName, MonoBehaviour src) {
			EventDelegate del = GetCallback(callbackList, target, methodName);
			if (del != null) {
				EventDelegate.Remove(callbackList, del);
				//			if (del.parameters.IsNotEmpty() && del.parameters[0].obj is CallbackParam) {
				//				CallbackParam callbackParam = del.parameters[0].obj as CallbackParam;
				//				callbackParam.gameObject.DestroyEx();
				//			} else {
				//				log.Warn("No matching callback to remove");
				//			}
			} else {
				log.Warn("No Callback to remove");
			}
		}
		
		private static EventDelegate CreateDelegate(MonoBehaviour target, string methodName, MonoBehaviour src, Object param) {
			EventDelegate del = new EventDelegate(target, methodName);
			del.parameters[0].obj = param;
			return del;
		}
	}
	
	
}
