using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Provides a simple, thread-safe Queue for storing Job objects.
/// </summary>
public class JobQueue
{

    public List<Job> allJobs = new List<Job>();

    /// <summary>
    /// Adds a job to this queue, deduping on the job's jobUID.
    /// </summary>
    /// <param name="job">The job to add to this queue.</param>
    /// <returns>True if the job is added to the queue;
    /// false if the operation fails or if the job already exists in this queue.</returns>
    public virtual bool addJob(Job job) {
        lock (allJobs) {
            if (!allJobs.Contains(job)) {
                allJobs.Add(job);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes a specified job from this queue.
    /// </summary>
    /// <param name="job">The job to remove from this queue.</param>
    /// <returns>True if at least one job was removed from the queue; false if the job was not found in the queue.</returns>
    public virtual bool removeJob(Job job) {
        lock (allJobs) {
            int removedCount = allJobs.RemoveAll(j => j.jobUID == job.jobUID);
            return removedCount > 0;
        }
    }

    /// <summary>
    /// Polls the head of the job queue. The job returned will have been removed from this job queue.
    /// </summary>
    /// <returns>The oldest element in this job queue; null if the job queue is empty.</returns>
    public virtual Job poll() {
        Job polledJob = null;
        lock (allJobs) {
            if (allJobs.Count > 0) {
                polledJob = allJobs[0];
                removeJob(polledJob);
            }
        }
        return polledJob;
    }

}