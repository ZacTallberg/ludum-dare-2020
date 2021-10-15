using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Job : IEquatable<Job>
{

    internal int jobUID;
    public JobType type;
    public GameObject target;
    internal JobState state;

    public Job(JobType type, GameObject target) {
        jobUID = (target.GetInstanceID() << ((int) Mathf.Floor(Mathf.Log(Enum.GetNames(typeof(JobType)).Length + 1, 2)))) + (int) type;
        this.type = type;
        this.target = target;
        this.state = JobState.UNASSIGNED;
    }

    public bool Equals(Job other) {
        return other.jobUID == jobUID;
    }

    override public string ToString() {
        return String.Format("Job: [UID: {0}, type: {1}, state: {2}, target: {3}]", jobUID, type, state, target);
    }

    public void startJob() {
        state = JobState.IN_PROGRESS;
    }

    public void cancelJob() {
        state = JobState.CANCELED;
    }

    public void completeJob() {
        state = JobState.COMPLETE;
    }
}
