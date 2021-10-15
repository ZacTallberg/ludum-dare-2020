using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A JobQueue with a size limit.
/// Provides insight into how full the queue is, allowing fullness to be taken into account
/// in the processes that perform decision making about job assignments.
/// </summary>
public class LimitedJobQueue : JobQueue
{
    private int currentQueueLength;
    private int maxQueueLength;

    public LimitedJobQueue(int sizeLimit) {
        if (sizeLimit <= 0) {
            throw new ArgumentException("Size limit must be > 0.");
        }
        maxQueueLength = sizeLimit;
    }

    public override bool addJob(Job job) {
        // prevent adding a job if queue is at max limit
        if (currentQueueLength >= maxQueueLength) return false;

        if (base.addJob(job)) {
            currentQueueLength++;
            return true;
        }
        
        return false;
    }

    public override bool removeJob(Job job) {
        // fail-fast here if queue is empty
        if (currentQueueLength <= 0) return false;

        if (base.removeJob(job)) {
            // tbf there's a possibility that the base class implementation removes more than 1 job
            // if at some point our deduping logic is circumvented...
            // But for now we'll assume the base class implementation removed only 1 job. 
            currentQueueLength--;
            return true;
        }

        return false;
    }

    public override Job poll() {
        // fail-fast here if queue is empty
        if (currentQueueLength <= 0) return null;

        Job polledJob = base.poll();
        // don't have to decrement - base.poll calls removeJob,
        // which will call our overriden removeJob method in this class,
        // which already decrements the current length field

        return polledJob;
    }

    /// <summary>
    /// Gets the max size that this queue will allow.
    /// </summary>
    /// <returns>An int representing the max size this queue can be.</returns>
    public int getQueueMaxSize() {
        return maxQueueLength;
    }

    /// <summary>
    /// Gets the current size of this queue.
    /// </summary>
    /// <returns>An int representing the current size of this queue.</returns>
    public int Count() {
        return currentQueueLength;
    }

    /// <summary>
    /// Calculates the fraction of used space in the queue.
    /// </summary>
    /// <returns>A float representing the percentage of used space in this queue.</returns>
    public float filledRatio() {
        return ((float) currentQueueLength) / ((float) maxQueueLength);
    }

    /// <summary>
    /// Calculates the fraction of space available in the queue.
    /// </summary>
    /// <returns>A float representing the percentage of available space in this queue.</returns>
    public float emptyRatio() {
        return 1f - filledRatio();
    }
}