using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JobManager : MonoBehaviour
{

    private JobManagementQueue jobQueue = new JobManagementQueue();

    private Coroutine coroutine;

    private List<IJobHandler> jobHandlers = new List<IJobHandler>();

    private Hashtable jobUIDToHandlerMappings = new Hashtable();

    /// <summary>
    /// Subscribes the provided job handler to this job manager.
    /// The job manager may attempt to assign jobs to this handler until it unsubscribes.
    /// </summary>
    /// <param name="subscriber">The job handler to subscribe to this job manager.</param>
    public void subscribe(IJobHandler subscriber) {
        lock(jobHandlers) {
            jobHandlers.Add(subscriber);
        }
    }

    /// <summary>
    /// Unsubscribes the provided job handler from this job manager.
    /// The job manager will no longer attempt to provide this handler with jobs to work.
    /// </summary>
    /// <param name="subscriber">The job handler to unsubscribe from this job manager.</param>
    public void unsubscribe(IJobHandler subscriber) {
        lock(jobHandlers) {
            jobHandlers.Remove(subscriber);
        }
    }

    /// <summary>
    /// Schedules a job that needs to be worked by an IJobHandler.
    /// </summary>
    /// <param name="job">The job to schedule for execution.</param>
    public void scheduleJob(Job job) {
        bool jobAddedSuccessfully = jobQueue.addJob(job);
        if (jobAddedSuccessfully && coroutine == null) {
            // start running the job assignment loop if it's not already running.
            coroutine = StartCoroutine(jobAssignmentLoop());
        }
    }

/// <summary>
/// Cancels a job, either waiting to be assigned or already in progress.
/// This function calls cancelJob() on the IJobHandler that holds ownership of an in progress job.
/// </summary>
/// <param name="job">The job to be canceled.</param>
    public void cancelJob(Job job) {
        jobQueue.removeJob(job);
        lock (jobUIDToHandlerMappings) {
            if (jobUIDToHandlerMappings.ContainsKey(job.jobUID)) {
                ((IJobHandler) jobUIDToHandlerMappings[job.jobUID]).cancelJob(job);
                job.cancelJob();
                jobUIDToHandlerMappings.Remove(job.jobUID);
            }
        }
    }

    /// <summary>
    /// Marks a job as complete by the IJobHandler that was assigned to execute it.
    /// </summary>
    /// <param name="job">The job that was completed by its handler.</param>
    public void completeJob(Job job) {
        job.completeJob();
        jobQueue.removeJob(job);
    }

    /// <summary>
    /// This is the loop that assigns any unassigned jobs to an owner IJobHandler.
    /// Should be envoked via a StartCoroutine() call.
    /// This loop will stop executing once the number of unassigned jobs in the jobQueue reaches 0.
    /// It will get started back up once a new job is added.
    /// </summary>
    /// <returns>IEnumerator</returns>
    public IEnumerator jobAssignmentLoop()
    {
        List<Job> unassignedJobs = new List<Job>(jobQueue.getUnassignedJobs());
        while(unassignedJobs.Count() > 0)
        {
            foreach (Job job in unassignedJobs) {
                lock(jobHandlers) {
                    IEnumerable<IJobHandler> eligibleHandlers =
                            from handler in jobHandlers
                            where handler.canTakeJob(job)
                            && handler.jobFitness(job) >= 0
                            orderby handler.jobFitness(job) descending
                            select handler;
                    foreach (IJobHandler handler in eligibleHandlers) {
                        if (handler.assignJob(job)) {
                            job.state = JobState.ASSIGNED;
                            lock (jobUIDToHandlerMappings) {
                                jobUIDToHandlerMappings[job.jobUID] = handler;
                            }
                            break;
                        }
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }

            unassignedJobs = new List<Job>(jobQueue.getUnassignedJobs());
        }
        lock(coroutine) {
            coroutine = null;
        }
    }

    void Start() {

    }

    void Update()
    {
        
    }
}
