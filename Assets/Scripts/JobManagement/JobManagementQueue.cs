using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// A JobQueue with extra handling needed for the JobManager class.
/// </summary>
internal class JobManagementQueue : JobQueue
{

    /// <summary>
    /// Gets all jobs in this queue that are in state JobState.UNASSIGNED.
    /// </summary>
    /// <returns>An IEnumerable<Job> collection of all unassigned jobs in this queue.</returns>
    internal IEnumerable<Job> getUnassignedJobs() {
        lock (allJobs) {
            return from job in allJobs
                    where job.state == JobState.UNASSIGNED
                    select job;
        }
    }

}