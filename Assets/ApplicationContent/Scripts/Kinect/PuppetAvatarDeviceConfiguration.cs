using UnityEngine;

[RequireComponent(typeof(TrackerHandler))]
public class PuppetAvatarDeviceConfiguration : MonoBehaviour
{
    public BackgroundData m_lastFrameData = new BackgroundData();

    // Handler for SkeletalTracking thread.
    private TrackerHandler trackerHandler;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;


    void Start()
    {
        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        trackerHandler = GetComponent<TrackerHandler>();
    }

    void Update()
    {
        if (m_skeletalTrackingProvider.IsRunning)
        {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                if (m_lastFrameData.NumOfBodies != 0)
                {
                    trackerHandler.updateTracker(m_lastFrameData);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }
    }
}