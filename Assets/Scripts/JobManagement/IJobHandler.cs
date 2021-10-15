public interface IJobHandler
{
    /// <summary>
    /// Determines whether this handler is able to take a particular job right now.
    /// </summary>
    /// <param name="job">The job that this handler is being asked to work.</param>
    /// <returns>True if the handler can accept this specific job at this time; false otherwise.</returns>
    bool canTakeJob(Job job);

    /// <summary>
    /// Assigns the job to this handler, if possible.
    /// </summary>
    /// <param name="job">The job that this handler will work.</param>
    /// <returns>True if this handler correctly accepted the job; false otherwise.</returns>
    bool assignJob(Job job);

    /// <summary>
    /// Cancels the specified job.
    /// </summary>
    /// <param name="job">The job to cancel.</param>
    void cancelJob(Job job);

    /// <summary>
    /// Determines the fitness of a particular job to be worked by this handler.
    /// </summary>
    /// <param name="job">The job that this handler may be assigned.</param>
    /// <returns>
    /// Can return any float value.
    /// a fitness value < 0 means that this job should not be performed by this handler,
    ///     even if it's the most fit handler available.
    ///     This should be the case if canTakeJob() returns false for the specified job.
    /// a fitness value >= 0 means that this job can be worked by this handler.
    ///     A higher value indicates that the job is more convenient for this handler to perform
    ///     than a handler of the same type that returns a lower fitness value.
    /// </returns>
    float jobFitness(Job job);
}