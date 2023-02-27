using System.Collections.Generic;

public class ActivityHandler
{
    public ActivityHandler()
    {
        _activities = new List<Activity>();
    }

    public void RegisterActivity(Activity activity)
    {
        activity.Register();
        _activities.Add(activity);
    }
    public void ProcessActivities()
    {
        foreach (var activity in _activities)
        {
            activity.Process();
        }
    }

    public void SetActivities(ActivityData data)
    {
        foreach (var activity in _activities)
        {
            activity.Set(data);
        }
    }

    private List<Activity> _activities;
}