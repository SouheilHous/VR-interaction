namespace UnityEditor.Experimental.EditorVR.Input
{
    sealed class OVRTouchInputToEvents : BaseVRInputToEventsSteamVR
    {
        protected override string DeviceName
        {
            get { return "Oculus Touch Controller"; }
        }
    }
}
