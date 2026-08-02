using System.Collections.Concurrent;

namespace SIMS.Application.Services;

/// <summary>
/// Singleton that provides one <see cref="SemaphoreSlim"/> per class ID.
///
/// Problem this solves
/// ───────────────────
/// <see cref="ClassService.EnrollStudentAsync"/> is a multi-step sequence:
///   1. Read class → check capacity
///   2. Read enrollments → check duplicate
///   3. Write enrollment record (enrollments.csv)
///   4. Write incremented count (classes.csv)
///
/// Steps 1-4 touch two separate CSV files, each guarded by its own
/// per-file semaphore in <c>CsvRepositoryBase</c>.  Those semaphores are
/// file-scoped, so two concurrent enrolment requests for the same class can
/// both pass the capacity check in step 1 and both commit in steps 3-4,
/// pushing CurrentEnrollment past MaxEnrollment.
///
/// Solution
/// ────────
/// Callers acquire this semaphore once around the entire check+write
/// sequence, serialising all enrolment attempts for the same class.
/// Each class ID gets its own slot so classes that are not related do not
/// block each other.
/// </summary>
public sealed class EnrollmentSemaphoreService
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _semaphores = new();

    /// <summary>
    /// Returns (and lazily creates) the semaphore for <paramref name="classId"/>.
    /// The semaphore has a maximum count of 1 — only one enrolment operation
    /// per class may be in flight at a time.
    /// </summary>
    public SemaphoreSlim GetSemaphore(int classId) =>
        _semaphores.GetOrAdd(classId, _ => new SemaphoreSlim(1, 1));
}
