using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;
using comunity;
 
namespace ngui.ex
{
    public class FPS : MonoBehaviour
    {
        public  float updateInterval = 0.5F;
        public UILabel label;
        
        private float accum   = 0; 
        private int   frames  = 0; 
        private float timeleft; 
        
        private float minframe = 60; 
        private string tframe;
        
        void Start()
        {
            if (Platform.isReleaseBuild) {
                Destroy(gameObject);
            }
            timeleft = updateInterval;  
			SceneManager.sceneLoaded += OnSceneLoaded;
        }

		void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			
		}

        void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale/Time.deltaTime;
            ++frames;
            
            if( timeleft <= 0.0 )
            {
                float fps = accum/frames;
                minframe = Mathf.Min(minframe, fps);    
                
                tframe = System.String.Format("{0:F1} (Min {1:F1})",fps, minframe);
                label.SetText(tframe);
                
                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
        
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			minframe = 60;
		}

        public void SetVisible(bool visible)
        {
            enabled = visible;
            label.enabled = visible;
        }
        
    }
}