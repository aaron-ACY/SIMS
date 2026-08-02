namespace SIMS.Domain.Entities;

/// <summary>Records a student's enrolment in a specific class.</summary>
public class Enrollment
{
    public int      Id         { get; set; }

    /// <summary>FK to classes.csv (Class.Id).</summary>
    public int      ClassId    { get; set; }

    /// <summary>FK to students.csv (Student.Id).</summary>
    public int      StudentId  { get; set; }

    public DateTime EnrolledAt { get; set; }
    public bool     IsActive   { get; set; }
}
